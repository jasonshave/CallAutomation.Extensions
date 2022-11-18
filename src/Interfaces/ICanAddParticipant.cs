﻿// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;
using CallAutomation.Extensions.Models;

namespace CallAutomation.Extensions.Interfaces;

public interface ICanAddParticipant : IExecuteAsync<AddParticipantsResult>
{
    ICanAddParticipant AddParticipant(string rawId);

    ICanAddParticipant AddParticipant(string rawId, Action<PstnParticipantOptions> options);

    ICanAddParticipant WithOptions(Action<ParticipantOptions> options);

    ICanAddParticipant WithContext<TContext>(TContext context)
        where TContext : OperationContext;

    ICanAddParticipant OnAddParticipantsSucceeded<THandler>()
        where THandler : CallAutomationHandler;

    ICanAddParticipant OnAddParticipantsSucceeded(
        Func<AddParticipantsSucceeded, CallConnection, CallMedia, CallRecording, ValueTask> callbackFunction);

    ICanAddParticipant OnAddParticipantsSucceeded(
        Func<ValueTask> callbackFunction);

    ICanAddParticipant OnAddParticipantsFailed<THandler>()
        where THandler : CallAutomationHandler;

    ICanAddParticipant OnAddParticipantsFailed(
        Func<ValueTask> callbackFunction);

    ICanAddParticipant OnAddParticipantsFailed(
        Func<AddParticipantsFailed, CallConnection, CallMedia, CallRecording, ValueTask> callbackFunction);
}