// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication;
using Azure.Communication.CallAutomation;
using CallAutomation.Extensions.Interfaces;
using CallAutomation.Extensions.Models;
using CallAutomation.Extensions.Services;

namespace CallAutomation.Extensions.Helpers;

/// <inheritdoc />
internal sealed class CallAutomationAddParticipantHelper : HelperCallbackBase, ICanAddParticipant
{
    private readonly CallConnection _connection;
    private readonly List<CommunicationIdentifier> _participantsToAdd = new ();
    private ParticipantOptions? _addParticipantsOptions;
    private PstnParticipantOptions? _pstnParticipantOptions;

    internal CallAutomationAddParticipantHelper(CallConnection connection, CommunicationIdentifier firstUserToAdd, string requestId)
        : base(requestId)
    {
        _connection = connection;
        _participantsToAdd.Add(firstUserToAdd);
    }

    internal CallAutomationAddParticipantHelper(CallConnection connection, CommunicationIdentifier firstUserToAdd, PstnParticipantOptions pstnParticipantOptions, string requestId)
        : base(requestId)
    {
        _connection = connection;
        _participantsToAdd.Add(firstUserToAdd);
        _pstnParticipantOptions = pstnParticipantOptions;
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

    public async ValueTask ExecuteAsync()
    {
        CallbackRegistry.RegisterHelperCallback(this, new[] { typeof(AddParticipantsSucceeded), typeof(AddParticipantsFailed) });

        var addParticipantsOptions = new AddParticipantsOptions(_participantsToAdd)
        {
            OperationContext = RequestId,
            InvitationTimeoutInSeconds = _addParticipantsOptions?.InvitationTimeoutInSeconds,
        };

        if (_pstnParticipantOptions is not null)
        {
            // caller ID number must be set specifically for PSTN participants
            addParticipantsOptions.SourceCallerId =
                new PhoneNumberIdentifier(_pstnParticipantOptions.SourceCallerIdNumber);
        }

        await _connection.AddParticipantsAsync(addParticipantsOptions);
    }
}