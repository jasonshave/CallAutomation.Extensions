// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication;
using Azure.Communication.CallAutomation;
using CallAutomation.Extensions.Interfaces;
using CallAutomation.Extensions.Models;
using CallAutomation.Extensions.Services;

namespace CallAutomation.Extensions.Helpers;

internal sealed class CallAutomationRemoveParticipantHelper : HelperCallbackWithContext, ICanRemoveParticipantWithHandler
{
    private readonly CallConnection _connection;
    private readonly List<CommunicationIdentifier> _identitiesToRemove = new();

    internal CallAutomationRemoveParticipantHelper(CallConnection connection, CommunicationIdentifier id, string requestId)
        : base(requestId)
    {
        _connection = connection;
        _identitiesToRemove.Add(id);
    }

    internal CallAutomationRemoveParticipantHelper(CallConnection connection, CommunicationIdentifier[] firstCollectionToRemove, string requestId)
        : base(requestId)
    {
        _connection = connection;
        _identitiesToRemove.AddRange(firstCollectionToRemove);
    }

    public ICanRemoveParticipant WithCallbackHandler(ICallbacksHandler handler)
    {
        CallbackHandler = handler;
        return this;
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

    public IExecuteAsync WithContext(OperationContext context)
    {
        SetContext(context);
        return this;
    }

    public async ValueTask ExecuteAsync()
    {
        await _connection.RemoveParticipantsAsync(_identitiesToRemove);
    }
}