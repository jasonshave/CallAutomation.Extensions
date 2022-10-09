// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;

namespace CallAutomation.Extensions;

public abstract class CallAutomationHandler
{
    public virtual ValueTask OnCallConnected(CallConnected @event, CallConnection connection, CallMedia media, CallRecording callRecording) => ValueTask.CompletedTask;

    public virtual ValueTask OnCallDisconnected(CallDisconnected @event, CallConnection connection, CallMedia media, CallRecording callRecording) => ValueTask.CompletedTask;

    public virtual ValueTask OnAddParticipantsSucceeded(AddParticipantsSucceeded @event, CallConnection connection, CallMedia media, CallRecording callRecording) => ValueTask.CompletedTask;

    public virtual ValueTask OnAddParticipantsFailed(AddParticipantsFailed @event, CallConnection connection, CallMedia media, CallRecording callRecording) => ValueTask.CompletedTask;

    public virtual ValueTask OnCallTransferAccepted(CallTransferAccepted @event, CallConnection connection, CallMedia media, CallRecording callRecording) => ValueTask.CompletedTask;

    public virtual ValueTask OnCallTransferFailed(CallTransferFailed @event, CallConnection connection, CallMedia media, CallRecording callRecording) => ValueTask.CompletedTask;

    public virtual ValueTask OnPlayCompleted(PlayCompleted @event, CallConnection connection, CallMedia media, CallRecording callRecording) => ValueTask.CompletedTask;

    public virtual ValueTask OnPlayFailed(PlayFailed @event, CallConnection connection, CallMedia media, CallRecording callRecording) => ValueTask.CompletedTask;

    public virtual ValueTask OnRecognizeCompleted(RecognizeCompleted @event, CallConnection connection, CallMedia media, CallRecording callRecording) => ValueTask.CompletedTask;

    public virtual ValueTask OnRecognizeFailed(RecognizeFailed @event, CallConnection connection, CallMedia media, CallRecording callRecording) => ValueTask.CompletedTask;
}