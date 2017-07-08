using ChemSpider.Molecules;
using ChemSpider.Utilities;
using CsvHelper;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace glygenutil
{
	partial class Program
	{
		private static void loadSdfToMongo(Options opt)
		{
			MongoUrl u = new MongoUrl(opt.Url ?? ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
			MongoClient client = new MongoClient(u);
			var db = client.GetDatabase(opt.Database ?? u.DatabaseName ?? ConfigurationManager.AppSettings["DefaultDatabase"]);
			var col = db.GetCollection<BsonDocument>(opt.Collection ?? ConfigurationManager.AppSettings["DefaultCollection"] ?? Path.GetFileNameWithoutExtension(opt.InputFile));

			int counter = 0;
			ConcurrentBag<BsonDocument> docs = new ConcurrentBag<BsonDocument>();

			using ( SdfReader reader = new SdfReader(opt.InputFile) ) {
				Console.Out.WriteLine("Converting using {0} threads", Environment.ProcessorCount);

				reader
					.Records
#if !DEBUG
					.AsParallel()
#endif
					.ForAll(r => {
						if ( r.Molecule != null ) {
							docs.Add(r.ToBsonDocument());
							lock ( _lock ) {
								counter++;

								if ( docs.Count == 1000 ) {
									col.InsertMany(docs);
									docs = new ConcurrentBag<BsonDocument>();

									Console.Out.Write(".");
									if ( counter % 10000 == 0 )
										Console.Out.Write(counter);
								}
							}
						}
					});

				if ( docs.Count > 0 )
					col.InsertMany(docs);

				Console.Out.WriteLine("{0} records inserted", counter);
			}
		}

		private static void loadCsvToMongo(Options opt)
		{
			MongoUrl u = new MongoUrl(opt.Url ?? ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
			MongoClient client = new MongoClient(u);
			var db = client.GetDatabase(opt.Database ?? u.DatabaseName ?? ConfigurationManager.AppSettings["DefaultDatabase"]);
			var col = db.GetCollection<BsonDocument>(opt.Collection ?? ConfigurationManager.AppSettings["DefaultCollection"] ?? Path.GetFileNameWithoutExtension(opt.InputFile));

			int counter = 0;
			ConcurrentBag<BsonDocument> docs = new ConcurrentBag<BsonDocument>();

			using ( var tr = new StreamReader(opt.InputFile) )
			using ( var cr = new CsvReader(tr, new CsvHelper.Configuration.CsvConfiguration { HasHeaderRecord = true }) ) {
				cr.Read();
				while ( cr.Read() ) {
					var bson = new BsonDocument();
					foreach ( var h in cr.FieldHeaders ) {
						bson.Add(h, cr.GetField<string>(h));
					}

					docs.Add(bson);

					lock ( _lock ) {
						counter++;

						if ( docs.Count == 1000 ) {
							col.InsertMany(docs);
							docs = new ConcurrentBag<BsonDocument>();

							Console.Out.Write(".");
							if ( counter % 10000 == 0 )
								Console.Out.Write(counter);
						}
					}
				}

				if ( docs.Count > 0 )
					col.InsertMany(docs);

				Console.Out.WriteLine("{0} records inserted", counter);
			}
		}

		private static void loadXmlToMongo(Options opt)
		{
			MongoUrl u = new MongoUrl(opt.Url ?? ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
			MongoClient client = new MongoClient(u);
			var db = client.GetDatabase(opt.Database ?? u.DatabaseName ?? ConfigurationManager.AppSettings["DefaultDatabase"]);
			var col = db.GetCollection<BsonDocument>(opt.Collection ?? ConfigurationManager.AppSettings["DefaultCollection"] ?? Path.GetFileNameWithoutExtension(opt.InputFile));

			XDocument xdoc = XDocument.Load(opt.InputFile);
			for ( int i = 0; i < opt.Repeat; i++ )
			{
				int counter = 0;
				ConcurrentBag<BsonDocument> docs = new ConcurrentBag<BsonDocument>();
				using ( var b = new Benchmark("load-xml-mongo") ) {
					xdoc
						.XPathSelectElements(opt.Element)
						.AsParallel()
						.WithDegreeOfParallelism(opt.Threads)
						.ForAll(x => {
							if ( !String.IsNullOrWhiteSpace(x.Value) ) {
								string json = JsonConvert.SerializeXNode(x);
								BsonDocument bson = BsonDocument.Parse(json);
								docs.Add(bson);
								lock ( _lock ) {
									counter++;

									if ( docs.Count == 1000 ) {
										col.InsertMany(docs);
										docs = new ConcurrentBag<BsonDocument>();

										Console.Out.Write(".");
										if ( counter % 10000 == 0 )
											Console.Out.Write(counter);
									}
								}
							}
						});

						if ( docs.Count > 0 )
							col.InsertMany(docs);

				}

				Console.Out.Write("{0} records inserted", counter);
			}
		}
	}
}
