﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TestMarrow.Configuration;

namespace TestMarrow
{
    public class MarrowParser
    {
		private readonly Dictionary<String, Dictionary<String, PropertyConfig>> config;
		public String NullValue { get; set; } = "NULL";
		public String EmptyStringValue { get; set; } = @"""";

		public MarrowParser()
		{
			config = new Dictionary<String, Dictionary<String, PropertyConfig>>();
		}

		public T Parse<T>(String str) where T: class, new()
		{
			T result = null;

			ParsingContext context = new ParsingContext();

			//Let's convert the source string into array of lines to iterate through them
			context.SourceLines = new List<String>();
			String line = null;
			using (StringReader sr = new StringReader(str))
				while( (line = sr.ReadLine()) != null)
					context.SourceLines.Add(line);

			//The first line is alway a meta data for the first leve class
			
			MetaInfo parentInfo = CheckCollectionClass(typeof(T), new MetaInfo());
			parentInfo.FirstLine = true;
			var metaInfo = ParseMetaLine(parentInfo, context.SourceLines[0]);

			//Now we start the recursive processing of the lines
			//Let's process the rest of the lines
			context.StructLevel = 0;
			context.LineIndex = 1;
			context.MetaInfo = metaInfo;
			result = ParseLevel<T>(context);

			return result;
		}

		/// <summary>
		/// The methos parces the data for one level. It uses recursion to parse sub-levels
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="level">The current level in the test structures</param>
		/// <param name="sourceLines">The string array that contains the source lines of the data</param>
		/// <param name="lineIndex">The zero-based index of the currently processing line in the source lines</param>
		/// <param name="metaInfo">The meta info about the class the current line must be parsed to</param>
		/// <returns></returns>
		internal T ParseLevel<T>(ParsingContext context) where T: class, new()
		{
			//Let's check if the T class is onle of supported collections
			T result = new T();
			Type listType = null;

			//The IEnumerable classes are supported.
			if (context.MetaInfo.IsCollection)
			{
				//we use the List type of generic enumerable collections
				Type listTypeBase = typeof(List<>);
				Type[] typeArgs = { context.MetaInfo.PropType };
				listType = listTypeBase.MakeGenericType(typeArgs);

				result = (T)Activator.CreateInstance(listType);
			}

			
			Boolean isPropLine = false;

			//the object to work with a single item;
			Object item = null;

			while(context.LineIndex < context.SourceLines.Count())
			{
				string line = context.SourceLines[context.LineIndex];

				if (String.IsNullOrWhiteSpace(line))
				{
					context.LineIndex++;
					continue;
				}

				line = line.Trim();

				//Let's check if the current line is a start for a complex property that contains the properties
				//such lines must start with the '>' sign
				isPropLine = line.Substring(0,1) == ">";

				//We encounter with the sub-structure - one of the class propery is not a scalar one
				if(isPropLine)
				{
					var subMetaInfo = ParseMetaLine(context.MetaInfo, line);

					//Let's use the recursion to parse it.

					var curLevelMetaInfo = context.MetaInfo;
					var curLevelStructLevel = context.StructLevel;


					MethodInfo method = ((TypeInfo)(this.GetType())).DeclaredMethods.First(t => t.Name == "ParseLevel");
					MethodInfo generic = null;
					if(subMetaInfo.IsCollection)
					{
						var genericList = typeof(List<>).MakeGenericType(subMetaInfo.PropType);
						generic = method.MakeGenericMethod(genericList);
					}
					else
					{
						generic = method.MakeGenericMethod(subMetaInfo.PropType);
					}


					//Create context for the call
					context.LineIndex++;
					context.StructLevel++;
					context.MetaInfo = subMetaInfo;
					var subResult = generic.Invoke(this,  new [] { context });

					//Restore context after the call
					context.StructLevel = curLevelStructLevel;
					context.MetaInfo = curLevelMetaInfo;

					//let's assing the result to the item object
					PropertyInfo prop = context.MetaInfo.SubPropInfo.First(p => p.Name.Equals(subMetaInfo.PropName));
					prop.SetValue(item, subResult);
				}
				else
				{
					//Let's check that the current level from the function argument matches the current level in the data line
					String[] rawValues = line.Split('|').Select(s => s.Trim()).ToArray();
					//we have to discard the data before the first '|' character and data after the last one
					rawValues = rawValues.Skip(1).Take(rawValues.Count() - 2).ToArray();
					int lineLevel = 0;
					for ( ; lineLevel < rawValues.Count(); lineLevel++)
						if (!String.IsNullOrWhiteSpace(rawValues[lineLevel]))
							break;

					//This line belongs to the upper level class. We don't have to process it
					if (lineLevel < context.StructLevel)
						break;

					
					//Les't create and object instance to work with for every new data line
					item = Activator.CreateInstance(context.MetaInfo.PropType);

					//We need data for the level specified in the argument
					var strValues = rawValues.Skip(context.StructLevel).Take(rawValues.Count() - context.StructLevel).ToArray();

					for (int i = 0; i < strValues.Count(); i++)
					{
						if (!context.MetaInfo.SubPropInfo.Any(p => p.Name.Equals(context.MetaInfo.SubPropNames[i])))
							throw new ArgumentException($"The property '{context.MetaInfo.SubPropNames[i]}' is absent in the '{context.MetaInfo.PropType.Name}' class");

						PropertyInfo prop = context.MetaInfo.SubPropInfo.First(p => p.Name.Equals(context.MetaInfo.SubPropNames[i]));

						//Standart type conversion
						Object value = null;
						if (prop.PropertyType.Name == "String" && strValues[i].Equals(EmptyStringValue, StringComparison.InvariantCulture))
							value = String.Empty;
						else if (strValues[i].Equals(NullValue, StringComparison.InvariantCulture))
							value = null;
						else
							value = Convert.ChangeType(strValues[i], prop.PropertyType);

						//Let's update the property with the value.
						prop.SetValue(item, value);
					}

					if (context.MetaInfo.IsCollection)
					{
						//we know that the result is a List and has the Add method
						MethodInfo addMethod = listType.GetMethod("Add");
						object magicValue = addMethod.Invoke(result, new[] { item });
					}

					//Line is processed
					context.LineIndex++;
					
				}
			}

			//for a non-collection type we return the object item.
			if( !context.MetaInfo.IsCollection )
				result = (T)item;

			return result;
		}

		/// <summary>
		/// parces the infor about property line. For example, such lines
		/// >| strcutProp1	| Name	    | Value | 
		/// </summary>
		/// <typeparam name="T">The type of the parent class</typeparam>
		/// <param name="level">Level in the string data</param>
		/// <param name="line">raw text of the line</param>
		/// <returns></returns>
		private MetaInfo ParseMetaLine(MetaInfo parentInfo, String line)
		{
			var result = new MetaInfo();

			var rawProps = line
							.Replace(">", String.Empty)
							.Split('|')
							.Where(s => ! String.IsNullOrWhiteSpace(s))//ignore empty names on the left and right sides of the source string. Note, it's assumend that a property name can't be empty!
							.Select(s => s.Trim());

			//For the first level, the prop type was already parsed
			if (parentInfo.FirstLine)
			{
				result.PropType = parentInfo.PropType;
				result.IsCollection = parentInfo.IsCollection;

				//The names of the properties specified in the test data
				result.SubPropNames = rawProps.ToArray();
			}
			else
			{
				//For the non-first levels, we have to extract data form the parent class and the line

				//the name of the complex property
				result.PropName = rawProps.First();

				PropertyInfo prop = parentInfo.PropType.GetProperties().FirstOrDefault(p => p.Name.Equals(result.PropName));

				if(prop == null)
						throw new ArgumentException($"The property {result.PropName} is absent in the {result.PropType.Name} class");

				result = CheckCollectionClass(prop.PropertyType, result);

				//The names of the properties specified in the test data
				//we skip the property name in the parent class
				result.SubPropNames = rawProps.Skip(1).ToArray();
			}

			//lets check that we will be able to create instances of the item type
			if (result.PropType.GetConstructor(Type.EmptyTypes) == null)
				throw new ArgumentException($"The generic type '{result.PropType.Name}' has to have a parameterless constructor.");
			
			result.SubPropInfo = result.PropType.GetProperties();

			return result;
		}

		private MetaInfo CheckCollectionClass(Type type, MetaInfo meta)
		{
			meta.IsCollection = typeof(IEnumerable).IsAssignableFrom(type);

			//If the type is a collection then we need the item type
			if (meta.IsCollection)
			{
				//actually we need the collection item type.
				//We support the collections of only one generic type
				Type[] genericTypes = type.GetGenericArguments();
				if (genericTypes.Count() != 1)
					throw new ArgumentException("currently TestMarrow supports collection of only one generic type");

				meta.PropType = genericTypes[0];
			}
			else
				meta.PropType = type;

			return meta;
		}


		public  ClassConfig<T> ConfigureClass<T>() where T: class
		{
			String className = typeof(T).Name;

			if (!config.Keys.Contains(className))
				config.Add(className, new Dictionary<string, PropertyConfig>());

			return new ClassConfig<T>(config[className]);
		}

		
    }
}
