// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication;
using Azure.Communication.CallAutomation;
using JasonShave.Azure.Communication.Service.CallAutomation.Extensions.Interfaces;
using JasonShave.Azure.Communication.Service.CallAutomation.Extensions.Models;
using JasonShave.Azure.Communication.Service.CallAutomation.Sdk.Contracts;

namespace JasonShave.Azure.Communication.Service.CallAutomation.Extensions.Helpers;

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
        _participant = CommunicationUserIdentifier.FromRawId(id);
        return this;
    }

    public ICanExecuteAsync ToParticipant<T>(string id, Action<PstnParticipantOptions> options)
        where T : PhoneNumberIdentifier
    {
        var participantOptions = new PstnParticipantOptions();
        options(participantOptions);
        _pstnParticipantOptions = participantOptions;
        _participant = PhoneNumberIdentifier.FromRawId(id);
        return this;
    }

    public async ValueTask ExecuteAsync()
    {
        await _client.RedirectCallAsync(_incomingCall.IncomingCallContext, _participant);
    }
}