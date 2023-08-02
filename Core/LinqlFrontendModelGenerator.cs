using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Linql.ModelGenerator.Core
{
    public abstract class LinqlFrontendModelGenerator
    {
        public string CoreJson { get; set; }

        public string ProjectPath { get; set; }

        public CoreModule Module { get; set; }

        protected HashSet<CoreType> ImportCache = new HashSet<CoreType>();

        protected abstract void CreateProject();

        protected abstract void CreateType(CoreType Type);

        protected abstract Dictionary<string, string> ExtractAdditionalModules(CoreModule Module);
        protected abstract void AddAdditionalModules(Dictionary<string, string> AdditionalModules);

        public LinqlFrontendModelGenerator(string CoreJson, string ProjectPath = null)
        {
            this.CoreJson = CoreJson;
            if (ProjectPath == null)
            {
                this.ProjectPath = Environment.CurrentDirectory;
            }
            else
            {
                this.ProjectPath = ProjectPath;
            }
            this.Module = JsonSerializer.Deserialize<CoreModule>(this.CoreJson);
        }

        public LinqlFrontendModelGenerator(CoreModule Module, string ProjectPath = null)
        {
            if (ProjectPath == null)
            {
                this.ProjectPath = Environment.CurrentDirectory;
            }
            else
            {
                this.ProjectPath = ProjectPath;
            }
            this.Module = Module;
        }

        public virtual void Generate()
        {
            this.CreateProject();

            Dictionary<string, string> additionalModules = this.ExtractAdditionalModules(this.Module);

            additionalModules.Remove(this.Module.ModuleName);

            this.AddAdditionalModules(additionalModules);

            this.Module.Types.ForEach(r =>
            {
                this.CreateType(r);
            });
        }

        public virtual void Clean()
        {
            Directory.Delete(Path.Combine(this.ProjectPath, this.Module.ModuleName), true);
        }
    }
}
