// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication;
using Azure.Communication.CallAutomation;
using CallAutomation.Extensions.Helpers;
using CallAutomation.Extensions.Interfaces;
using CallAutomation.Extensions.Models;

namespace CallAutomation.Extensions;

public static class CallConnectionExtensions
{
    /// <summary>
    /// Add a participant to an existing call using the <see cref="string"/> raw ID value.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="rawId"></param>
    public static ICanAddParticipant AddParticipant(this CallConnection connection, string rawId)
    {
        var helper = new CallAutomationAddParticipantHelper(connection, CommunicationIdentifier.FromRawId(rawId));
        return helper;
    }

    /// <summary>
    /// Adds a PSTN participant while offering options to set for the request using the <see cref="string"/> raw ID value.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="rawId"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static ICanAddParticipant AddParticipant(this CallConnection connection, string rawId, Action<PstnParticipantOptions> options)
    {
        var pstnParticipantOptions = new PstnParticipantOptions();
        options(pstnParticipantOptions);
        var helper = new CallAutomationAddParticipantHelper(connection, CommunicationIdentifier.FromRawId(rawId), pstnParticipantOptions);
        return helper;
    }

    /// <summary>
    /// Removes a participant from the call using the <see cref="string"/> raw ID value.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="rawId"></param>
    /// <returns></returns>
    public static ICanRemoveParticipant RemoveParticipant(this CallConnection connection, string rawId)
    {
        var helper = new CallAutomationRemoveParticipantHelper(connection, CommunicationIdentifier.FromRawId(rawId));
        return helper;
    }

    /// <summary>
    /// Removes a collection of identities from a call using the <see cref="CommunicationIdentifier"/> abstract type.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="participantsToRemove"></param>
    /// <returns></returns>
    public static ICanRemoveParticipant RemoveParticipants(this CallConnection connection, CommunicationIdentifier[] participantsToRemove)
    {
        var helper = new CallAutomationRemoveParticipantHelper(connection, participantsToRemove);
        return helper;
    }
}