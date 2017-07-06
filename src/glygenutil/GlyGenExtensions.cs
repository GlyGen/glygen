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
		public static double MolecularWeight(this IMolecule molecule)
		{
			using ( Indigo indigo = new Indigo() ) {
				var mol = indigo.loadMolecule(molecule.Mol);
				return mol.molecularWeight();
			}
		}

		public static double MonoisotopicMass(this IMolecule molecule)
		{
			using ( Indigo indigo = new Indigo() ) {
				var mol = indigo.loadMolecule(molecule.Mol);
				return mol.monoisotopicMass();
			}
		}

		public static string MolecularFormula(this IMolecule molecule)
		{
			using ( Indigo indigo = new Indigo() ) {
				var mol = indigo.loadMolecule(molecule.Mol);
				return mol.grossFormula();
			}
		}

		public static BsonDocument ToBsonDocument(this SdfRecord r)
		{
			var doc = new BsonDocument {
								{ "__SMILES", String.IsNullOrEmpty(r.Molecule.SMILES) ? BsonNull.Value : BsonValue.Create(r.Molecule.SMILES) },
								{ "__InChI", String.IsNullOrEmpty(r.Molecule.InChI) ? BsonNull.Value : BsonValue.Create(r.Molecule.InChI) },
								{ "__InChIKey", String.IsNullOrEmpty(r.Molecule.InChIKey) ? BsonNull.Value : BsonValue.Create(r.Molecule.InChIKey) },
								{ "__MOL", String.IsNullOrEmpty(r.Mol) ? BsonNull.Value : BsonValue.Create(r.Mol) },
								{ "__MW", BsonValue.Create(r.Molecule.MolecularWeight()) },
								{ "__MM", BsonValue.Create(r.Molecule.MonoisotopicMass()) },
								{ "__MF", BsonValue.Create(r.Molecule.MolecularFormula()) },
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
