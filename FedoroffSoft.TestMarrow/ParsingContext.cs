using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMarrow
{
	/// <summary>
	/// This an auxiliary class to store the current data during the recursive parsing of a string
	/// </summary>
	internal class ParsingContext
	{
		internal List<String> SourceLines { get; set; }

		internal int LineIndex { get; set; }

		internal int StructLevel { get; set; }

		internal MetaInfo MetaInfo { get; set; }
	}
}
