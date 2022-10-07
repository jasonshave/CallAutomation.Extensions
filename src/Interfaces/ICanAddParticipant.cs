// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication;
using Azure.Communication.CallAutomation;
using JasonShave.Azure.Communication.CallAutomation.Extensions.Models;

namespace JasonShave.Azure.Communication.CallAutomation.Extensions.Interfaces;

public interface ICanAddParticipant
{
    /// <summary>
    /// Adds a type of <see cref="CommunicationUserIdentifier"/> to the active call.
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <param name="id"></param>
    ICanAddParticipant AddParticipant<TUser>(string id)
        where TUser : CommunicationUserIdentifier;

    /// <summary>
    /// Adds a type of <see cref="PhoneNumberIdentifier"/> to the active call.
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <param name="id"></param>
    /// <param name="options">Allows for the customization of options when adding a PSTN participant to the call.</param>
    /// <returns></returns>
    ICanAddParticipant AddParticipant<TUser>(string id, Action<PstnParticipantOptions> options)
        where TUser : PhoneNumberIdentifier;

    /// <summary>
    /// Allows for customization of the add participant action.
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    ICanAddParticipant WithOptions(Action<ParticipantOptions> options);

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

    /// <summary>
    /// Invokes asynchronous delayed execution of the action being called.
    /// </summary>
    /// <returns></returns>
    ValueTask ExecuteAsync();
}