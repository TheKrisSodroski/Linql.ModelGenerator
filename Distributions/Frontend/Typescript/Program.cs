using Linql.ModelGenerator.Core;
using Linql.ModelGenerator.Typescript.Frontend;
using System.Reflection;
using System.Text.Json;

class Program
{
    public static void Main(string[] args)
    {
        string firstArg = args.FirstOrDefault();

        if(firstArg != null)
        {
            string json = File.ReadAllText(firstArg);
            CoreModule module = JsonSerializer.Deserialize<CoreModule>(json);
            LinqlModelGeneratorTypescriptFrontend generator = new LinqlModelGeneratorTypescriptFrontend(module);
            generator.Generate();
        }
        else
        {
            string currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            List<string> linqlModels = Directory.GetFiles(currentDirectory, "*.linqlmodel.json", SearchOption.AllDirectories).ToList();

            Console.WriteLine("Found the following linql models:");
            linqlModels.ForEach(r => Console.WriteLine(r));

            linqlModels.ForEach(r =>
            {
                Console.WriteLine($"Generating from file {r}");
                string json = File.ReadAllText(r);
                CoreModule module = JsonSerializer.Deserialize<CoreModule>(json);
                LinqlModelGeneratorTypescriptFrontend generator = new LinqlModelGeneratorTypescriptFrontend(module);
                generator.Generate();
            });
        }
    }
}