using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Reflection.Emit;
using ActionFramework;
using Agent.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Linq;
using ActionFramework.Configuration;
using System.Dynamic;
using Agent.GenericControllerProvider;
using Microsoft.Extensions.DependencyModel;
using Serilog;
using ActionFramework.Logger;

namespace Agent.Controllers.Generic
{
    public class RemoteControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        private AgentSettings agentSettings = Startup.AgentSettings;
        private string rootDir;
        private ILogger logger = LogService.Logger;
        //private readonly ILogger<RemoteControllerFeatureProvider> _logger;

        public RemoteControllerFeatureProvider(IWebHostEnvironment env)
        {
            rootDir = env.ContentRootPath;

            //_logger?.LogDebug("Init RemoteControllerFeatureProvider");

        }

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            //var remoteCode = new HttpClient().GetStringAsync("https://gist.githubusercontent.com/filipw/9311ce866edafde74cf539cbd01235c9/raw/6a500659a1c5d23d9cfce95d6b09da28e06c62da/types.txt").GetAwaiter().GetResult();
            //var remoteCode = new HttpClient().GetStringAsync("https://gist.githubusercontent.com/cllp/bea2a66419b8a89e3fb4a0282690ea75/raw/8b54c783658cf5c9c8c4a024dcddf0d8bcfca76b/test.txt").GetAwaiter().GetResult();


            //Read all directories
            var directories = Directory.GetDirectories(agentSettings.AppsDir);

            //declare startswith using directive
            string[] startswithArray = { "using", "//using" };

            foreach (var directory in directories)
            {
                //_logger?.LogInformation($"Init Apps Directory{directory}");


                //var path = @"/Users/smcho/filegen_from_directory/AIRPassthrough/";
                var dirName = new DirectoryInfo(directory).Name;

                var compileFileFullPath = directory + @"\" + dirName + ".compile";

                if (File.Exists(compileFileFullPath))
                    File.Delete(compileFileFullPath);

                var compileFile = File.Create(compileFileFullPath);

                TextWriter tw = new StreamWriter(compileFile);

                //string folderName = new DirectoryInfo(System.IO.Path.GetDirectoryName(directory)).Name;

                //get files in the directory
                var app_files = Directory.EnumerateFiles(directory, "*.cs", SearchOption.AllDirectories);

                var app_content = string.Empty;

                List<string> usingLine = new List<string>();
                List<string> contentLine = new List<string>();

                foreach (var f in app_files)
                {
                    // Open the file to read from.
                    var textLines = File.ReadAllLines(f).ToList();
                    for (int i = 0; i < textLines.Count(); i++)// string s in textLines.COu)
                    {
                        bool isUsingDirective = startswithArray.Any(textLines[i].ToLower().StartsWith);

                        //if (textLines[i].StartsWith("using", System.StringComparison.CurrentCultureIgnoreCase))
                        if (isUsingDirective)
                        {
                            //check for dublicate using
                            if (!usingLine.Contains(textLines[i]))
                                usingLine.Add(textLines[i]);
                        }
                        else
                        {
                            contentLine.Add(textLines[i]);
                        }
                    }
                }

                //tw.WriteLine(textLines[i]);
                foreach (var line in usingLine)
                {
                    tw.WriteLine(line);
                }

                //space between using and namespaces
                tw.WriteLine("");

                foreach (var line in contentLine)
                {
                    tw.WriteLine(line);
                }

                tw.Close();

                app_content = File.ReadAllText(compileFileFullPath);

                //COMPILE THE FILE START

                if (app_content != null)
                {
                    /*METADATA REFERENCES*/


                    //add default references
                    var references = new ReferenceAssemblies(ConfigurationManager.AppRootDirectory, "DefaultAppReferences.json");

                    //add specific references
                    var referencepath = directory; //CHECK IF OK //new FileInfo(f).Directory.FullName;
                    references.AddRange(new ReferenceAssemblies(referencepath, "references.json"));

                    //add hardcoded default references

                    references.Add(MetadataReference.CreateFromFile(typeof(Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo).Assembly.Location));
                    references.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));
                    references.Add(MetadataReference.CreateFromFile(typeof(RemoteControllerFeatureProvider).Assembly.Location));
                    references.Add(MetadataReference.CreateFromFile(typeof(ActionFramework.Action).Assembly.Location));
                    references.Add(MetadataReference.CreateFromFile(typeof(System.Linq.Enumerable).Assembly.Location));
                    references.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));
                    references.Add(MetadataReference.CreateFromFile(typeof(ExpandoObject).Assembly.Location));
                    references.Add(MetadataReference.CreateFromFile(typeof(Newtonsoft.Json.JsonConvert).Assembly.Location));
                    //references.Add(MetadataReference.CreateFromFile(typeof(RestSharp.Http).Assembly.Location));
                    references.Add(MetadataReference.CreateFromFile(typeof(System.Data.SqlClient.SqlConnection).Assembly.Location));

                    //references.Add(MetadataReference.CreateFromFile(typeof(Microsoft.WindowsAzure.Storage.Table.TableEntity).Assembly.Location));
                    
                    references.Add(MetadataReference.CreateFromFile(typeof(System.Diagnostics.Trace).Assembly.Location));

                    //references.AddRange(GetMetaDataReferences());

                    var op = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

                    //here is where we add all references
                    //referencesvar assrefs = GetMetaDataReferences();

                    //StartUpLogger.FileLog.Information($"Adding references for file {dirName}.", references);

                    //op = op.WithAssemblyIdentityComparer(DesktopAssemblyIdentityComparer.Default);

                    CSharpCompilation compilation = CSharpCompilation.Create(
                        dirName,
                        syntaxTrees: new[] { CSharpSyntaxTree.ParseText(app_content) },
                        references: references,// references: references,
                        options: op);

                    using (var ms = new MemoryStream())
                    {
                        var emitResult = compilation.Emit(ms);

                        if (!emitResult.Success)
                        {
                            foreach (var msg in emitResult.Diagnostics)
                            {
                                logger.Error($"RemoteController Build Error for app {dirName}. Message {msg.GetMessage()}");
                                //TODO: LOGGING Write to log here
                                //StartUpLogger.FileLog.Error(new System.Exception($"Compile error for App: '{dirName}'."), msg.GetMessage().ToString());
                            }
                            // handle, log errors etc

                            return; //TODO the debugger might want to succeed?
                            //but for now build anyway
                        }

                        ms.Seek(0, SeekOrigin.Begin);
                        var assembly = Assembly.Load(ms.ToArray());
                        //AssemblyBuilder builder = new AssemblyBuilder()

                        //add the assembly to Apps
                        AppContext.AddApp(assembly);

                        var candidates = assembly.GetExportedTypes();

                        foreach (var candidate in candidates)
                        {
                            var actionType = typeof(ActionFramework.Action);

                            //check if the candidate is an action
                            if (candidate.IsSubclassOf(actionType) && !candidate.IsAbstract && candidate.IsClass && candidate.IsSubclassOf(actionType))
                            {
                                feature.Controllers.Add(typeof(BaseController<>).MakeGenericType(candidate).GetTypeInfo());
                            }
                        }
                    }
                }
            }
        }

        private static IEnumerable<MetadataReference> GetMetaDataReferences()
        {
            return DependencyContext.Default
                .CompileLibraries
                .SelectMany(x => x.ResolveReferencePaths())
                .Where(path => !string.IsNullOrWhiteSpace(path))
                .Select(path => MetadataReference.CreateFromFile(path));
        }
    }
}
