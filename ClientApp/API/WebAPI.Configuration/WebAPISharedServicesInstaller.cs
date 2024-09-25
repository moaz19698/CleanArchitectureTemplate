using CleanArchitecture.Application.Common.Abstracts.Business;
using CleanArchitecture.Application.Common.Abstracts.Persistence;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Messaging;
using Common.DependencyInjection.Extensions;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CleanArchitecture.WebAPI.Configuration
{
    public class WebApiSharedServicesInstaller : IServiceInstaller
    {
        public void Install(IServiceCollection services, IConfiguration configuration)
        {
            services.AddValidatorsFromAssembly(GetAssembly(nameof(Application)));
            services.AddAutoMapper(GetAssembly(nameof(Application)));
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(GetAssembly(nameof(Application))));
            services.RegisterAllChildsDynamic(ServiceLifetime.Transient, nameof(Application), nameof(Persistence.EF), typeof(IEntitySet<>));
            services.RegisterAllForBaseDynamic(ServiceLifetime.Transient, nameof(Application), typeof(IRequestResponsePipeline<,>));
            services.RegisterAllForBaseDynamic(ServiceLifetime.Transient, nameof(Application), typeof(IRequestPreProcessor<>));
            services.RegisterAllForBaseDynamic(ServiceLifetime.Transient, nameof(Application), typeof(IRequestPostProcessor<>));
            services.RegisterAllChildsDynamic(ServiceLifetime.Scoped, nameof(Application), typeof(IService));

            static Assembly GetAssembly(string assemblyName) => Array.Find(AppDomain.CurrentDomain.GetAssemblies(), a => a.FullName != null && a.FullName.Contains(assemblyName))
                                            ?? throw new NotFoundAssmblyException(assemblyName);
        }
    }
}
