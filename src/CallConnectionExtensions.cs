﻿// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication;
using Azure.Communication.CallAutomation;
using CallAutomation.Contracts;
using CallAutomation.Extensions.Helpers;
using CallAutomation.Extensions.Interfaces;
using CallAutomation.Extensions.Models;

namespace CallAutomation.Extensions;

public static class CallConnectionExtensions
{
    public static ICanAddParticipant AddParticipant<T>(this CallConnection connection, string id)
        where T : CommunicationUserIdentifier
    {
        var helper = new CallAutomationAddParticipantHelper(connection, new CommunicationUserIdentifier(id), Guid.NewGuid().ToString());
        return helper;
    }

    public static ICanAddParticipant AddParticipant<T>(this CallConnection connection, string id, Action<PstnParticipantOptions> options)
        where T : PhoneNumberIdentifier
    {
        var pstnParticipantOptions = new PstnParticipantOptions();
        options(pstnParticipantOptions);
        var helper = new CallAutomationAddParticipantHelper(connection, new PhoneNumberIdentifier(id), pstnParticipantOptions, Guid.NewGuid().ToString());
        return helper;
    }

    public static ICanRemoveParticipant RemoveParticipant<T>(this CallConnection connection, string id)
    {
        var firstUserToAdd = CommunicationIdentifier.FromRawId(id);
        var helper = new CallAutomationRemoveParticipantHelper(connection, firstUserToAdd, Guid.NewGuid().ToString());
        return helper;
    }

    public static ICanRemoveParticipant RemoveParticipants(this CallConnection connection, CommunicationIdentifier[] participantsToRemove)
    {
        var helper = new CallAutomationRemoveParticipantHelper(connection, participantsToRemove, Guid.NewGuid().ToString());
        return helper;
    }
}