using System;
using System.Runtime.Remoting.Messaging;
using System.ServiceModel;
using System.Web;
using Microsoft.Practices.Unity;

namespace Application.Core.Unity.LifetimeManager
{
    /// <summary>
    ///     This is a custom lifetime that preserve  instance on the same
    ///     execution environment. For example, in  a WCF request or ASP.NET request, diferent
    ///     call to resolve method return the same instance
    /// </summary>
    internal class PerContextLifetimeManager : SynchronizedLifetimeManager
    {
        public enum AspNetStorage
        {
            Request,
            Session,
            Application
        }

        #region Nested

        /// <summary>
        ///     Custom extension for OperationContext scope
        /// </summary>
        private class ContainerExtension : IExtension<OperationContext>
        {
            #region Members

            /// <summary>
            ///     Value
            /// </summary>
            public object Value { get; set; }

            #endregion

            #region IExtension<OperationContext> Members

            /// <summary>
            ///     Attach
            /// </summary>
            /// <param name="owner">Operation context</param>
            public void Attach(OperationContext owner)
            {
            }

            /// <summary>
            ///     Detach
            /// </summary>
            /// <param name="owner">Operation context</param>
            public void Detach(OperationContext owner)
            {
            }

            #endregion
        }

        #endregion

        #region Members

        private readonly string _key;
        private readonly AspNetStorage _aspNetStorage;

        #endregion

        #region Constructor

        /// <summary>
        ///     Default constructor (using ASP.NET Request Storage)
        /// </summary>
        public PerContextLifetimeManager()
            : this(AspNetStorage.Request, Guid.NewGuid().ToString())
        {
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="aspNetStorage">Storage when in ASP.NET Context</param>
        /// <param name="key">A key for this lifetimemanager resolver</param>
        public PerContextLifetimeManager(AspNetStorage aspNetStorage, string key)
        {
            if (String.IsNullOrEmpty(key))
                throw new ArgumentException("Key cannot be empty");

            _key = key;
            _aspNetStorage = aspNetStorage;
        }

        #endregion

        /// <summary>
        ///     <see cref="M:Microsoft.Practices.Unity.LifetimeManager.RemoveValue" />
        /// </summary>
        public override void RemoveValue()
        {
            if (OperationContext.Current != null)
            {
                //WCF without HttpContext environment
                var containerExtension = OperationContext.Current.Extensions.Find<ContainerExtension>();
                if (containerExtension != null)
                    OperationContext.Current.Extensions.Remove(containerExtension);
            }
            else if (HttpContext.Current != null)
            {
                //HttpContext avaiable ( ASP.NET ..)
                switch (_aspNetStorage)
                {
                    case AspNetStorage.Request:
                        if (HttpContext.Current.Items[_key] != null)
                        {
                            HttpContext.Current.Items.Remove(_key);
                        }
                        break;
                    case AspNetStorage.Session:
                        if (HttpContext.Current.Session != null && HttpContext.Current.Session[_key] != null)
                        {
                            HttpContext.Current.Session.Remove(_key);
                        }
                        break;
                    case AspNetStorage.Application:
                        if (HttpContext.Current.Application[_key] != null)
                        {
                            HttpContext.Current.Application.Remove(_key);
                        }
                        break;
                }
            }
            else
            {
                //Not in WCF or ASP.NET Environment, UnitTesting, WinForms, WPF etc.
                CallContext.FreeNamedDataSlot(_key);
            }
        }

        protected override object SynchronizedGetValue()
        {
            object result = null;

            //Get object depending on  execution environment ( WCF without HttpContext,HttpContext or CallContext)

            if (OperationContext.Current != null)
            {
                //WCF without HttpContext environment
                var containerExtension = OperationContext.Current.Extensions.Find<ContainerExtension>();
                if (containerExtension != null)
                {
                    result = containerExtension.Value;
                }
            }
            else if (HttpContext.Current != null)
            {
                //HttpContext available ( ASP.NET ..)
                switch (_aspNetStorage)
                {
                    case AspNetStorage.Request:
                        if (HttpContext.Current.Items[_key] != null)
                        {
                            result = HttpContext.Current.Items[_key];
                        }
                        break;
                    case AspNetStorage.Session:
                        if (HttpContext.Current.Session != null && HttpContext.Current.Session[_key] != null)
                        {
                            result = HttpContext.Current.Session[_key];
                        }
                        break;
                    case AspNetStorage.Application:
                        HttpContext.Current.Application.Lock();
                        if (HttpContext.Current.Application[_key] != null)
                        {
                            result = HttpContext.Current.Application[_key];
                        }
                        HttpContext.Current.Application.UnLock();
                        break;
                }
            }
            else
            {
                //Not in WCF or ASP.NET Environment, UnitTesting, WinForms, WPF etc.
                if (AppDomain.CurrentDomain.IsFullyTrusted)
                {
                    //ensure that we're in full trust
                    result = CallContext.GetData(_key);
                }
            }


            return result;
        }

        protected override void SynchronizedSetValue(object newValue)
        {
            if (OperationContext.Current != null)
            {
                //WCF without HttpContext environment
                var containerExtension = OperationContext.Current.Extensions.Find<ContainerExtension>();
                if (containerExtension == null)
                {
                    containerExtension = new ContainerExtension
                    {
                        Value = newValue
                    };

                    OperationContext.Current.Extensions.Add(containerExtension);
                }
            }
            else if (HttpContext.Current != null)
            {
                //HttpContext avaiable ( ASP.NET ..)
                switch (_aspNetStorage)
                {
                    case AspNetStorage.Request:
                        if (HttpContext.Current.Items[_key] == null)
                        {
                            HttpContext.Current.Items[_key] = newValue;
                        }
                        break;
                    case AspNetStorage.Session:
                        if (HttpContext.Current.Session != null && HttpContext.Current.Session[_key] == null)
                        {
                            HttpContext.Current.Session[_key] = newValue;
                        }
                        break;
                    case AspNetStorage.Application:
                        HttpContext.Current.Application.Lock();
                        if (HttpContext.Current.Application[_key] == null)
                        {
                            HttpContext.Current.Application[_key] = newValue;
                        }
                        HttpContext.Current.Application.UnLock();
                        break;
                }
            }
            else
            {
                //Not in WCF or ASP.NET Environment, UnitTesting, WinForms, WPF etc.
                if (AppDomain.CurrentDomain.IsFullyTrusted)
                {
                    //ensure that we're in full trust
                    CallContext.SetData(_key, newValue);
                }
            }
        }
    }
}