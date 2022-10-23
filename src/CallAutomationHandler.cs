// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;

namespace CallAutomation.Extensions;

public abstract class CallAutomationHandler
{
    public virtual ValueTask OnCallConnected(CallConnected @event, CallConnection callConnection, CallMedia callMedia, CallRecording callRecording) => ValueTask.CompletedTask;

    public virtual ValueTask OnCallDisconnected(CallDisconnected @event, CallConnection callConnection, CallMedia callMedia, CallRecording callRecording) => ValueTask.CompletedTask;

    public virtual ValueTask OnAddParticipantsSucceeded(AddParticipantsSucceeded @event, CallConnection callConnection, CallMedia callMedia, CallRecording callRecording) => ValueTask.CompletedTask;

    public virtual ValueTask OnAddParticipantsFailed(AddParticipantsFailed @event, CallConnection callConnection, CallMedia callMedia, CallRecording callRecording) => ValueTask.CompletedTask;

    public virtual ValueTask OnCallTransferAccepted(CallTransferAccepted @event, CallConnection callConnection, CallMedia callMedia, CallRecording callRecording) => ValueTask.CompletedTask;

    public virtual ValueTask OnCallTransferFailed(CallTransferFailed @event, CallConnection callConnection, CallMedia callMedia, CallRecording callRecording) => ValueTask.CompletedTask;

    public virtual ValueTask OnPlayCompleted(PlayCompleted @event, CallConnection callConnection, CallMedia callMedia, CallRecording callRecording) => ValueTask.CompletedTask;

    public virtual ValueTask OnPlayFailed(PlayFailed @event, CallConnection callConnection, CallMedia callMedia, CallRecording callRecording) => ValueTask.CompletedTask;

    public virtual ValueTask OnRecognizeCompleted(RecognizeCompleted @event, CallConnection callConnection, CallMedia callMedia, CallRecording callRecording, IReadOnlyList<DtmfTone> Tones) => ValueTask.CompletedTask;

    public virtual ValueTask OnSilenceTimeout(RecognizeFailed @event, CallConnection callConnection, CallMedia callMedia, CallRecording callRecording) => ValueTask.CompletedTask;

    public virtual ValueTask OnInterToneTimeout(RecognizeFailed @event, CallConnection callConnection, CallMedia callMedia, CallRecording callRecording) => ValueTask.CompletedTask;
}