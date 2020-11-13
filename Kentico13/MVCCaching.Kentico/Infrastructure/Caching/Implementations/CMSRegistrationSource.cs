using Autofac.Builder;
using Autofac.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MVCCaching.Kentico
{

    public class CMSRegistrationSource : IRegistrationSource
    {
        /// <summary>
        /// Gets whether the registrations provided by this source are 1:1 adapters on top of other components (I.e. like Meta, Func or Owned.)
        /// </summary>
        public bool IsAdapterForIndividualComponents => false;

        public IEnumerable<IComponentRegistration> RegistrationsFor(Service service, Func<Service, IEnumerable<ServiceRegistration>> registrationAccessor)
        {
            // Checks whether the container already contains an existing registration for the requested service
            if (registrationAccessor(service).Any())
            {
                return Enumerable.Empty<IComponentRegistration>();
            }

            // Checks that the requested service carries valid type information
            var swt = service as IServiceWithType;
            if (swt == null)
            {
                return Enumerable.Empty<IComponentRegistration>();
            }

            // Gets an instance of the requested service using the CMS.Core API
            object instance = null;
            if (CMS.Core.Service.ResolveOptional(swt.ServiceType) != null)
            {
                instance = CMS.Core.Service.Resolve(swt.ServiceType);
            }

            if (instance == null)
            {
                return Enumerable.Empty<IComponentRegistration>();
            }

            // Registers the service instance in the container
            return new[] { RegistrationBuilder.ForDelegate(swt.ServiceType, (c, p) => instance).CreateRegistration() };
        }
    }
}