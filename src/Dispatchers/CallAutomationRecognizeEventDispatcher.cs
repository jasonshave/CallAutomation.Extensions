// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;
using CallAutomation.Extensions.Interfaces;
using CallAutomation.Extensions.Models;
using System.Reflection;

namespace CallAutomation.Extensions.Dispatchers;

internal sealed class CallAutomationRecognizeEventDispatcher : ICallAutomationRecognizeEventDispatcher
{
    public async ValueTask DispatchAsync(RecognizeCompleted @event, Delegate callbackFunction, CallAutomationClientElements clientElements, IReadOnlyList<DtmfTone> tones) =>
        await LocalDispatchAsync(@event, callbackFunction, clientElements, tones);

    public async ValueTask DispatchAsync(RecognizeCompleted @event, CallAutomationHandler handlerInstance, string methodName, CallAutomationClientElements clientElements, IReadOnlyList<DtmfTone> tones) =>
        await LocalDispatchAsync(@event, handlerInstance, methodName, clientElements, tones);

    public async ValueTask DispatchAsync(RecognizeFailed @event, Delegate callbackFunction, CallAutomationClientElements clientElements) =>
        await LocalDispatchAsync(@event, callbackFunction, clientElements, null);

    public async ValueTask DispatchAsync(RecognizeFailed @event, CallAutomationHandler handlerInstance, string methodName, CallAutomationClientElements clientElements) =>
        await LocalDispatchAsync(@event, handlerInstance, methodName, clientElements, null);

    private static async ValueTask LocalDispatchAsync(CallAutomationEventBase eventBase, CallAutomationHandler handlerInstance, string methodName, CallAutomationClientElements clientElements, IReadOnlyList<DtmfTone>? tones)
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

    private static async ValueTask LocalDispatchAsync(CallAutomationEventBase eventBase, Delegate callbackFunction, CallAutomationClientElements clientElements, IReadOnlyList<DtmfTone>? tones)
    {
        if (!callbackFunction.Method.GetParameters().Any())
        {
            await ((ValueTask)callbackFunction.DynamicInvoke()).ConfigureAwait(false);
            return;
        }

        await ((ValueTask)callbackFunction.DynamicInvoke(eventBase, clientElements.CallConnection, clientElements.CallMedia, clientElements.CallRecording)).ConfigureAwait(false);
    }
}