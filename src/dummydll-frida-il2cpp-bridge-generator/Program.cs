using System.Text;
using Mono.Cecil;

namespace dummydll_frida_il2cpp_bridge_generator;

class Program
{
    private const string ParamAssemblyName = "--assembly=";
    private const string ParamTypeScriptOutputDir = "--tsoutput=";
    private const string ParamAnnotation = "--annotation";
    private const string ParamHelp = "--help";
    private const string ParamJsDoc = "--jsdoc";

    public static string? AssemblyName { get; set; }
    public static string? TypeScriptOutputDir { get; set; }
    public static bool Annotation { get; set; }
    public static bool JsDoc { get; set; }


    [STAThread]
    public static void Main(string[] args)
    {
        if (args.Length != 0)
        {
            var parameters = new Dictionary<string, Action<string>>
            {
                {
                    ParamHelp, _ =>
                    {
                        PrintHelp();
                        Environment.Exit(0);
                    }
                },
                { ParamAssemblyName, value => { AssemblyName = value; } },
                { ParamTypeScriptOutputDir, value => { TypeScriptOutputDir = value; } },
                { ParamAnnotation, _ => { Annotation = true; } },
                { ParamJsDoc, _ => { JsDoc = true; } }
            };

            foreach (var arg in args)
            {
                var matched = false;
                foreach (var param in parameters)
                {
                    if (arg.StartsWith(param.Key))
                    {
                        var value = arg.Substring(param.Key.Length);

                        param.Value(value);
                        matched = true;
                    }
                }

                if (!matched)
                {
                    Console.WriteLine($"Unrecognized option {arg}; use --help for usage information.");
                    Environment.Exit(1);
                }
            }

            HandleAssembly(AssemblyName, true);
        }
        else
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Choose a dummy dll (Il2cppDumper) assembly file to load",
                Filter = "DummyDll (*.dll)|*.dll"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string assemblyFile = openFileDialog.FileName;
                HandleAssembly(assemblyFile);
            }
            else
            {
                Environment.Exit(0);
            }
        }
    }

    public static void PrintHelp()
    {
        string format = "{0,-30} {1}";
        Console.WriteLine("Usage: dummydll-frida-il2cpp-bridge-generator [options]");
        Console.WriteLine("Options:");
        Console.WriteLine(format, "--assembly=<file path>",
            "[Required] Path to the dummy dll (Il2cppDumper) assembly file to load.");
        Console.WriteLine(format, "--tsoutput=<file path>",
            "[Required] Path to the directory where the generated TypeScript files will be saved.");
        Console.WriteLine(format, "--annotation", "Generate TypeScript annotations for the generated classes.");
        Console.WriteLine(format, "--jsdoc", "Generate JSDoc comments for the generated classes.");
        Console.WriteLine(format, "--help", "Print this help message.");
    }

    public static void HandleAssembly(string assemblyFile, bool commandLine = false)
    {
        if (commandLine && string.IsNullOrEmpty(assemblyFile))
        {
            Console.WriteLine(
                "The path to the dummy dll file to load is invalid, use --help for usage information.");
            Environment.Exit(1);
        }

        if (commandLine && string.IsNullOrEmpty(TypeScriptOutputDir))
        {
            Console.WriteLine(
                "The path to the directory where the generated TypeScript files will be saved is invalid, use --help for usage information.");
            Environment.Exit(1);
        }

        try
        {
            var assembly = AssemblyDefinition.ReadAssembly(assemblyFile);

            Console.WriteLine("Loading assembly...");
            Console.WriteLine($"Assembly {assembly.FullName} loaded successfully");
            Console.WriteLine("Classes in the assembly: " + assembly.MainModule.Types.Count);
            Console.WriteLine("Methods in the assembly: " +
                              assembly.MainModule.Types.SelectMany(t => t.Methods).Count());
            Console.WriteLine("Fields in the assembly: " +
                              assembly.MainModule.Types.SelectMany(t => t.Fields).Count());
            Console.WriteLine("Properties in the assembly: " +
                              assembly.MainModule.Types.SelectMany(t => t.Properties).Count());
            Console.WriteLine("Events in the assembly: " +
                              assembly.MainModule.Types.SelectMany(t => t.Events).Count());
            Console.WriteLine("Custom attributes in the assembly: " +
                              assembly.MainModule.Types.SelectMany(t => t.CustomAttributes).Count());

            Console.WriteLine("\nNow generating...");

            StringBuilder tsScript;
            using (new TimingCookie("Generating frida-il2cpp-bridge classes"))
            {
                tsScript = AssemblyToFridaIl2CppBridgeClasses.Generate(assembly, Annotation, JsDoc);
            }

            using (new TimingCookie("Writing frida-il2cpp-bridge classes to file"))
            {
                if (commandLine == false)
                {
                    File.WriteAllText(assembly.MainModule.Name + ".ts", tsScript.ToString());
                }
                else
                {
                    File.WriteAllText(Path.Combine(TypeScriptOutputDir), tsScript.ToString());
                }
            }

            Console.WriteLine("Done!");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }
}