// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure;
using Azure.Communication;
using Azure.Communication.CallAutomation;
using CallAutomation.Extensions.Interfaces;
using CallAutomation.Extensions.Models;
using CallAutomation.Extensions.Services;

namespace CallAutomation.Extensions.Helpers;

/// <inheritdoc />
internal sealed class CallAutomationAddParticipantHelper : HelperCallbackWithContext, ICanAddParticipantWithHandler
{
    private static readonly IEnumerable<Type> _types = new[] { typeof(AddParticipantsSucceeded), typeof(AddParticipantsFailed) };
    private readonly CallConnection _connection;
    private readonly List<CommunicationIdentifier> _participantsToAdd = new();
    private ParticipantOptions? _addParticipantsOptions;
    private PstnParticipantOptions? _pstnParticipantOptions;

    internal CallAutomationAddParticipantHelper(CallConnection connection, CommunicationIdentifier firstUserToAdd, string requestId)
        : base(requestId, _types)
    {
        _connection = connection;
        _participantsToAdd.Add(firstUserToAdd);
    }

    internal CallAutomationAddParticipantHelper(CallConnection connection, CommunicationIdentifier firstUserToAdd, PstnParticipantOptions pstnParticipantOptions, string requestId)
        : base(requestId, _types)
    {
        _connection = connection;
        _participantsToAdd.Add(firstUserToAdd);
        _pstnParticipantOptions = pstnParticipantOptions;
    }

    public ICanAddParticipant WithCallbackHandler(ICallbacksHandler handler)
    {
        CallbackHandler = handler;
        return this;
    }

    public ICanAddParticipant AddParticipant(string rawId)
    {
        _participantsToAdd.Add(CommunicationIdentifier.FromRawId(rawId));
        return this;
    }

    public ICanAddParticipant AddParticipant(string rawId, Action<PstnParticipantOptions> options)
    {
        var pstnParticipantOptions = new PstnParticipantOptions();
        options(pstnParticipantOptions);
        _pstnParticipantOptions = pstnParticipantOptions;
        _participantsToAdd.Add(CommunicationIdentifier.FromRawId(rawId));
        return this;
    }

    public ICanAddParticipant WithOptions(Action<ParticipantOptions> options)
    {
        var participantOptions = new ParticipantOptions();
        options(participantOptions);
        _addParticipantsOptions = participantOptions;
        return this;
    }

    public ICanAddParticipant OnAddParticipantsSucceeded<THandler>()
        where THandler : CallAutomationHandler
    {
        CallbackHandler.AddHandlerCallback<THandler, AddParticipantsSucceeded>(RequestId, $"On{nameof(AddParticipantsSucceeded)}");
        return this;
    }

    public ICanAddParticipant OnAddParticipantsSucceeded(Func<ValueTask> callbackFunction)
    {
        CallbackHandler.AddDelegateCallback<AddParticipantsSucceeded>(RequestId, callbackFunction);
        return this;
    }

    public ICanAddParticipant OnAddParticipantsSucceeded(Func<AddParticipantsSucceeded, CallConnection, CallMedia, CallRecording, ValueTask> callbackFunction)
    {
        CallbackHandler.AddDelegateCallback<AddParticipantsSucceeded>(RequestId, callbackFunction);
        return this;
    }

    public ICanAddParticipant OnAddParticipantsFailed<THandler>()
        where THandler : CallAutomationHandler
    {
        CallbackHandler.AddHandlerCallback<THandler, AddParticipantsFailed>(RequestId, $"On{nameof(AddParticipantsFailed)}");
        return this;
    }

    public ICanAddParticipant OnAddParticipantsFailed(Func<ValueTask> callbackFunction)
    {
        CallbackHandler.AddDelegateCallback<AddParticipantsFailed>(RequestId, callbackFunction);
        return this;
    }

    public ICanAddParticipant OnAddParticipantsFailed(Func<AddParticipantsFailed, CallConnection, CallMedia, CallRecording, ValueTask> callbackFunction)
    {
        CallbackHandler.AddDelegateCallback<AddParticipantsFailed>(RequestId, callbackFunction);
        return this;
    }

    public IExecuteAsync<AddParticipantsResult> WithContext(IOperationContext context)
    {
        SetContext(context);
        return this;
    }

    public async ValueTask<AddParticipantsResult> ExecuteAsync()
    {
        var addParticipantsOptions = new AddParticipantsOptions(_participantsToAdd)
        {
            OperationContext = JSONContext,
            InvitationTimeoutInSeconds = _addParticipantsOptions?.InvitationTimeoutInSeconds,
        };

        if (_pstnParticipantOptions is not null)
        {
            // caller ID number must be set specifically for PSTN participants
            addParticipantsOptions.SourceCallerId =
                new PhoneNumberIdentifier(_pstnParticipantOptions.SourceCallerIdNumber);
        }

        Response<AddParticipantsResult> result = await _connection.AddParticipantsAsync(addParticipantsOptions);
        return result;
    }
}