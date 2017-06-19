using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace ActiveXGen
{
    class Program
    {
        static void Main(string[] args)
        {
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(@"
                using System;
                using System.Collections.Generic;
                using System.Text;
                using System.Windows.Forms;
                using StrataRunView;

                namespace ActiveXTest
                {
                 class Program
                    {
                        static void Main(string[] args)
                        {
                             // Use ur code here to generate an exe
                        }
                     }
               }
            ");

            //string assemblyName = Path.GetRandomFileName();
            string assemblyName = "ActiveXOutput.exe";
            MetadataReference[] references = new MetadataReference[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
                MetadataReference.CreateFromFile("C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\System.Windows.Forms.dll")
            };

            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.WindowsApplication));

            using (var ms = new MemoryStream())
            {                
                EmitResult result1 = compilation.Emit(@"C:\Tyndora-Desktop\TIM\ActiveX\SampleDLL\" + assemblyName);
                EmitResult result = compilation.Emit(ms);

                if (!result.Success)
                {
                    IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic => 
                        diagnostic.IsWarningAsError || 
                        diagnostic.Severity == DiagnosticSeverity.Error);

                    foreach (Diagnostic diagnostic in failures)
                    {
                        Console.Error.WriteLine("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                    }
                }
                else
                {
                    //ms.Seek(0, SeekOrigin.Begin);
                    //Assembly assembly = Assembly.Load(ms.ToArray());

                    //Type type = assembly.GetType("RoslynCompileSample.Writer");
                    //object obj = Activator.CreateInstance(type);
                    //type.InvokeMember("Write",
                    //    BindingFlags.Default | BindingFlags.InvokeMethod,
                    //    null,
                    //    obj,
                    //    new object[] { "Hello World" });
                }
            }

            Console.ReadLine();
        }
    }
}