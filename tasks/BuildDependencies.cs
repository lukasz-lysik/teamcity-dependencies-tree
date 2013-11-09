using System;
using System.Collections.Generic;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.Build.Evaluation;
using System.Xml.Linq;
using System.IO;

namespace ProjectsDependenciesTree
{
    public class BuildDependencies : Task
    {
		public string InputFolder { get; set; }
		public string OutputFile { get; set; }

		public override bool Execute()
		{
			var references = new XElement("Projects");

			foreach (var file in Directory.GetFiles(InputFolder, "*.csproj", SearchOption.AllDirectories))
			{
				references.Add(ParseProject(file));
			}

			references.Save(OutputFile);

			return true;
		}

		private XElement ParseProject(string projectFilePath)
		{
			var project = new Project(projectFilePath);
			var assemblyName = project.GetProperty("AssemblyName");

			var output = new XElement("Project");
			output.Add(new XAttribute("name", assemblyName.EvaluatedValue));
			output.Add(new XAttribute("path", projectFilePath));

			foreach (var item in project.GetItems("Reference"))
			{
				if (item.EvaluatedInclude.StartsWith("System") ||
					item.EvaluatedInclude.StartsWith("Microsoft"))
					continue;

				output.Add(new XElement("Reference", new XAttribute("name", item.EvaluatedInclude)));
			}
			
			return output;
		}
	}
}
