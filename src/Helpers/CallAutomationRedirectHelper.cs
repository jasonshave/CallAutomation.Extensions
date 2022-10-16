// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;
using CallAutomation.Contracts;
using CallAutomation.Extensions.Interfaces;
using CallAutomation.Extensions.Models;
using CommunicationIdentifier = Azure.Communication.CommunicationIdentifier;
using CommunicationUserIdentifier = Azure.Communication.CommunicationUserIdentifier;

namespace CallAutomation.Extensions.Helpers;

internal sealed class CallAutomationRedirectHelper : IRedirectCall
{
    private readonly CallAutomationClient _client;
    private readonly IncomingCall _incomingCall;
    private CommunicationIdentifier _participant;
    private PstnParticipantOptions? _pstnParticipantOptions;

    internal CallAutomationRedirectHelper(CallAutomationClient client, IncomingCall incomingCall)
    {
        _client = client;
        _incomingCall = incomingCall;
    }

    public ICanExecuteAsync ToParticipant<T>(string id)
        where T : CommunicationUserIdentifier
    {
        _participant = CommunicationIdentifier.FromRawId(id);
        return this;
    }

    public async ValueTask ExecuteAsync()
    {
        await _client.RedirectCallAsync(_incomingCall.IncomingCallContext, _participant);
    }
}