// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication;
using Azure.Communication.CallAutomation;
using JasonShave.Azure.Communication.CallAutomation.Extensions.Helpers;
using JasonShave.Azure.Communication.CallAutomation.Extensions.Interfaces;
using JasonShave.Azure.Communication.CallAutomation.Extensions.Models;

namespace JasonShave.Azure.Communication.CallAutomation.Extensions;

public static class CallConnectionExtensions
{
    /// <summary>
    /// Call an Azure Communication Services user or application by specifying a <see cref="CommunicationUserIdentifier"/> type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="client"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public static ICanCallFrom Call<T>(this CallAutomationClient client, string id)
        where T : CommunicationUserIdentifier
    {
        var helper =
            new CallFromAutomationCreateCallHelper(client, new CommunicationUserIdentifier(id), Guid.NewGuid().ToString());
        return helper;
    }

    /// <summary>
    /// Call a PSTN number by specifying a <see cref="PhoneNumberIdentifier"/> type along with <see cref="PstnParticipantOptions"/> to set caller Id data.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="client"></param>
    /// <param name="id"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static ICanCallFrom Call<T>(this CallAutomationClient client, string id, Action<PstnParticipantOptions> options)
        where T : PhoneNumberIdentifier
    {
        var pstnParticipantOptions = new PstnParticipantOptions();
        options(pstnParticipantOptions);
        var helper =
            new CallFromAutomationCreateCallHelper(client, new PhoneNumberIdentifier(id), pstnParticipantOptions, Guid.NewGuid().ToString());
        return helper;
    }

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