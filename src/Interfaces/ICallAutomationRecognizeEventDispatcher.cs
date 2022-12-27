// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;
using CallAutomation.Extensions.Models;

namespace CallAutomation.Extensions.Interfaces;

public interface ICallAutomationRecognizeEventDispatcher
{
    ValueTask DispatchDelegateAsync(RecognizeCompleted @event, Delegate callbackFunction, CallAutomationClientElements clientElements, IReadOnlyList<DtmfTone> tones);

    ValueTask DispatchHandlerAsync(RecognizeCompleted @event, object handlerInstance, string methodName, CallAutomationClientElements clientElements, IReadOnlyList<DtmfTone> tones);

    ValueTask DispatchDelegateAsync(RecognizeFailed @event, Delegate callbackFunction, CallAutomationClientElements clientElements);

    ValueTask DispatchHandlerAsync(RecognizeFailed @event, object handlerInstance, string methodName, CallAutomationClientElements clientElements);
}