// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;
using CallAutomation.Extensions.Interfaces;

namespace CallAutomation.Extensions;

public abstract class CallAutomationHandler
{
    /// <summary>
    /// Executed when a new inbound or outbound call is answered.
    /// </summary>
    /// <param name="event"></param>
    /// <param name="callConnection"></param>
    /// <param name="callMedia"></param>
    /// <param name="callRecording"></param>
    /// <returns></returns>
    public virtual ValueTask OnCallConnected(CallConnected @event, IOperationContext operationContext, CallConnection callConnection, CallMedia callMedia, CallRecording callRecording) => ValueTask.CompletedTask;

    /// <summary>
    /// Executed when an existing call leg controlled by Call Automation is disconnected.
    /// </summary>
    /// <param name="event"></param>
    /// <param name="callConnection"></param>
    /// <param name="callMedia"></param>
    /// <param name="callRecording"></param>
    /// <returns></returns>
    public virtual ValueTask OnCallDisconnected(CallDisconnected @event, IOperationContext operationContext, CallConnection callConnection, CallMedia callMedia, CallRecording callRecording) => ValueTask.CompletedTask;

    /// <summary>
    /// Executed when a participant is requested to be added to a call.
    /// </summary>
    /// <param name="event"></param>
    /// <param name="callConnection"></param>
    /// <param name="callMedia"></param>
    /// <param name="callRecording"></param>
    /// <returns></returns>
    public virtual ValueTask OnAddParticipantsSucceeded(AddParticipantsSucceeded @event, IOperationContext operationContext, CallConnection callConnection, CallMedia callMedia, CallRecording callRecording) => ValueTask.CompletedTask;

    /// <summary>
    /// Executed when unable to add a participant to a call upon request.
    /// </summary>
    /// <param name="event"></param>
    /// <param name="callConnection"></param>
    /// <param name="callMedia"></param>
    /// <param name="callRecording"></param>
    /// <returns></returns>
    public virtual ValueTask OnAddParticipantsFailed(AddParticipantsFailed @event, IOperationContext operationContext,  CallConnection callConnection, CallMedia callMedia, CallRecording callRecording) => ValueTask.CompletedTask;

    /// <summary>
    /// Executed when a request to transfer the call succeeds.
    /// </summary>
    /// <param name="event"></param>
    /// <param name="callConnection"></param>
    /// <param name="callMedia"></param>
    /// <param name="callRecording"></param>
    /// <returns></returns>
    public virtual ValueTask OnCallTransferAccepted(CallTransferAccepted @event, IOperationContext operationContext, CallConnection callConnection, CallMedia callMedia, CallRecording callRecording) => ValueTask.CompletedTask;

    /// <summary>
    /// Executed when a request to transfer the call fails.
    /// </summary>
    /// <param name="event"></param>
    /// <param name="callConnection"></param>
    /// <param name="callMedia"></param>
    /// <param name="callRecording"></param>
    /// <returns></returns>
    public virtual ValueTask OnCallTransferFailed(CallTransferFailed @event, IOperationContext operationContext, CallConnection callConnection, CallMedia callMedia, CallRecording callRecording) => ValueTask.CompletedTask;

    /// <summary>
    /// Executed when an action to play audio succeeds.
    /// </summary>
    /// <param name="event"></param>
    /// <param name="callConnection"></param>
    /// <param name="callMedia"></param>
    /// <param name="callRecording"></param>
    /// <returns></returns>
    public virtual ValueTask OnPlayCompleted(PlayCompleted @event, IOperationContext operationContext, CallConnection callConnection, CallMedia callMedia, CallRecording callRecording) => ValueTask.CompletedTask;

    /// <summary>
    /// Executed when an action to play audio fails.
    /// </summary>
    /// <param name="event"></param>
    /// <param name="callConnection"></param>
    /// <param name="callMedia"></param>
    /// <param name="callRecording"></param>
    /// <returns></returns>
    public virtual ValueTask OnPlayFailed(PlayFailed @event, IOperationContext operationContext, CallConnection callConnection, CallMedia callMedia, CallRecording callRecording) => ValueTask.CompletedTask;

    /// <summary>
    /// Executed when the collection of DTMF tones is successful.
    /// </summary>
    /// <param name="event"></param>
    /// <param name="callConnection"></param>
    /// <param name="callMedia"></param>
    /// <param name="callRecording"></param>
    /// <param name="Tones"></param>
    /// <returns></returns>
    public virtual ValueTask OnRecognizeCompleted(RecognizeCompleted @event, IOperationContext operationContext, CallConnection callConnection, CallMedia callMedia, CallRecording callRecording, IReadOnlyList<DtmfTone> Tones) => ValueTask.CompletedTask;

    /// <summary>
    /// Executed when the collection of DTMF tones fails due to lack of input.
    /// </summary>
    /// <param name="event"></param>
    /// <param name="callConnection"></param>
    /// <param name="callMedia"></param>
    /// <param name="callRecording"></param>
    /// <returns></returns>
    public virtual ValueTask OnSilenceTimeout(RecognizeFailed @event, IOperationContext operationContext, CallConnection callConnection, CallMedia callMedia, CallRecording callRecording) => ValueTask.CompletedTask;

    /// <summary>
    /// Executed when the timer expires between collecting multiple DTMF tones.
    /// </summary>
    /// <param name="event"></param>
    /// <param name="callConnection"></param>
    /// <param name="callMedia"></param>
    /// <param name="callRecording"></param>
    /// <returns></returns>
    public virtual ValueTask OnInterToneTimeout(RecognizeFailed @event, IOperationContext operationContext, CallConnection callConnection, CallMedia callMedia, CallRecording callRecording) => ValueTask.CompletedTask;

    /// <summary>
    /// Executed when the recognize DTMF API cannot play the prompt file.
    /// </summary>
    /// <param name="event"></param>
    /// <param name="callConnection"></param>
    /// <param name="callMedia"></param>
    /// <param name="callRecording"></param>
    /// <returns></returns>
    public virtual ValueTask OnPromptFailed(RecognizeFailed @event, IOperationContext operationContext, CallConnection callConnection, CallMedia callMedia, CallRecording callRecording) => ValueTask.CompletedTask;

    /// <summary>
    /// Executed when the call recording state changed.
    /// </summary>
    /// <param name="event"></param>
    /// <param name="callConnection"></param>
    /// <param name="callMedia"></param>
    /// <param name="callRecording"></param>
    /// <returns></returns>
    public virtual ValueTask OnRecordingStateChanged(RecordingStateChanged @event, IOperationContext operationContext, CallConnection callConnection, CallMedia callMedia, CallRecording callRecording) => ValueTask.CompletedTask;
}