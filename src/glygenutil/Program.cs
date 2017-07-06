using ChemSpider.Molecules;
using ChemSpider.Utilities;
using com.ggasoftware.indigo;
using CommandLine;
using CommandLine.Text;
using Elasticsearch.Net;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace glygenutil
{
	public class Options
	{
		[Option('c', "command", Required = false, HelpText = "Command to execute")]
		public string Command { get; set; }

		[Option('i', "in", Required = true, HelpText = "Input file")]
		public string InputFile { get; set; }

		[Option('o', "out", Required = false, HelpText = "Output file")]
		public string OutputFile { get; set; }

		[Option('d', "db", Required = false, HelpText = "Target database")]
		public string Database { get; set; }

		[Option("collection", Required = false, HelpText = "Target collection")]
		public string Collection { get; set; }

		[Option('h', "host", Required = false, HelpText = "Server host")]
		public string Host { get; set; }

		[Option('p', "port", Required = false, HelpText = "Server port")]
		public int Port { get; set; }

		[Option('u', "url", Required = false, HelpText = "Endpoint URL")]
		public string Url { get; set; }

		[Option('e', "element", Required = false, HelpText = "Document-defining XML element")]
		public string Element { get; set; }

		[HelpOption]
		public string GetUsage()
		{
			var help = HelpText.AutoBuild(this);
			return help.ToString();
		}
	}


	partial class Program
	{
		static object _lock = new object();

		static void Main(string[] args)
		{
			var opt = new Options();
			if ( !CommandLine.Parser.Default.ParseArgumentsStrict(args, opt) ) {
				Console.Error.WriteLine(opt.GetUsage());
				Environment.Exit(-1);
			}

			switch ( opt.Command ) {
				case "load-mongo":
					if ( Path.GetExtension(opt.InputFile).EqualsNoCase(".sdf") )
						loadSdfToMongo(opt.InputFile, opt.Url, opt.Database, opt.Collection ?? Path.GetFileNameWithoutExtension(opt.InputFile));
					else if ( Path.GetExtension(opt.InputFile).EqualsNoCase(".csv") )
						loadCsvToMongo(opt.InputFile, opt.Url, opt.Database, opt.Collection ?? Path.GetFileNameWithoutExtension(opt.InputFile));
					else if ( Path.GetExtension(opt.InputFile).EqualsNoCase(".xml") )
						loadXmlToMongo(opt.InputFile, opt.Url, opt.Database, opt.Collection ?? Path.GetFileNameWithoutExtension(opt.InputFile), opt.Element);
					else
						throw new NotSupportedException("Not supported file format");
					break;

				case "load-elastic":
					if ( Path.GetExtension(opt.InputFile).EqualsNoCase(".sdf") )
						loadSdfToElastic(opt.InputFile, opt.Url, opt.Database, opt.Collection ?? Path.GetFileNameWithoutExtension(opt.InputFile));
					else if ( Path.GetExtension(opt.InputFile).EqualsNoCase(".csv") )
						loadCsvToElastic(opt.InputFile, opt.Url, opt.Database, opt.Collection ?? Path.GetFileNameWithoutExtension(opt.InputFile));
					else if ( Path.GetExtension(opt.InputFile).EqualsNoCase(".xml") )
						loadXmlToElastic(opt.InputFile, opt.Url, opt.Database, opt.Collection ?? Path.GetFileNameWithoutExtension(opt.InputFile));
					else
						throw new NotSupportedException("Not supported file format");
					break;

				case "load-ttl-neo4j":
					loadTtlToNeo4j(opt.InputFile, opt.Url, opt.Database, opt.Collection ?? Path.GetFileNameWithoutExtension(opt.InputFile));
					break;

				case "convert-json":
					saveToJson(opt.InputFile, opt.OutputFile);
					break;

				case "create-bingo":
					createBingoDatabase(opt.InputFile, opt.OutputFile);
					break;

				default:
					Console.Error.WriteLine(opt.GetUsage());
					break;
			}
		}
	}
}
