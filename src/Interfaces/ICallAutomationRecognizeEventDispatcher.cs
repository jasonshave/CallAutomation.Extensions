// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;
using CallAutomation.Extensions.Models;

namespace CallAutomation.Extensions.Interfaces;

public interface ICallAutomationRecognizeEventDispatcher
{
    ValueTask DispatchAsync(RecognizeCompleted @event, Delegate callbackFunction, CallAutomationClientElements clientElements, IReadOnlyList<DtmfTone> tones);

    ValueTask DispatchAsync(RecognizeCompleted @event, IOperationContext? operationContext, CallAutomationHandler handlerInstance, string methodName, CallAutomationClientElements clientElements, IReadOnlyList<DtmfTone> tones);

    ValueTask DispatchAsync(RecognizeFailed @event, Delegate callbackFunction, CallAutomationClientElements clientElements);

    ValueTask DispatchAsync(RecognizeFailed @event, IOperationContext? operationContext, CallAutomationHandler handlerInstance, string methodName, CallAutomationClientElements clientElements);
}