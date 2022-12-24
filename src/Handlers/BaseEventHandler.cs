// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;
using CallAutomation.Extensions.Interfaces;

namespace CallAutomation.Extensions.Handlers;

public abstract class BaseEventHandler
{
    protected readonly ICallbacksHandler _callbackHandler;
    protected readonly CallAutomationClient _client;

    private readonly IServiceProvider _serviceProvider;

    protected BaseEventHandler(
        IServiceProvider serviceProvider,
        ICallbacksHandler callbackHandler,
        CallAutomationClient client)
    {
        _serviceProvider = serviceProvider;
        _callbackHandler = callbackHandler;
        _client = client;
    }

    protected CallAutomationHandler? GetHandler(string handlerName)
    {
        var handlerType = AppDomain.CurrentDomain.GetAssemblies()
            .Select(assembly => assembly.GetType(handlerName))
            .FirstOrDefault(t => t?.IsSubclassOf(typeof(CallAutomationHandler)) == true);
        if (handlerType is null) return null;

        return (CallAutomationHandler?)_serviceProvider.GetService(handlerType);
    }
}
