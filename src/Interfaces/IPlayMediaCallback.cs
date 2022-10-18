// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication;
using Azure.Communication.CallAutomation;

namespace CallAutomation.Extensions.Interfaces;

public interface IPlayMediaCallback
{
    IPlayMediaCallback ToParticipant<T>(string rawId)
        where T : CommunicationIdentifier;

    IPlayMediaCallback OnPlayCompleted<THandler>()
        where THandler : CallAutomationHandler;

    IPlayMediaCallback OnPlayCompleted(
        Func<PlayCompleted, CallConnection, CallMedia, CallRecording, ValueTask> callbackFunction);

    IPlayMediaCallback OnPlayCompleted(Func<ValueTask> callbackFunction);

    IPlayMediaCallback OnPlayFailed<THandler>()
        where THandler : CallAutomationHandler;

    IPlayMediaCallback OnPlayFailed(
        Func<PlayFailed, CallConnection, CallMedia, CallRecording, ValueTask> callbackFunction);

    ValueTask ExecuteAsync();
}