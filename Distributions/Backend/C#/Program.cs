using Linql.ComponentModel.DataAnnotations;
using Linql.ModelGenerator.Core;
using Linql.ModelGenerator.CSharp.Backend;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

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
            generator.OverridePlugins.Add(new LinqlDataAnnotationsIgnore());
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