using application.Behaviors;
using application.Queries;
using application.UseCases;
using application.UseCases.Feature;
using application.UseCases.Plan;
using application.UseCases.Service;
using application.Validators;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
namespace application.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // UseCases
        services.AddScoped<CreatePlanUseCase>();
        services.AddScoped<UpdatePlanUseCase>();
        services.AddScoped<DeletePlanUseCase>();

        services.AddScoped<CreateServiceUseCase>();
        services.AddScoped<UpdateServiceUseCase>();
        services.AddScoped<DeleteServiceUseCase>();

        services.AddScoped<CreateFeatureUseCase>();
        services.AddScoped<UpdateFeatureUseCase>();
        services.AddScoped<DeleteFeatureUseCase>();

        services.AddScoped<BindServiceToPlanUseCase>();
        services.AddScoped<BindFeatureToServiceUseCase>();
        
        // Queries
        services.AddScoped<PlanQueryService>();
        services.AddScoped<ServiceQueryService>();
        services.AddScoped<FeatureQueryService>();

        // Validators (FluentValidation)
        services.AddValidatorsFromAssemblyContaining<CreatePlanValidator>();
        services.AddValidatorsFromAssemblyContaining<CreateServiceValidator>();
        services.AddValidatorsFromAssemblyContaining<CreateFeatureValidator>();

        // MediatR (Event Handlers)
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        });
        return services;
    }
}