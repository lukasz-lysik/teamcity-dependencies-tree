using System;
using System.Collections.Generic;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.Build.Evaluation;
using System.Xml.Linq;
using System.IO;
using System.Text;

namespace ProjectsDependenciesTree
{
	public class DrawDependencies : Task
	{
		public string InputFile { get; set; }
		public string OutputFile { get; set; }

		public override bool Execute()
		{
			var dependenciesXml = XDocument.Load(InputFile);

			var outputDot = new StringBuilder();

			outputDot.AppendLine("digraph dependencies {");
			outputDot.AppendLine("\tnode [style=\"rounded,filled\", shape=box, color=lightblue2];");

			foreach (var project in dependenciesXml.Descendants("Project"))
			{
				var name = project.Attribute("name").Value;

				foreach (var dependency in project.Descendants("Reference"))
				{
					outputDot.AppendLine(string.Format("\t\"{0}\" -> \"{1}\";", name, dependency.Attribute("name").Value));
				}
			}

			outputDot.AppendLine("}");

			using (var file = new StreamWriter(OutputFile))
			{
				file.WriteLine(outputDot.ToString());
			}

			return true;
		}
	}
}
