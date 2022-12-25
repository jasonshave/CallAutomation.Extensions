// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;
using CallAutomation.Extensions.Dispatchers;
using CallAutomation.Extensions.Handlers;
using CallAutomation.Extensions.Interfaces;
using CallAutomation.Extensions.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CallAutomation.Extensions;

public static class ServiceCollectionExtensions
{
    public static CallAutomationConfigurationBuilder AddCallAutomationClient(this IServiceCollection services, string connectionString, string? endpointOverride = null)
    {
        services.AddSingleton(endpointOverride is not null
            ? new CallAutomationClient(new Uri(endpointOverride), connectionString)
            : new CallAutomationClient(connectionString));

        return new CallAutomationConfigurationBuilder(services);
    }

    public static CallAutomationConfigurationBuilder AddCallAutomationEventHandling(this CallAutomationConfigurationBuilder builder)
    {
        builder.Services.AddSingleton<ICallAutomationEventPublisher, CallAutomationEventPublisher>();
        builder.Services.AddSingleton<ICallAutomationEventHandler, CallAutomationEventHandler>();
        builder.Services.AddSingleton<ICallAutomationEventDispatcher, CallAutomationEventDispatcher>();
        builder.Services.AddSingleton<ICallAutomationRecognizeEventDispatcher, CallAutomationRecognizeEventDispatcher>();
        builder.Services.AddSingleton<ICallAutomationRecognizeDtmfHandler, CallAutomationRecognizeDtmfEventHandler>();
        return builder;
    }
}