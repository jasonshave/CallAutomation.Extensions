// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication;

namespace CallAutomation.Extensions.Models;

public sealed class CallTarget
{
    public CommunicationIdentifier Target { get; }

    public PhoneNumberIdentifier? CallerId { get; }

    public string? DisplayName { get; }

    public CallTarget(PhoneNumberIdentifier target, PhoneNumberIdentifier callerId, string? displayName = null)
    {
        Target = target;
        CallerId = callerId;
        DisplayName = displayName;
    }

    public CallTarget(CommunicationUserIdentifier target, string? displayName = null)
    {
        Target = target;
        DisplayName = displayName;
    }

    public CallTarget(MicrosoftTeamsUserIdentifier target, string? displayName = null)
    {
        Target = target;
        DisplayName = displayName;
    }
}