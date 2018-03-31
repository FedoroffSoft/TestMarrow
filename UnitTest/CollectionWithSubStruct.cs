using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestMarrow;
using System.Collections.Generic;

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
						   >| strcutProp1	| Name2	    | Value2 | 
							|				| SubMarrow | 0		 |
							| Marrow 3		| -12		|
					";

			var parser = new MarrowParser();
			var struct1 = parser.Parse<List<Struct1>>(str);

			Assert.AreEqual("Marrow", struct1[0].Name1, "Name parsing failed");
			Assert.AreEqual(13.07, struct1[0].Value1, "Value parsing failed");
			Assert.AreEqual("SubMarrow", struct1[0].strcutProp1.Name2, "strcutProp1.Value parsing failed");
			Assert.AreEqual(0, struct1[0].strcutProp1.Value2, "strcutProp1.Value parsing failed");

			Assert.AreEqual("Marrow 3", struct1[1].Name1, "Name parsing failed");
			Assert.AreEqual(-12, struct1[1].Value1, "Value parsing failed");
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
