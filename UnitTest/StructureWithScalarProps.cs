using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestMarrow;
using System.Collections.Generic;

namespace FedoroffSoft.TestMarrow.UnitTest.StructureWithScalarProps
{
	[TestClass]
	public class StructureWithScalarProps
	{
		[TestMethod]
		public void SimpleObject()
		{
			String str = @">| Name   | Value | Key | CreatedOn  | 
							| Marrow | 13.07 | 27  | 2018-03-15 |";

			var parser = new MarrowParser();
			var struct1 = parser.Parse<Struct1>(str);

			Assert.AreEqual("Marrow", struct1.Name, "Name parsing failed");
			Assert.AreEqual(13.07, struct1.Value, "Value parsing failed");
			Assert.AreEqual(27, struct1.Key, "Key parsing failed");
			Assert.AreEqual(new DateTime(2018,03,15), struct1.CreatedOn, "CreatedOn parsing failed");
		}

		[TestMethod]
		public void SimpleList()
		{
			String str = @">| Name          | Value| Key | CreatedOn |
							| Ma rrow       | 13.07| 0   | 2017-03-15|
							| Marrow 1      | 13   | -123| 2018-03-15|
							| Marrow one two| -16  | 56  | 2019-03-15| ";

			var parser = new MarrowParser();
			var struct1 = parser.Parse<List<Struct1>>(str);

			Assert.AreEqual("Ma rrow"					, struct1[0].Name		, "Name parsing failed for line 0");
			Assert.AreEqual(13.07						, struct1[0].Value		, "Value parsing failed for line 0");
			Assert.AreEqual(0							, struct1[0].Key		, "Key parsing failed for line 0");
			Assert.AreEqual(new DateTime(2017,03,15)	, struct1[0].CreatedOn	, "CreatedOn parsing failed for line 0");

			Assert.AreEqual("Marrow 1"					, struct1[1].Name		, "Name parsing failed for line 1");
			Assert.AreEqual(13							, struct1[1].Value		, "Value parsing failed for line 1");
			Assert.AreEqual(-123						, struct1[1].Key		, "Key parsing failed for line 1");
			Assert.AreEqual(new DateTime(2018,03,15)	, struct1[1].CreatedOn	, "CreatedOn parsing failed for line 1");

			Assert.AreEqual("Marrow one two"			, struct1[2].Name		, "Name parsing failed for line 2");
			Assert.AreEqual(-16							, struct1[2].Value		, "Value parsing failed for line 2");
			Assert.AreEqual(56							, struct1[2].Key		, "Key parsing failed for line 2");
			Assert.AreEqual(new DateTime(2019,03,15)	, struct1[2].CreatedOn	, "CreatedOn parsing failed for line 2");
		}
	}

	public class Struct1
	{
		public string Name { get; set; }
		public Double Value { get; set; }
		public int Key { get; set; }
		public DateTime CreatedOn { get; set; }
	}
}
