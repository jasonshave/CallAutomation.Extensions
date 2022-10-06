// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication;
using Azure.Communication.CallAutomation;
using JasonShave.Azure.Communication.Service.CallAutomation.Extensions.Interfaces;

namespace JasonShave.Azure.Communication.Service.CallAutomation.Extensions.Helpers;

internal sealed class CallAutomationRemoveParticipantHelper : ICanRemoveParticipant
{
    private readonly CallConnection _connection;
    private readonly List<CommunicationIdentifier> _identitiesToRemove = new();
    private readonly string _requestId;

    internal CallAutomationRemoveParticipantHelper(CallConnection connection, CommunicationIdentifier firstToRemove, string requestId)
    {
        _connection = connection;
        _requestId = requestId;
        _identitiesToRemove.Add(firstToRemove);
    }

    internal CallAutomationRemoveParticipantHelper(CallConnection connection, CommunicationIdentifier[] firstCollectionToRemove, string requestId)
    {
        _connection = connection;
        _requestId = requestId;
        _identitiesToRemove.AddRange(firstCollectionToRemove);
    }

    public ICanRemoveParticipant RemoveParticipant<T>(string id)
        where T : CommunicationIdentifier
    {
        if (typeof(T) is CommunicationUserIdentifier)
        {
            _identitiesToRemove.Add(new CommunicationUserIdentifier(id));
        }

        if (typeof(T) is PhoneNumberIdentifier)
        {
            _identitiesToRemove.Add(new PhoneNumberIdentifier(id));
        }

        return this;
    }

    public ICanRemoveParticipant RemoveParticipants(CommunicationIdentifier[] participantsToRemove)
    {
        _identitiesToRemove.AddRange(participantsToRemove);
        return this;
    }

    public async ValueTask ExecuteAsync()
    {
        await _connection.RemoveParticipantsAsync(_identitiesToRemove);
    }
}