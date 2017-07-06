using ChemSpider.Molecules;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace glygenutil
{
	partial class Program
	{
		private static void loadSdfToMongo(string inputFile, string url, string database, string collection)
		{
			MongoUrl u = new MongoUrl(url ?? ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
			MongoClient client = new MongoClient(u);
			var db = client.GetDatabase(database ?? u.DatabaseName ?? ConfigurationManager.AppSettings["DefaultDatabase"]);
			var col = db.GetCollection<BsonDocument>(collection ?? ConfigurationManager.AppSettings["DefaultCollection"] ?? Path.GetFileNameWithoutExtension(inputFile));

			int counter = 0;
			ConcurrentBag<BsonDocument> docs = new ConcurrentBag<BsonDocument>();

			using ( SdfReader reader = new SdfReader(inputFile) ) {
				Console.Out.WriteLine("Converting using {0} threads", Environment.ProcessorCount);

				reader
					.Records
					.AsParallel()
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

		private static void loadCsvToMongo(string inputFile, string url, string database, string collection)
		{
			MongoUrl u = new MongoUrl(url ?? ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
			MongoClient client = new MongoClient(u);
			var db = client.GetDatabase(database ?? u.DatabaseName ?? ConfigurationManager.AppSettings["DefaultDatabase"]);
			var col = db.GetCollection<BsonDocument>(collection ?? ConfigurationManager.AppSettings["DefaultCollection"] ?? Path.GetFileNameWithoutExtension(inputFile));

			int counter = 0;
			ConcurrentBag<BsonDocument> docs = new ConcurrentBag<BsonDocument>();

			using ( SdfReader reader = new SdfReader(inputFile) ) {
				Console.Out.WriteLine("Converting using {0} threads", Environment.ProcessorCount);

				reader
					.Records
					.AsParallel()
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

		private static void loadXmlToMongo(string inputFile, string url, string database, string collection)
		{
			MongoUrl u = new MongoUrl(url ?? ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
			MongoClient client = new MongoClient(u);
			var db = client.GetDatabase(database ?? u.DatabaseName ?? ConfigurationManager.AppSettings["DefaultDatabase"]);
			var col = db.GetCollection<BsonDocument>(collection ?? ConfigurationManager.AppSettings["DefaultCollection"] ?? Path.GetFileNameWithoutExtension(inputFile));

			int counter = 0;
			ConcurrentBag<BsonDocument> docs = new ConcurrentBag<BsonDocument>();

			using ( SdfReader reader = new SdfReader(inputFile) ) {
				Console.Out.WriteLine("Converting using {0} threads", Environment.ProcessorCount);

				reader
					.Records
					.AsParallel()
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
	}
}
