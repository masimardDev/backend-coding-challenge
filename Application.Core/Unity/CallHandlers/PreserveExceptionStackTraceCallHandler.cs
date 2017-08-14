using System.Runtime.ExceptionServices;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace Application.Core.Unity.CallHandlers
{
    internal class PreserveExceptionStackTraceCallHandler : ICallHandler
    {
        public int Order { get; set; }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            //Execute real method (or next call handler...)
            IMethodReturn methodReturn = getNext()(input, getNext);

            //Post-processing
            if (methodReturn.Exception != null)
                ExceptionDispatchInfo.Capture(methodReturn.Exception).Throw();

            //Return result to the client (or previous call handler)
            return methodReturn;
        }
    }
}