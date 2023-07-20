using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Linql.ModelGenerator.Intermediary;

namespace Linql.ModelGenerator.Frontend
{
    public class LinqlFrontendModelGenerator
    {
        public LinqlFrontendModelGenerator() { }

        public async Task Generate(string IntermediaryJson, string ProjectPath = null)
        {
            if(ProjectPath == null)
            {
                ProjectPath = Environment.CurrentDirectory;
            }
            IntermediaryModule module = null;

            using (Stream stream = this.GenerateStreamFromString(IntermediaryJson))
            {
                module = await JsonSerializer.DeserializeAsync<IntermediaryModule>(stream);
            }

            this.CreateProject(module, ProjectPath);

            module.Types.ForEach(r =>
            {
                this.CreateType(module, ProjectPath, r);
            });
        }

        protected void CreateProject(IntermediaryModule Module, string ProjectPath)
        {
            Process process = new Process();
            ProcessStartInfo processStartInfo = new ProcessStartInfo("dotnet");
            processStartInfo.Arguments = $"new classlib -o {Module.ModuleName}";
            processStartInfo.WorkingDirectory = ProjectPath;
            processStartInfo.CreateNoWindow = false;
            process.StartInfo = processStartInfo;
            process.Start();

            File.Delete(Path.Combine(ProjectPath, Module.ModuleName, "Class1.cs"));
        }

        private Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        protected void CreateType(IntermediaryModule Module, string ProjectPath, IntermediaryType Type)
        {
            string fileText = "";
            string directory = Path.Combine(ProjectPath, Module.ModuleName, Type.NameSpace.Replace($"{Module.ModuleName}.", String.Empty));
            Directory.CreateDirectory(directory);
            File.WriteAllText(Path.Combine(directory, $"{Type.TypeName}.cs"), fileText);
        }
    }
}
