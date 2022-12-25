// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;
using CallAutomation.Extensions.Interfaces;
using CallAutomation.Extensions.Models;
using Microsoft.Extensions.Logging;

namespace CallAutomation.Extensions.Dispatchers;

internal sealed class CallAutomationEventDispatcher : ICallAutomationEventDispatcher
{
    private readonly ILogger<CallAutomationEventDispatcher> _logger;

    public CallAutomationEventDispatcher(ILogger<CallAutomationEventDispatcher> logger)
    {
        _logger = logger;
    }

    public async ValueTask DispatchDelegateAsync(CallAutomationEventBase @event, Delegate callbackFunction, CallAutomationClientElements clientElements)
    {
        _logger.LogDebug($"Found {callbackFunction.Method.GetParameters().Count()} parameters in delegate callback for method {callbackFunction.Method.Name}.");
        if (!callbackFunction.Method.GetParameters().Any())
        {
            await ((ValueTask)callbackFunction.DynamicInvoke()).ConfigureAwait(false);
            return;
        }

        await ((ValueTask)callbackFunction.DynamicInvoke(@event, clientElements.CallConnection, clientElements.CallMedia, clientElements.CallRecording)).ConfigureAwait(false);
    }

    public async ValueTask DispatchHandlerAsync(CallAutomationEventBase @event, CallAutomationHandler handlerInstance, string methodName, CallAutomationClientElements clientElements)
    {
        var methodInfo = handlerInstance.GetType().GetMethod(methodName);
        var task = (ValueTask)methodInfo.Invoke(handlerInstance, new object[] { @event, clientElements.CallConnection, clientElements.CallMedia, clientElements.CallRecording });
        await task.ConfigureAwait(false);
    }
}