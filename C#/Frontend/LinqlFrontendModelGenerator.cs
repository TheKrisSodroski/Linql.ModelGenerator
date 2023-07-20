using System;
using System.Collections.Generic;
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

        public async Task Generate(string IntermediaryJson, string ProjectPath = "")
        {
            using (Stream stream = this.GenerateStreamFromString(IntermediaryJson))
            {
                IntermediaryModule module = await JsonSerializer.DeserializeAsync<IntermediaryModule>(stream);
            }
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
    }
}
