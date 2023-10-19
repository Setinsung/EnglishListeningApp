﻿using Commons.Helpers;
using IdentityService.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService.Infrastructure;

public class ModuleInitializer : IModuleInitializer
{
    public void Initialize(IServiceCollection services)
    {
        services.AddScoped<IdDomainService>();
        services.AddScoped<IIdentityServiceRepository, IdentityServiceRepository>();
    }
}
