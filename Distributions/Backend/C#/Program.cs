using Linql.ComponentModel.Annotations;
using Linql.ModelGenerator.Core;
using Linql.ModelGenerator.CSharp.Backend;
using Linql.System.Spatial;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Emit;
using System.Text.Json;

class Program
{
    public static void Main(string[] args)
    {
        string firstArg = args.FirstOrDefault();

        if(firstArg != null)
        {
            LinqlModelGeneratorCSharpBackend generator = null;

            if (firstArg.ToLower().Contains("system.componentmodel.annotations"))
            {
                 generator = new LinqlModelGeneratorCSharpBackend(typeof(KeyAttribute).Assembly);
                
            }
            else
            {
                generator = new LinqlModelGeneratorCSharpBackend(firstArg);
            }
            generator.OverridePlugins.Add(new LinqlAnnotationsModuleOverride());
            generator.OverridePlugins.Add(new LinqlSpatialModuleOverride());
            CoreModule module = generator.Generate();
            string moduleJson = JsonSerializer.Serialize(module);
            string currentDirectory = Path.GetDirectoryName(System.AppContext.BaseDirectory);
            string file = Path.Combine(currentDirectory, $"{module.ModuleName}.linqlmodel.json");
            File.WriteAllText(file, moduleJson);
        }
        else
        {
            throw new Exception("Invalid arguments supplied to Program");
        }
    }
}