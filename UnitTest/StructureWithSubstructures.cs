using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestMarrow;

namespace FedoroffSoft.TestMarrow.UnitTest.StructWithSubstruct
{
	/// <summary>
	/// Here we test the parsing of a structire that besides the scalar properties has other structures but not collections
	/// </summary>
	[TestClass]
	public class StructureWithSubstructures
	{
		[TestMethod]
		public void SimpleObjectWithSubObjects()
		{
			String str = @">| Name1			| Value1	| 
							| Marrow		| 13.07		|
						   >| strcutProp1	| Name2	    | Value2 | 
							|				| SubMarrow | 0		 |
					";

			var parser = new MarrowParser();
			var struct1 = parser.Parse<Struct1>(str);

			Assert.AreEqual("Marrow", struct1.Name1, "Name parsing failed");
			Assert.AreEqual(13.07, struct1.Value1, "Value parsing failed");
			Assert.AreEqual("SubMarrow", struct1.strcutProp1.Name2, "strcutProp1.Value parsing failed");
			Assert.AreEqual(0, struct1.strcutProp1.Value2, "strcutProp1.Value parsing failed");
		}
	}

	public class Struct1
	{
		public string Name1 { get; set; }
		public Double Value1 { get; set; }
		public Struct2 strcutProp1 { get; set; }
	}

	public class Struct2
	{
		public string Name2 { get; set; }
		public Double Value2 { get; set; }
	}
}
