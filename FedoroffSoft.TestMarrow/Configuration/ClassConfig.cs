using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestMarrow.Configuration
{
	public class ClassConfig<T>
	{
		private Dictionary<String, PropertyConfig> classConfig;
		private String curProperty;

		internal ClassConfig(Dictionary<String, PropertyConfig> classConfig)
		{
			this.classConfig = classConfig;
		}

		public ClassConfig<T> ConfigureProperty(Expression<Func<T, object>> expr)
		{
			MemberExpression memberExpression = null;
			if (expr.NodeType == ExpressionType.Lambda)
			{
				var body = (UnaryExpression) expr.Body;
				memberExpression = body.Operand as MemberExpression;
				curProperty = memberExpression.Member.Name;
			}
			else
				throw new ArgumentException("Property is expected", "expr");

			return this;
		}
	}
}
