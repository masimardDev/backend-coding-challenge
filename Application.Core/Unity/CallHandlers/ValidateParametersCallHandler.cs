using System;
using System.Reflection;
using Application.Core.Unity.Attributes;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace Application.Core.Unity.CallHandlers
{
    internal class ValidateParametersCallHandler : ICallHandler
    {
        public int Order { get; set; }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            //Pre-processing
            foreach (ParameterInfo param in input.MethodBase.GetParameters())
            {
                var attribute = param.GetCustomAttribute<ValidateAttribute>();
                if (attribute != null)
                {
                    object value = input.Arguments[param.Name];
                    Type type = param.ParameterType;

                    if (type == typeof (sbyte) || type == typeof (byte)
                        || type == typeof (short) || type == typeof (ushort)
                        || type == typeof (int) || type == typeof (uint)
                        || type == typeof (long) || type == typeof (ulong)
                        || type == typeof (float) || type == typeof (double)
                        || type == typeof (decimal))
                    {
                        long numericValue = Convert.ToInt64(value);

                        if (numericValue < attribute.MinimumNumericValue)
                        {
                            return
                                input.CreateExceptionMethodReturn(
                                    new ArgumentException(
                                        "Value minimum numerical value is " + attribute.MinimumNumericValue, param.Name));
                        }
                        if (numericValue > attribute.MaxiumumNumericValue)
                        {
                            return
                                input.CreateExceptionMethodReturn(
                                    new ArgumentException(
                                        "Value maximum numerical value is " + attribute.MaxiumumNumericValue, param.Name));
                        }
                    }
                    else
                    {
                        var stringValue = value as string;

                        //Check empty values
                        switch (attribute.Disallow)
                        {
                            case Disallow.None:
                                break;
                            case Disallow.Null:
                                if (value == null)
                                {
                                    return
                                        input.CreateExceptionMethodReturn(
                                            new ArgumentNullException("Value can't be null", param.Name));
                                }
                                break;
                            case Disallow.Empty:
                                if (stringValue != null && stringValue.Length == 0)
                                {
                                    return
                                        input.CreateExceptionMethodReturn(new ArgumentException("Value can't be empty",
                                            param.Name));
                                }
                                break;
                            case Disallow.NullOrEmpty:
                                if (string.IsNullOrEmpty(stringValue))
                                {
                                    return
                                        input.CreateExceptionMethodReturn(
                                            new ArgumentException("Value can't be null or empty", param.Name));
                                }
                                break;
                            case Disallow.Whitespaces:
                                if (stringValue != null && stringValue.Trim().Length == 0)
                                {
                                    return
                                        input.CreateExceptionMethodReturn(
                                            new ArgumentException("Value can't be empty or whitespaces", param.Name));
                                }
                                break;
                            case Disallow.NullOrWhitespaces:
                                if (string.IsNullOrWhiteSpace(stringValue))
                                {
                                    return
                                        input.CreateExceptionMethodReturn(
                                            new ArgumentException("Value can't be null, empty or whitespaces",
                                                param.Name));
                                }
                                break;
                            default:
                                break;
                        }

                        //Check string length if applicable
                        if (attribute.MaxLength > 0 && stringValue != null && stringValue.Length > attribute.MaxLength)
                        {
                            return
                                input.CreateExceptionMethodReturn(
                                    new ArgumentException("Value can't exceed " + attribute.MaxLength + " chars",
                                        param.Name));
                        }
                    }
                }
            }

            //Execute real method (or next call handler...)
            IMethodReturn methodReturn = getNext()(input, getNext);

            //Post-processing

            //Return result to the client (or previous call handler)
            return methodReturn;
        }
    }
}