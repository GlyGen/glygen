using ChemSpider.Molecules;
using ChemSpider.Utilities;
using com.ggasoftware.indigo;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace glygenutil
{
	partial class Program
	{
		private static Bingo openOrCreateBingoDb(Indigo indigo, string location, bool optimize = false)
		{
			Bingo bingo = null;
			if ( !Directory.Exists(location) )
				bingo = Bingo.createDatabaseFile(indigo, location, "molecule", "");
			else {
				bingo = Bingo.loadDatabaseFile(indigo, location);
				if ( optimize )
					bingo.optimize();
			}
			return bingo;
		}

		private static void createBingoDatabase(string inputFile, string outputFile)
		{
			int nRecords = 0;
			using ( SdfReader sdf = new SdfReader(inputFile) ) {
				string dir = outputFile;
				if ( String.IsNullOrEmpty(dir) )
					dir = Path.GetFileNameWithoutExtension(inputFile);

				Console.Out.WriteLine("Writing Bingo NoSQL database to: {0}", dir);

				using ( Indigo indigo = new Indigo() )
				using ( var db = openOrCreateBingoDb(indigo, dir) ) {
					sdf.Records
						.ForAll(r => {
							string smiles = r.Molecule.SMILES;
							if ( !String.IsNullOrEmpty(smiles) ) {
								try {
									IndigoObject mol = indigo.loadMolecule(smiles);
									db.insert(mol);
								}
								catch ( IndigoException ex ) {
									Trace.TraceWarning(ex.Message);
								}
								catch ( BingoException ex ) {
									Trace.TraceWarning(ex.Message);
								}
							}

							if ( ++nRecords % 1000 == 0 )
								Console.Out.Write("{0} ", nRecords);
						});
					db.close();
				}
			}
		}
	}
}
