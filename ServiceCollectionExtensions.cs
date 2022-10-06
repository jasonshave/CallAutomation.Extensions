// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;
using JasonShave.Azure.Communication.Service.CallAutomation.Extensions.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace JasonShave.Azure.Communication.Service.CallAutomation.Extensions;

public static class ServiceCollectionExtensions
{
    public static CallAutomationConfigurationBuilder AddCallAutomationClient(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton(new CallAutomationClient(connectionString));
        return new CallAutomationConfigurationBuilder(services);
    }

    public static CallAutomationConfigurationBuilder AddCallAutomationEventPublisher(
        this CallAutomationConfigurationBuilder builder)
    {
        builder.Services.AddSingleton<ICallAutomationEventPublisher, CallAutomationEventPublisher>();
        return builder;
    }

    public static CallAutomationConfigurationBuilder DiscoverCallbackHandlers(this CallAutomationConfigurationBuilder builder, params Type[] searchTypes)
    {
        return builder;
    }
}