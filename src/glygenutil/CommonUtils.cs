using ChemSpider.Molecules;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace glygenutil
{
	partial class Program
	{
		private static void saveToJson(string inputFile, string outputFile)
		{
			int counter = 0;
			using ( SdfReader reader = new SdfReader(inputFile) )
			using ( StreamWriter writer = new StreamWriter(outputFile) ) {
				Console.Out.WriteLine("Converting using {0} threads", Environment.ProcessorCount);
				reader
					.Records
					.AsParallel()
					.WithDegreeOfParallelism(Environment.ProcessorCount)
					.ForAll(r => {
						if ( r.Molecule != null ) {
							var doc = r.ToBsonDocument();
							lock ( writer ) {
								writer.Write(doc.ToJson());

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
