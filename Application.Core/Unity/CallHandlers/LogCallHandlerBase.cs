using System.Linq;
using System.Reflection;
using System.Text;
using Application.Core.Unity.Attributes;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace Application.Core.Unity.CallHandlers
{
    internal abstract class LogCallHandlerBase
    {
        protected internal string GetInputParameters(IMethodInvocation method)
        {
            var builder = new StringBuilder();

            //Input parameters
            AppendParameters(builder, method, false);

            return builder.ToString();
        }

        protected internal string GetOutputParameters(IMethodInvocation method, IMethodReturn methodReturn)
        {
            var builder = new StringBuilder();
            var methodInfo = method.MethodBase as MethodInfo;

            //Out parameters
            AppendParameters(builder, method, true);

            //Return value
            if (methodInfo != null)
            {
                var attribute =
                    (LogAttribute)
                        methodInfo.ReturnTypeCustomAttributes.GetCustomAttributes(typeof (LogAttribute), true)
                            .FirstOrDefault();
                if (attribute != null)
                {
                    if (builder.Length > 0)
                    {
                        builder.Append(", ");
                    }
                    builder.Append("return");
                    builder.Append("=");
                    attribute.AppendValue(builder, methodReturn.ReturnValue);
                }
            }

            return builder.ToString();
        }

        private void AppendParameters(StringBuilder builder, IMethodInvocation method, bool outputs)
        {
            foreach (ParameterInfo param in method.MethodBase.GetParameters().Where(p => p.IsOut == outputs))
            {
                var attribute = param.GetCustomAttribute<LogAttribute>();
                if (attribute != null)
                {
                    builder.Append(param.Name);
                    builder.Append("=");
                    attribute.AppendValue(builder, method.Arguments[param.Name]);
                    builder.Append(", ");
                }
            }

            //remove trailing ", "
            if (builder.Length > 0)
            {
                builder.Length -= 2;
            }
        }
    }
}