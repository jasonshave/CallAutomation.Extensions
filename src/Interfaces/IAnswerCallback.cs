// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;

namespace CallAutomation.Extensions.Interfaces;

public interface IAnswerCallback
{
    IAnswerCallback OnCallConnected<THandler>()
        where THandler : CallAutomationHandler;

    IAnswerCallback OnCallDisconnected<THandler>()
        where THandler : CallAutomationHandler;

    IAnswerCallback OnCallConnected(
        Func<ValueTask> callbackFunction);

    IAnswerCallback OnCallConnected(
        Func<CallConnected, CallConnection, CallMedia, CallRecording, ValueTask> callbackFunction);

    IAnswerCallback OnCallDisconnected(
        Func<ValueTask> callbackFunction);

    IAnswerCallback OnCallDisconnected(
        Func<CallDisconnected, CallConnection, CallMedia, CallRecording, ValueTask> callbackFunction);

    ValueTask ExecuteAsync();
}