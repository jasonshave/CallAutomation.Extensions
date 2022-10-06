// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication;
using Azure.Communication.CallAutomation;
using JasonShave.Azure.Communication.Service.CallAutomation.Extensions.Interfaces;
using JasonShave.Azure.Communication.Service.CallAutomation.Extensions.Models;

namespace JasonShave.Azure.Communication.Service.CallAutomation.Extensions.Helpers;

/// <inheritdoc />
internal sealed class CallAutomationAddParticipantHelper :
    ICanAddParticipant,
    IAddParticipantOptions
{
    private readonly CallConnection _connection;
    private readonly string _requestId;
    private readonly List<CommunicationIdentifier> _participantsToAdd = new();
    private ParticipantOptions? _addParticipantsOptions;
    private PstnParticipantOptions? _pstnParticipantOptions;

    internal CallAutomationAddParticipantHelper(CallConnection connection, CommunicationIdentifier firstUserToAdd, string requestId)
    {
        _connection = connection;
        _participantsToAdd.Add(firstUserToAdd);
        _requestId = requestId;
    }

    internal CallAutomationAddParticipantHelper(CallConnection connection, CommunicationIdentifier firstUserToAdd, PstnParticipantOptions pstnParticipantOptions, string requestId)
    {
        _connection = connection;
        _participantsToAdd.Add(firstUserToAdd);
        _pstnParticipantOptions = pstnParticipantOptions;
        _requestId = requestId;
    }

    public ICanAddParticipant AddParticipant<TUser>(string id)
        where TUser : CommunicationUserIdentifier
    {
        _participantsToAdd.Add(new CommunicationUserIdentifier(id));
        return this;
    }

    public ICanAddParticipant AddParticipant<TUser>(string id, Action<PstnParticipantOptions> options)
        where TUser : PhoneNumberIdentifier
    {
        var participantOptions = new PstnParticipantOptions();
        options(participantOptions);
        _pstnParticipantOptions = participantOptions;
        _participantsToAdd.Add(new PhoneNumberIdentifier(id));
        return this;
    }

    public IAddParticipantOptions WithOptions(Action<ParticipantOptions> options)
    {
        var participantOptions = new ParticipantOptions();
        options(participantOptions);
        _addParticipantsOptions = participantOptions;
        return this;
    }

    public IAddParticipantOptions OnAddParticipantsSucceeded<THandler>()
        where THandler : CallAutomationHandler
    {
        CallbackRegistry.Register<THandler, AddParticipantsSucceeded>(_requestId);
        return this;
    }

    public IAddParticipantOptions OnAddParticipantsSucceeded(Func<ValueTask> callbackFunction)
    {
        CallbackRegistry.Register<AddParticipantsSucceeded>(_requestId, callbackFunction);
        return this;
    }

    public IAddParticipantOptions OnAddParticipantsSucceeded(Func<AddParticipantsSucceeded, CallConnection, CallMedia, CallRecording, ValueTask> callbackFunction)
    {
        CallbackRegistry.Register(_requestId, callbackFunction);
        return this;
    }

    public IAddParticipantOptions OnAddParticipantsFailed<THandler>()
        where THandler : CallAutomationHandler
    {
        CallbackRegistry.Register<THandler, AddParticipantsFailed>(_requestId);
        return this;
    }

    public IAddParticipantOptions OnAddParticipantsFailed(Func<ValueTask> callbackFunction)
    {
        CallbackRegistry.Register<AddParticipantsFailed>(_requestId, callbackFunction);
        return this;
    }

    public IAddParticipantOptions OnAddParticipantsFailed(Func<AddParticipantsFailed, CallConnection, CallMedia, CallRecording, ValueTask> callbackFunction)
    {
        CallbackRegistry.Register(_requestId, callbackFunction);
        return this;
    }

    public async ValueTask ExecuteAsync()
    {
        var addParticipantsOptions = new AddParticipantsOptions(_participantsToAdd)
        {
            OperationContext = _requestId,
            InvitationTimeoutInSeconds = _addParticipantsOptions?.InvitationTimeoutInSeconds
        };

        if (_pstnParticipantOptions is not null)
        {
            // set caller ID number specifically for PSTN
            addParticipantsOptions.SourceCallerId =
                new PhoneNumberIdentifier(_pstnParticipantOptions.SourceCallerIdNumber);
        }

        await _connection.AddParticipantsAsync(addParticipantsOptions);
    }
}