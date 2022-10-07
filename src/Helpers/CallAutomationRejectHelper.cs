﻿// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;
using JasonShave.Azure.Communication.CallAutomation.Extensions.Interfaces;
using JasonShave.Azure.Communication.CallAutomation.Extensions.Models;

namespace JasonShave.Azure.Communication.CallAutomation.Extensions.Helpers;

internal sealed class CallAutomationRejectHelper : IRejectCallWithReason, IRejectCall
{
    private readonly CallAutomationClient _client;
    private readonly IncomingCall _incomingCall;
    private CallRejectReason _rejectReason;

    internal CallAutomationRejectHelper(CallAutomationClient client, IncomingCall incomingCall)
    {
        _client = client;
        _incomingCall = incomingCall;
    }

    public IRejectCall WithReason(CallRejectReason reason)
    {
        _rejectReason = reason;
        return this;
    }

    public async ValueTask ExecuteAsync() => await _client.RejectCallAsync(_incomingCall.IncomingCallContext, _rejectReason);
}