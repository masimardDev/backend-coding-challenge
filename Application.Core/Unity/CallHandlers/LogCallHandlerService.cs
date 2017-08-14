using System;
using System.Diagnostics;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace Application.Core.Unity.CallHandlers
{
    internal class LogCallHandlerService : LogCallHandlerBase, ICallHandler
    {
        public int Order { get; set; }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            Type targetType = input.Target.GetType();
            string methodName = input.MethodBase.Name;

            //Pre-processing
            string inputs = GetInputParameters(input);
            Stopwatch watch = Stopwatch.StartNew();

            //Execute real method (or next call handler...)
            IMethodReturn methodReturn = getNext()(input, getNext);

            //Post-processing
            watch.Stop();
            string outputs = GetOutputParameters(input, methodReturn);
            object properties = new {Inputs = inputs, Outputs = outputs, Duration = watch.ElapsedMilliseconds};

            //if (methodReturn.Exception == null)
            //{
            //    Logger.InfoProperties(targetType, properties, "Operation {0} completed successfully", methodName);
            //}
            //else
            //{
            //    Logger.FatalProperties(targetType, properties, "Operation {0} failed : Exception \"{1}\" unhandled",
            //        methodName, methodReturn.Exception.GetType().FullName);
            //    Logger.Fatal(targetType, methodReturn.Exception);
            //}

            //Return result to the client (or previous call handler)
            return methodReturn;
        }
    }
}