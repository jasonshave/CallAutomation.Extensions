﻿// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using System.Reflection;
using Azure.Communication.CallAutomation;
using CallAutomation.Extensions.Models;

namespace CallAutomation.Extensions.Interfaces;

public interface ICallAutomationRecognizeEventDispatcher
{
    ValueTask DispatchAsync(RecognizeCompleted @event, Delegate callbackFunction, CallAutomationClientElements clientElements, IReadOnlyList<DtmfTone> tones);

    ValueTask DispatchAsync(RecognizeCompleted @event, MethodInfo methodInfo, object handlerInstance, CallAutomationClientElements clientElements, IReadOnlyList<DtmfTone> tones);

    ValueTask DispatchAsync(RecognizeFailed @event, Delegate callbackFunction, CallAutomationClientElements clientElements);

    ValueTask DispatchAsync(RecognizeFailed @event, MethodInfo methodInfo, object handlerInstance, CallAutomationClientElements clientElements);
}