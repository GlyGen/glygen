using ChemSpider.Molecules;
using Elasticsearch.Net;
using MongoDB.Bson;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace glygenutil
{
	partial class Program
	{
		private static void loadSdfToElastic(string inputFile, string url, string index, string type)
		{
			var local = new Uri(url ?? "http://localhost:9200");
			var settings = new ConnectionConfiguration(local);
			var elastic = new ElasticLowLevelClient(settings);

			int counter = 0;
			ConcurrentBag<BsonDocument> docs = new ConcurrentBag<BsonDocument>();

			using ( SdfReader reader = new SdfReader(inputFile) ) {
				Console.Out.WriteLine("Converting using {0} threads", Environment.ProcessorCount);

				reader
					.Records
					.AsParallel()
					.WithDegreeOfParallelism(Environment.ProcessorCount)
					.ForAll(r => {
						if ( r.Molecule != null ) {
							docs.Add(r.ToBsonDocument());
							lock ( _lock ) {
								if ( docs.Count == 100 ) {
									foreach ( var d in docs ) {
										var res = elastic.Index<object>(index, type, d["ID"].AsString, new PostData<object>(d.ToJson()), null);
										// Console.Out.WriteLine(res);
									}
									docs = new ConcurrentBag<BsonDocument>();
								}

								if ( ++counter % 1000 == 0 )
									Console.Out.Write(counter);
								else if ( counter % 100 == 0 )
									Console.Out.Write(".");
							}
						}
					});
			}
		}

		private static void loadCsvToElastic(string inputFile, string url, string index, string type)
		{
			var local = new Uri(url ?? "http://localhost:9200");
			var settings = new ConnectionConfiguration(local);
			var elastic = new ElasticLowLevelClient(settings);

			int counter = 0;
			ConcurrentBag<BsonDocument> docs = new ConcurrentBag<BsonDocument>();

			using ( SdfReader reader = new SdfReader(inputFile) ) {
				Console.Out.WriteLine("Converting using {0} threads", Environment.ProcessorCount);

				reader
					.Records
					.AsParallel()
					.WithDegreeOfParallelism(Environment.ProcessorCount)
					.ForAll(r => {
						if ( r.Molecule != null ) {
							docs.Add(r.ToBsonDocument());
							lock ( _lock ) {
								if ( docs.Count == 100 ) {
									foreach ( var d in docs ) {
										var res = elastic.Index<object>(index, type, d["ID"].AsString, new PostData<object>(d.ToJson()), null);
										// Console.Out.WriteLine(res);
									}
									docs = new ConcurrentBag<BsonDocument>();
								}

								if ( ++counter % 1000 == 0 )
									Console.Out.Write(counter);
								else if ( counter % 100 == 0 )
									Console.Out.Write(".");
							}
						}
					});
			}
		}

		private static void loadXmlToElastic(string inputFile, string url, string index, string type)
		{
			var local = new Uri(url ?? "http://localhost:9200");
			var settings = new ConnectionConfiguration(local);
			var elastic = new ElasticLowLevelClient(settings);

			int counter = 0;
			ConcurrentBag<BsonDocument> docs = new ConcurrentBag<BsonDocument>();

			using ( SdfReader reader = new SdfReader(inputFile) ) {
				Console.Out.WriteLine("Converting using {0} threads", Environment.ProcessorCount);

				reader
					.Records
					.AsParallel()
					.WithDegreeOfParallelism(Environment.ProcessorCount)
					.ForAll(r => {
						if ( r.Molecule != null ) {
							docs.Add(r.ToBsonDocument());
							lock ( _lock ) {
								if ( docs.Count == 100 ) {
									foreach ( var d in docs ) {
										var res = elastic.Index<object>(index, type, d["ID"].AsString, new PostData<object>(d.ToJson()), null);
										// Console.Out.WriteLine(res);
									}
									docs = new ConcurrentBag<BsonDocument>();
								}

								if ( ++counter % 1000 == 0 )
									Console.Out.Write(counter);
								else if ( counter % 100 == 0 )
									Console.Out.Write(".");
							}
						}
					});
			}
		}
	}
}
