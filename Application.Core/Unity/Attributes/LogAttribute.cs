using System;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace Application.Core.Unity.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue, Inherited = true)]
    internal class LogAttribute : Attribute
    {
        public string Expression { get; private set; }

        public LogAttribute(string expression = null)
        {
            Expression = expression;
        }

        internal void AppendValue(StringBuilder dest, object parameter)
        {
            if (String.IsNullOrEmpty(Expression))
            {
                dest.Append(parameter == null ? "null" : parameter.ToString());
                return;
            }

            string[] parts = Expression.Split(',').Select(p => p.Trim()).ToArray();

            dest.Append('[');
            foreach (string part in parts)
            {
                object value = DataBinder.Eval(parameter, part);
                dest.Append(value == null ? "null" : value.ToString());
                dest.Append(',');
            }

            if (dest.Length > 0)
            {
                dest.Length -= 1; //remove trailing ','
            }

            dest.Append(']');
        }
    }
}