// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;
using CallAutomation.Extensions.Interfaces;
using CallAutomation.Extensions.Models;
using CallAutomation.Extensions.Services;
using Microsoft.Extensions.Logging;

namespace CallAutomation.Extensions.Handlers;

internal sealed class CallAutomationEventHandler : ICallAutomationEventHandler
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ICallAutomationEventDispatcher _dispatcher;
    private readonly CallAutomationClient _client;
    private readonly ILogger<CallAutomationEventHandler> _logger;

    public CallAutomationEventHandler(
        IServiceProvider serviceProvider,
        ICallAutomationEventDispatcher dispatcher,
        CallAutomationClient client,
        ILogger<CallAutomationEventHandler> logger)
    {
        _serviceProvider = serviceProvider;
        _dispatcher = dispatcher;
        _client = client;
        _logger = logger;
    }

    public async ValueTask Handle(CallAutomationEventBase eventBase, string? requestId)
    {
        var clientElements = new CallAutomationClientElements(_client, eventBase.CallConnectionId);

        // dispatch handler callbacks
        var handlerTuples = CallAutomationCallbacks.GetHandlers(requestId, eventBase.GetType());
        foreach (var handlerTuple in handlerTuples)
        {
            var cahType = typeof(CallAutomationHandler);
            var handlerType = AppDomain.CurrentDomain.GetAssemblies()
                .Select(assembly => assembly.GetType(handlerTuple.HandlerName))
                .FirstOrDefault(t => t?.IsSubclassOf(cahType) == true);
            if (handlerType is null) return;

            var handler = (CallAutomationHandler)_serviceProvider.GetService(handlerType);

            if (handler is null) return;

            _logger.LogInformation("Found callback handler for request {requestId} and event {event}", requestId, eventBase.GetType());

            await _dispatcher.DispatchAsync(eventBase, handler, handlerTuple.MethodName, clientElements);
        }
    }
}