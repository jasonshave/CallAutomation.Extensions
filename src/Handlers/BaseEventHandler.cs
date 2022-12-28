// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;

namespace CallAutomation.Extensions.Handlers;

public abstract class BaseEventHandler
{
    protected readonly CallAutomationClient _client;

    private readonly IServiceProvider _serviceProvider;

    protected BaseEventHandler(
        IServiceProvider serviceProvider,
        CallAutomationClient client)
    {
        _serviceProvider = serviceProvider;
        _client = client;
    }

    protected object? GetHandler(Type handlerType)
    {
        //var handlerType = AppDomain.CurrentDomain.GetAssemblies()
        //    .Select(assembly => assembly.GetType(handlerName))
        //    .FirstOrDefault(t => t?.IsSubclassOf(typeof(CallAutomationHandler)) == true);
        //if (handlerType is null) return null;

        return _serviceProvider.GetService(handlerType);
    }
}
