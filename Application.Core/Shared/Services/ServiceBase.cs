using Application.DAL;
using Microsoft.Practices.Unity;

namespace Application.Core.Shared.Services
{
    public abstract class ServiceBase : IServiceBase
    {
        [Dependency]
        public CoreContext CoreContext { get; set; }

        [Dependency]
        public ApplicationEntities Entities { get; set; }
    }
}