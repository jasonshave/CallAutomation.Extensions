// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;
using CallAutomation.Extensions.Interfaces;
using CallAutomation.Extensions.Models;
using Microsoft.Extensions.Logging;

namespace CallAutomation.Extensions.Dispatchers;

internal sealed class CallAutomationRecognizeEventDispatcher : ICallAutomationRecognizeEventDispatcher
{
    private readonly ILogger<CallAutomationRecognizeEventDispatcher> _logger;

    public CallAutomationRecognizeEventDispatcher(ILogger<CallAutomationRecognizeEventDispatcher> logger)
    {
        _logger = logger;
    }

    public async ValueTask DispatchDelegateAsync(RecognizeCompleted @event, Delegate callbackFunction, CallAutomationClientElements clientElements, IReadOnlyList<DtmfTone> tones) =>
        await DispatchDelegateCallbackAsync(@event, callbackFunction, clientElements, tones, _logger);

    public async ValueTask DispatchHandlerAsync(RecognizeCompleted @event, CallAutomationHandler handlerInstance, string methodName, CallAutomationClientElements clientElements, IReadOnlyList<DtmfTone> tones) =>
        await DispatchToHandlerAsync(@event, handlerInstance, methodName, clientElements, tones);

    public async ValueTask DispatchDelegateAsync(RecognizeFailed @event, Delegate callbackFunction, CallAutomationClientElements clientElements) =>
        await DispatchDelegateCallbackAsync(@event, callbackFunction, clientElements, null, _logger);

    public async ValueTask DispatchHandlerAsync(RecognizeFailed @event, CallAutomationHandler handlerInstance, string methodName, CallAutomationClientElements clientElements) =>
        await DispatchToHandlerAsync(@event, handlerInstance, methodName, clientElements, null);

    private static async ValueTask DispatchToHandlerAsync(CallAutomationEventBase eventBase, CallAutomationHandler handlerInstance, string methodName, CallAutomationClientElements clientElements, IReadOnlyList<DtmfTone>? tones)
    {
        var methodInfo = handlerInstance.GetType().GetMethod(methodName);
        ValueTask task;
        if (tones is null)
        {
            task = (ValueTask)methodInfo.Invoke(handlerInstance, new object[] { eventBase, clientElements.CallConnection, clientElements.CallMedia, clientElements.CallRecording });
        }
        else
        {
            task = (ValueTask)methodInfo.Invoke(handlerInstance, new object[] { eventBase, clientElements.CallConnection, clientElements.CallMedia, clientElements.CallRecording, tones });
        }

        await task.ConfigureAwait(false);
    }

    private static async ValueTask DispatchDelegateCallbackAsync(CallAutomationEventBase eventBase, Delegate callbackFunction, CallAutomationClientElements clientElements, IReadOnlyList<DtmfTone>? tones, ILogger logger)
    {
        logger.LogDebug($"Found {callbackFunction.Method.GetParameters().Count()} parameters in delegate callback for method {callbackFunction.Method.Name}.");
        if (!callbackFunction.Method.GetParameters().Any())
        {
            await ((ValueTask)callbackFunction.DynamicInvoke()).ConfigureAwait(false);
            return;
        }

        await ((ValueTask)callbackFunction.DynamicInvoke(eventBase, clientElements.CallConnection, clientElements.CallMedia, clientElements.CallRecording)).ConfigureAwait(false);
    }
}