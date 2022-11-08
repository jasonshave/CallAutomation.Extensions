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
internal sealed class CallAutomationAddParticipantHelper : HelperCallbackBase, ICanAddParticipant
{
    private static readonly IEnumerable<Type> _types = new[] { typeof(AddParticipantsSucceeded), typeof(AddParticipantsFailed) };
    private readonly CallConnection _connection;
    private readonly List<CommunicationIdentifier> _participantsToAdd = new ();
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
        HelperCallbacks.AddHandlerCallback<THandler, AddParticipantsSucceeded>($"On{nameof(AddParticipantsSucceeded)}", typeof(AddParticipantsSucceeded), typeof(CallConnection), typeof(CallMedia), typeof(CallRecording));
        return this;
    }

    public ICanAddParticipant OnAddParticipantsSucceeded(Func<ValueTask> callbackFunction)
    {
        HelperCallbacks.AddDelegateCallback<AddParticipantsSucceeded>(callbackFunction);
        return this;
    }

    public ICanAddParticipant OnAddParticipantsSucceeded(Func<AddParticipantsSucceeded, CallConnection, CallMedia, CallRecording, ValueTask> callbackFunction)
    {
        HelperCallbacks.AddDelegateCallback<AddParticipantsSucceeded>(callbackFunction);
        return this;
    }

    public ICanAddParticipant OnAddParticipantsFailed<THandler>()
        where THandler : CallAutomationHandler
    {
        HelperCallbacks.AddHandlerCallback<THandler, AddParticipantsSucceeded>($"On{nameof(AddParticipantsSucceeded)}", typeof(AddParticipantsSucceeded), typeof(CallConnection), typeof(CallMedia), typeof(CallRecording));
        return this;
    }

    public ICanAddParticipant OnAddParticipantsFailed(Func<ValueTask> callbackFunction)
    {
        HelperCallbacks.AddDelegateCallback<AddParticipantsFailed>(callbackFunction);
        return this;
    }

    public ICanAddParticipant OnAddParticipantsFailed(Func<AddParticipantsFailed, CallConnection, CallMedia, CallRecording, ValueTask> callbackFunction)
    {
        HelperCallbacks.AddDelegateCallback<AddParticipantsFailed>(callbackFunction);
        return this;
    }

    public async ValueTask<AddParticipantsResult> ExecuteAsync(IOperationContext operationContext)
    {
        var addParticipantsOptions = new AddParticipantsOptions(_participantsToAdd)
        {
            OperationContext = OperationContextToJSON(operationContext),
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