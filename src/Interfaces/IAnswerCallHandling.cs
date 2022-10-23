// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;

namespace CallAutomation.Extensions.Interfaces;

public interface IAnswerCallHandling
{
    IAnswerCallHandling OnCallConnected<THandler>()
        where THandler : CallAutomationHandler;

    IAnswerCallHandling OnCallDisconnected<THandler>()
        where THandler : CallAutomationHandler;

    IAnswerCallHandling OnCallConnected(
        Func<ValueTask> callbackFunction);

    IAnswerCallHandling OnCallConnected(
        Func<CallConnected, CallConnection, CallMedia, CallRecording, ValueTask> callbackFunction);

    IAnswerCallHandling OnCallDisconnected(
        Func<ValueTask> callbackFunction);

    IAnswerCallHandling OnCallDisconnected(
        Func<CallDisconnected, CallConnection, CallMedia, CallRecording, ValueTask> callbackFunction);

    ValueTask<AnswerCallResult> ExecuteAsync();
}