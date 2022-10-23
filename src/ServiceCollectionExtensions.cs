// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;
using CallAutomation.Extensions.Dispatchers;
using CallAutomation.Extensions.Handlers;
using CallAutomation.Extensions.Interfaces;
using CallAutomation.Extensions.Models;
using CallAutomation.Extensions.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CallAutomation.Extensions;

public static class ServiceCollectionExtensions
{
    public static CallAutomationConfigurationBuilder AddCallAutomationClient(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton(new CallAutomationClient(connectionString));
        return new CallAutomationConfigurationBuilder(services);
    }

    public static CallAutomationConfigurationBuilder AddCallAutomationClient(this IServiceCollection services, Action<CallAutomationSdkClientOptions> options)
    {
        var callAutomationClientOptions = new CallAutomationSdkClientOptions();
        options(callAutomationClientOptions);

        services.AddSingleton(callAutomationClientOptions.OverrideEndpointUri is null
            ? new CallAutomationClient(callAutomationClientOptions.ConnectionString)
            : new CallAutomationClient(callAutomationClientOptions.OverrideEndpointUri,
                callAutomationClientOptions.ConnectionString));

        return new CallAutomationConfigurationBuilder(services);
    }

    public static CallAutomationConfigurationBuilder AddCallAutomationEventHandling(
        this CallAutomationConfigurationBuilder builder)
    {
        builder.Services.AddSingleton<ICallAutomationEventPublisher, CallAutomationEventPublisher>();
        builder.Services.AddSingleton<ICallAutomationEventHandler, CallAutomationEventHandler>();
        builder.Services.AddSingleton<ICallAutomationEventDispatcher, CallAutomationEventDispatcher>();
        builder.Services.AddSingleton<ICallAutomationRecognizeEventDispatcher, CallAutomationRecognizeEventDispatcher>();
        builder.Services.AddSingleton<ICallAutomationRecognizeDtmfHandler, CallAutomationRecognizeDtmfEventHandler>();
        return builder;
    }

    // todo: add auto discovery
    // public static CallAutomationConfigurationBuilder DiscoverCallbackHandlers(this CallAutomationConfigurationBuilder builder, params Type[] searchTypes)
    // {
    //     return builder;
    // }
}