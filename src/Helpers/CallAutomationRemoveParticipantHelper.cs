// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication;
using Azure.Communication.CallAutomation;
using CallAutomation.Extensions.Interfaces;

namespace CallAutomation.Extensions.Helpers;

internal sealed class CallAutomationRemoveParticipantHelper : ICanRemoveParticipant
{
    private readonly CallConnection _connection;
    private readonly List<CommunicationIdentifier> _identitiesToRemove = new();

    internal CallAutomationRemoveParticipantHelper(CallConnection connection, CommunicationIdentifier id)
    {
        _connection = connection;
        _identitiesToRemove.Add(id);
    }

    internal CallAutomationRemoveParticipantHelper(CallConnection connection, CommunicationIdentifier[] firstCollectionToRemove)
    {
        _connection = connection;
        _identitiesToRemove.AddRange(firstCollectionToRemove);
    }

    public ICanRemoveParticipant RemoveParticipant(string rawId)
    {
        _identitiesToRemove.Add(CommunicationIdentifier.FromRawId(rawId));

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