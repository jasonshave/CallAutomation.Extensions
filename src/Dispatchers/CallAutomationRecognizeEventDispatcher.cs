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

    public async ValueTask DispatchAsync(RecognizeCompleted @event, MethodInfo methodInfo, object handlerInstance, CallAutomationClientElements clientElements, IReadOnlyList<DtmfTone> tones) =>
        await LocalDispatchAsync(@event, methodInfo, handlerInstance, clientElements, tones);

    private static async ValueTask LocalDispatchAsync(CallAutomationEventBase eventBase, MethodInfo methodInfo, object handlerInstance, CallAutomationClientElements clientElements, IReadOnlyList<DtmfTone>? tones)
    {
        Task task;
        if (tones is null)
        {
            task = methodInfo.Invoke(handlerInstance, new object[] { eventBase, clientElements.CallConnection, clientElements.CallMedia, clientElements.CallRecording }) as Task;
            if (task is null) return;
        }

        task = methodInfo.Invoke(handlerInstance, new object[] { eventBase, clientElements.CallConnection, clientElements.CallMedia, clientElements.CallRecording, tones }) as Task;
        if (task is null) return;

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