using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestMarrow;

namespace FedoroffSoft.TestMarrow.UnitTest.SimpleConfigTest
{
	[TestClass]
	public class SimpleConfigTest
	{
		[TestMethod]
		public void TestSimpleConfig()
		{
			String str = @">| Name   | Value | Key | CreatedOn  | 
							| Marrow | 13.07 | 27  | 2018-03-15 |";

			var parser = new MarrowParser();

			parser.ConfigureClass<Struct1>()
				.ConfigureProperty(s => s.Key);

			var struct1 = parser.Parse<Struct1>(str);

			Assert.AreEqual("Marrow", struct1.Name, "Name parsing failed");
			Assert.AreEqual(13.07, struct1.Value, "Value parsing failed");
			Assert.AreEqual(27, struct1.Key, "Key parsing failed");
			Assert.AreEqual(new DateTime(2018,03,15), struct1.CreatedOn, "CreatedOn parsing failed");
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
