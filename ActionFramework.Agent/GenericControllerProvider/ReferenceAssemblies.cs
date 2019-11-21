using ActionFramework.Configuration;
using ActionFramework.Logger;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Agent.GenericControllerProvider
{
    public class ReferenceAssemblies : List<MetadataReference>
    {
        private string filepath;
        private string filename;// = $"references.json";

        /// <summary>
        /// filepath to references.json
        /// </summary>
        /// <param name="filepath"></param>
        public ReferenceAssemblies(string filepath, string filename)
        {
            this.filepath = filepath;
            this.filename = filename;
            GetConfiguration();

            //TODO check how to oad more references from dotnet core C:\Program Files\dotnet\shared\Microsoft.NETCore.App\2.2.5\
            //Get directory from a reference?
        }

        private void GetConfiguration()
        {
            try
            {
                //find file even if there are root directory
                string jsonstring = File.ReadAllText(Path.Combine(filepath, filename));

                //check if json string is null or empty
                if (string.IsNullOrEmpty(jsonstring))
                    throw new Exception($"Could not find file '{filename}' recursively from dir '{filepath}'");

                //LogFactory.File.Information($"App reading references.json '{filepath}'");

                var jsonobj = JsonConvert.DeserializeObject<dynamic>(jsonstring);

                //load assemblies from directory
                var directoryGetFiles = jsonobj["Directory.GetFiles"];

                foreach (var dir in directoryGetFiles)
                {
                    string path;

                    if (!string.IsNullOrEmpty(dir.ToString()))
                    {
                        path = ConfigurationManager.AppRootDirectory + dir.ToString();
                    }
                    else
                    {
                        path = ConfigurationManager.AppRootDirectory;
                    }

                    if (Directory.Exists(path))
                    { 
                        var dllfiles = Directory.GetFiles(path, "*.dll", SearchOption.TopDirectoryOnly);

                        foreach (var file in dllfiles)
                        {
                            this.Add(MetadataReference.CreateFromFile(file));
                        }
                    }
                }

                //load references from string definition
                var assemblyLoad = jsonobj["Assembly.Load"];

                foreach (var refstring in assemblyLoad)
                {
                    this.Add(MetadataReference.CreateFromFile(Assembly.Load(refstring.ToString()).Location));
                }

                //load references from type
                var typeGetType = jsonobj["Type.GetType"];

                foreach (var typename in typeGetType)
                {
                    //this is how I should load it. Find out how to get the reference fully qualified name
                    //var reftype = Type.GetType("System.String, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
                    //this.Add(MetadataReference.CreateFromFile(reftype.Assembly.Location));
                }

                //references.AddRange(Directory.GetFiles(rootDir, "*.dll").Select(f => MetadataReference.CreateFromFile(f)));
            }
            catch (Exception ex)
            {
                //LogFactory.File.Error(ex, $"Reading ReferenceAssemblies error");
                throw ex;
            }
        }
    }
}
