﻿// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication;
using Azure.Communication.CallAutomation;
using CallAutomation.Contracts;
using CallAutomation.Extensions.Interfaces;

namespace CallAutomation.Extensions.Helpers;

internal sealed class CallAutomationRedirectHelper : IRedirectCall
{
    private readonly CallAutomationClient _client;
    private readonly IncomingCall _incomingCall;
    private CommunicationIdentifier _participant;

    internal CallAutomationRedirectHelper(CallAutomationClient client, IncomingCall incomingCall)
    {
        _client = client;
        _incomingCall = incomingCall;
    }

    public ICanExecuteAsync ToParticipant(string rawId)
    {
        _participant = CommunicationIdentifier.FromRawId(rawId);
        return this;
    }

    public async ValueTask ExecuteAsync()
    {
        await _client.RedirectCallAsync(_incomingCall.IncomingCallContext, _participant);
    }
}