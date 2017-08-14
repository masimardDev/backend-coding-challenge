using System;
using System.Collections.Concurrent;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Web;
using Application.Configuration;
using Application.Core.Shared.Services;
using Application.Core.Unity;
using Application.Core.Unity.CallHandlers;
using Application.Core.Unity.LifetimeManager;
using Application.Core.Unity.MatchingRules;
using Application.DAL;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace Application.Core
{
    public class CoreContext
    {
        private static IUnityContainer _unityContainer;

        public static CoreContext Instance
        {
            get
            {
                if (_unityContainer == null)
                {
                    _unityContainer = ConfigureUnityContainer();
                }
                return _unityContainer.Resolve<CoreContext>();
            }
        }

        #region Configure Unity Container

        private static UnityContainer ConfigureUnityContainer()
        {
            var container = new UnityContainer();
            container.AddNewExtension<Interception>();

            ConfigureServices(container);


            //Register External componenents
            //ConfigureExternalComponents(container);

            //Register Entity Framework Contexts
            container.RegisterType<ApplicationEntities>(new PerContextLifetimeManager(),
                new InjectionConstructor(
                    CoreConfiguration.Instance.InternalEntityConnectionString
                    )
                );


            //Register the CoreContext class
            container.RegisterType<CoreContext>(new ContainerControlledLifetimeManager());

            return container;
        }

        private static void ConfigureServices(UnityContainer container)
        {
            //Create interception policies
            //ServicesPolicy: Matching rules should only match methods defined in the interfaces 
            //                of the services that are in the current assembly

            PolicyDefinition servicesPolicy =
                container.Configure<Interception>().AddPolicy(UnityConstants.ServicesPolicyName);
            servicesPolicy.AddMatchingRule(new TypeInheritanceMatchingRule(typeof (IServiceBase)));
            servicesPolicy.AddMatchingRule<MemberNameMatchingRule>(new InjectionConstructor("*", true));
            //HACK: Allows to preserve Exception Call Stack when interception is used
            servicesPolicy.AddCallHandler<PreserveExceptionStackTraceCallHandler>();
            servicesPolicy.AddCallHandler<LogCallHandlerService>();
            servicesPolicy.AddCallHandler<ValidateParametersCallHandler>();

            //Register services
            var serviceClasses = new ConcurrentBag<TypeInfo>
                (
                Assembly.GetCallingAssembly()
                    .DefinedTypes.Where(
                        t => !t.IsInterface && !t.IsAbstract && typeof (IServiceBase).IsAssignableFrom(t))
                    .ToList()
                );

            foreach (TypeInfo serviceClass in serviceClasses)
            {
                Type serviceInterface =
                    serviceClass.ImplementedInterfaces.Single(
                        i => typeof (IServiceBase) != i && typeof (IServiceBase).IsAssignableFrom(i));
                container.RegisterType(serviceInterface, serviceClass, new PerContextLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            }
        }

        #endregion

        // Allows CMS & Website to retreive a service
        public TService Service<TService>() where TService : IServiceBase
        {
            return Resolve<TService>();
        }

        // Allows services to retreive any object managed by the container
        internal T Resolve<T>()
        {
            return _unityContainer.Resolve<T>();
            ;
        }

        /// <summary>
        ///     Used to get a class in order to use private methods.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        internal T ResolveTarget<T>() where T : ServiceBase
        {
            Type serviceInterface =
                ((TypeInfo) typeof (T)).ImplementedInterfaces.Single(
                    i => typeof (IServiceBase) != i && typeof (IServiceBase).IsAssignableFrom(i));
            var serviceWrapper = (IServiceBase) _unityContainer.Resolve(serviceInterface);
            return
                (T)
                    serviceWrapper.GetType()
                        .GetField(UnityConstants.TargetFieldName, BindingFlags.NonPublic | BindingFlags.Instance)
                        .GetValue(serviceWrapper);
        }
    }
}