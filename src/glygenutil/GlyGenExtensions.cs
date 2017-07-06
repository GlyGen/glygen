using ChemSpider.Molecules;
using com.ggasoftware.indigo;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace glygenutil
{
	public static class GlyGenExtensions
	{
		public static double? MolecularWeight(this IMolecule molecule)
		{
			try {
				using ( Indigo indigo = new Indigo() ) {
					var mol = indigo.loadMolecule(molecule.Mol);
					return mol.molecularWeight();
				}
			}
			catch ( IndigoException ) {
				return null;
			}
		}

		public static double? MonoisotopicMass(this IMolecule molecule)
		{
			try {
				using ( Indigo indigo = new Indigo() ) {
					var mol = indigo.loadMolecule(molecule.Mol);
					return mol.monoisotopicMass();
				}
			}
			catch ( IndigoException ) {
				return null;
			}
		}

		public static string MolecularFormula(this IMolecule molecule)
		{
			try {
				using ( Indigo indigo = new Indigo() ) {
					var mol = indigo.loadMolecule(molecule.Mol);
					return mol.grossFormula();
				}
			}
			catch ( IndigoException ) {
				return null;
			}
		}

		public static BsonDocument ToBsonDocument(this SdfRecord r)
		{
			double? mw = r.Molecule.MolecularWeight();
			double? mm = r.Molecule.MonoisotopicMass();
			string mf = r.Molecule.MolecularFormula();

			var doc = new BsonDocument {
				{ "__SMILES", String.IsNullOrEmpty(r.Molecule.SMILES) ? BsonNull.Value : BsonValue.Create(r.Molecule.SMILES) },
				{ "__InChI", String.IsNullOrEmpty(r.Molecule.InChI) ? BsonNull.Value : BsonValue.Create(r.Molecule.InChI) },
				{ "__InChIKey", String.IsNullOrEmpty(r.Molecule.InChIKey) ? BsonNull.Value : BsonValue.Create(r.Molecule.InChIKey) },
				{ "__MOL", String.IsNullOrEmpty(r.Mol) ? BsonNull.Value : BsonValue.Create(r.Mol) },
				{ "__MW", mw == null ? BsonNull.Value : BsonValue.Create(mw) },
				{ "__MM", mm == null ? BsonNull.Value : BsonValue.Create(mm) },
				{ "__MF", String.IsNullOrEmpty(mf) ? BsonNull.Value : BsonValue.Create(mf) },
			};

			doc.AddRange(r.Properties.ToDictionary(p => p.Key, p => {
				string s = String.Join("\n", p.Value);
				if ( String.IsNullOrEmpty(s) )
					return BsonNull.Value;

				int i;
				if ( int.TryParse(s, out i) )
					return BsonValue.Create(i);

				double f;
				if ( double.TryParse(s, out f) )
					return BsonValue.Create(f);

				return BsonValue.Create(s);
			}));

			return doc;
		}
	}
}
