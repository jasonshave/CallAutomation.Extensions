// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;
using CallAutomation.Extensions.Interfaces;
using CallAutomation.Extensions.Models;

namespace CallAutomation.Extensions.Dispatchers;

public class CallAutomationRecognizeEventDispatcher : ICallAutomationRecognizeEventDispatcher
{
    public async ValueTask DispatchAsync(CallAutomationEventBase @event, Delegate callbackFunction,
        CallAutomationClientElements clientElements, IReadOnlyList<DtmfTone> tones)
    {
        if (!callbackFunction.Method.GetParameters().Any())
        {
            await ((ValueTask)callbackFunction.DynamicInvoke()).ConfigureAwait(false);
            return;
        }

        await ((ValueTask)callbackFunction.DynamicInvoke(@event, tones, clientElements.CallConnection, clientElements.CallMedia, clientElements.CallRecording)).ConfigureAwait(false);
    }

    public async ValueTask DispatchAsync(CallAutomationEventBase @event, Delegate callbackFunction, CallAutomationClientElements clientElements)
    {
        if (!callbackFunction.Method.GetParameters().Any())
        {
            await ((ValueTask)callbackFunction.DynamicInvoke()).ConfigureAwait(false);
            return;
        }

        await ((ValueTask)callbackFunction.DynamicInvoke(@event, clientElements.CallConnection, clientElements.CallMedia, clientElements.CallRecording)).ConfigureAwait(false);
    }

    public void Dispatch(CallAutomationEventBase @event, Action<CallAutomationEventBase, IReadOnlyList<DtmfTone>, CallConnection, CallMedia, CallRecording> callbackAction, CallAutomationClientElements clientElements, IReadOnlyList<DtmfTone> tones)
    {
        callbackAction(@event, tones, clientElements.CallConnection, clientElements.CallMedia, clientElements.CallRecording);
    }
}