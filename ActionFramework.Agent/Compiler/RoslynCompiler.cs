using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

    /// <summary>
    /// Compiler that uses the Roslyn compiler (<see cref="CSharpCompilation" />).
    /// </summary>
    /// <seealso cref="KeesTalksTech.Utilities.Compilation.ICompiler" />
    public class RoslynCompiler
    {
        /// <summary>
        /// Gets the compilation options.
        /// </summary>
        /// <value>
        /// The options.
        /// </value>
        public CSharpCompilationOptions Options { get; } = new CSharpCompilationOptions(
            OutputKind.DynamicallyLinkedLibrary,
            reportSuppressedDiagnostics: true,
            optimizationLevel: OptimizationLevel.Release,
            generalDiagnosticOption: ReportDiagnostic.Error
        );

        /// <summary>
        /// Compiles the specified code the sepcified assembly locations.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="assemblyLocations">The assembly locations.</param>
        /// <returns>
        /// The assembly.
        /// </returns>
        /// <exception cref="KeesTalksTech.Utilities.Compilation.Roslyn.RoslynCompilationException">Assembly could not be created.</exception>
        public Assembly Compile(string code, params string[] assemblyLocations)
        {
            var references = assemblyLocations.Select(l => MetadataReference.CreateFromFile(l));

            var compilation = CSharpCompilation.Create(
                "_" + Guid.NewGuid().ToString("D"),
                references: references,
                syntaxTrees: new SyntaxTree[] { CSharpSyntaxTree.ParseText(code) },
                options: this.Options
            );

            using (var ms = new MemoryStream())
            {
                var compilationResult = compilation.Emit(ms);

                if (compilationResult.Success)
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    return Assembly.Load(ms.ToArray());
                }

                throw new RoslynCompilationException("Assembly could not be created.", compilationResult);
            }
        }
    }

    /// <summary>
    /// Object that stores the compilation exception for the Roslyn compiler.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class RoslynCompilationException : Exception
    {
        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        public EmitResult Result { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoslynCompilationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="result">The result.</param>
        public RoslynCompilationException(string message, EmitResult result) : base(message)
        {
            this.Result = result;
        }
    }