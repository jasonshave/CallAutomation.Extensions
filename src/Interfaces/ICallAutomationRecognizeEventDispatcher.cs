// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;
using CallAutomation.Extensions.Models;

namespace CallAutomation.Extensions.Interfaces;

public interface ICallAutomationRecognizeEventDispatcher
{
    ValueTask DispatchAsync(CallAutomationEventBase @event, Delegate callbackFunction, CallAutomationClientElements clientElements, IReadOnlyList<DtmfTone> tones);

    ValueTask DispatchAsync(CallAutomationEventBase @event, Delegate callbackFunction, CallAutomationClientElements clientElements);

    void Dispatch(CallAutomationEventBase @event, Action<CallAutomationEventBase, IReadOnlyList<DtmfTone>, CallConnection, CallMedia, CallRecording> callbackAction, CallAutomationClientElements clientElements, IReadOnlyList<DtmfTone> tones);
}