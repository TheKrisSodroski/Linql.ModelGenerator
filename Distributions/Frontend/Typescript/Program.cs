using Linql.ModelGenerator.Core;
using Linql.ModelGenerator.Typescript.Frontend;
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
            throw new Exception("Invalid arguments supplied to Program");
        }
    }
}