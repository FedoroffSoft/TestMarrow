using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestMarrow;
using System.Collections.Generic;
using KellermanSoftware.CompareNetObjects;

namespace FedoroffSoft.TestMarrow.UnitTest.CollectionWithSubStruct
{
	[TestClass]
	public class CollectionWithSubStruct
	{
		[TestMethod]
		public void OneLineTestData()
		{
			String str = @">| Name1			| Value1	| 
							| Marrow		| 13.07		|
						   >| StrcutProp1	| Name2	    | Value2 | 
							|				| SubMarrow | 0		 |
							| Marrow 3		| -12		|
					";

			var parser = new MarrowParser();
			var actual = parser.Parse<List<Struct1>>(str);

			var expected = new List<Struct1> {
				new Struct1 { Name1 = "Marrow", Value1 = 13.07,
					StrcutProp1 = new Struct2 { Name2 = "SubMarrow", Value2 = 0 }
				},
				new Struct1 { Name1 = "Marrow 3", Value1 = -12 }
			};

			CompareLogic compareLogic = new CompareLogic();
			ComparisonResult result = compareLogic.Compare(actual, expected);
			if (!result.AreEqual)
				throw new Exception( result.DifferencesString );

		}

		[TestMethod]
		public void MultyLineSubPropData()
		{
			String str = @">| Name1				| Value1		| 
							| Marrow			| 13.07			|
						   >| EnumStrcutProp1	| Name2			| Value2 | 
							|					| SubMarrow1	| 4		 |
							|					| SubMarrow2	| 5		 |
							| NULL				| -12			|
							| ""				| -.43			|
					";

			var parser = new MarrowParser();
			var actual = parser.Parse<List<Struct1>>(str);

			var expected = new List<Struct1> {
				new Struct1 { Name1 = "Marrow", Value1 = 13.07,
					EnumStrcutProp1 = new List<Struct2> {
						new Struct2 {Name2 = "SubMarrow1", Value2 = 4 },
						new Struct2 {Name2 = "SubMarrow2", Value2 = 5 },
					}
				},
				new Struct1 { Value1 = -12 },
				new Struct1 { Name1 = String.Empty, Value1 = -0.43 }
			};


			CompareLogic compareLogic = new CompareLogic();
			compareLogic.Config.IgnoreCollectionOrder = true;

			ComparisonResult result = compareLogic.Compare(actual, expected);

			if (!result.AreEqual)
				throw new Exception( result.DifferencesString );

		}
	}

		public class Struct1
	{
		public string Name1 { get; set; }
		public Double Value1 { get; set; }
		public Struct2 StrcutProp1 { get; set; }

		public IEnumerable<Struct2> EnumStrcutProp1 { get; set; }
	}

	public class Struct2
	{
		public string Name2 { get; set; }
		public Double Value2 { get; set; }
	}
}
