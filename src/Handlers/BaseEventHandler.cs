// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;
using CallAutomation.Extensions.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallAutomation.Extensions.Handlers;

public class BaseEventHandler
{
    protected readonly IServiceProvider _serviceProvider;
    protected readonly ICallbacksHandler _callbackHandler;
    protected readonly CallAutomationClient _client;

    public BaseEventHandler(
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
        var cahType = typeof(CallAutomationHandler);
        var handlerType = AppDomain.CurrentDomain.GetAssemblies()
            .Select(assembly => assembly.GetType(handlerName))
            .FirstOrDefault(t => t?.IsSubclassOf(cahType) == true);
        if (handlerType is null) return null;

        return (CallAutomationHandler?)_serviceProvider.GetService(handlerType);
    }
}
