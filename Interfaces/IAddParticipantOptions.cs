// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;

namespace JasonShave.Azure.Communication.Service.CallAutomation.Extensions.Interfaces;

public interface IAddParticipantOptions
{
    IAddParticipantOptions OnAddParticipantsSucceeded<THandler>()
        where THandler : CallAutomationHandler;

    IAddParticipantOptions OnAddParticipantsSucceeded(
        Func<AddParticipantsSucceeded, CallConnection, CallMedia, CallRecording, ValueTask> callbackFunction);

    IAddParticipantOptions OnAddParticipantsSucceeded(
        Func<ValueTask> callbackFunction);

    IAddParticipantOptions OnAddParticipantsFailed<THandler>()
        where THandler : CallAutomationHandler;

    IAddParticipantOptions OnAddParticipantsFailed(
        Func<ValueTask> callbackFunction);

    IAddParticipantOptions OnAddParticipantsFailed(
        Func<AddParticipantsFailed, CallConnection, CallMedia, CallRecording, ValueTask> callbackFunction);

    ValueTask ExecuteAsync();
}