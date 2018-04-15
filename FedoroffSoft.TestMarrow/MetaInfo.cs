using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestMarrow
{
	/// <summary>
	/// Stores info about a parced meta line. For example such line: 
	/// >| strcutProp1	| Name	    | Value | 
	/// 
	/// </summary>
	internal class MetaInfo
	{
		/// <summary>
		/// The name of the property. It's used when we analys the type of a property that belongs to a class
		/// </summary>
		internal  String PropName { get; set; }

		/// <summary>
		/// Indicates that the property is a collection
		/// </summary>
		internal  bool IsCollection { get; set; }
		
		/// <summary>
		/// The type of the property we analyse. If the propery is a collection then here's the type of the collection item.
		/// </summary>
		internal  Type PropType { get; set; }

		/// <summary>
		/// The properties names extracted from the header lines.
		/// </summary>
		internal  String[] SubPropNames { get; set; }

		internal  PropertyInfo[] SubPropInfo { get; set; }

		internal Boolean FirstLine { get; set; } = false;
	}
}
