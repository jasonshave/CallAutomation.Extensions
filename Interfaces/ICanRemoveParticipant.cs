// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication;

namespace JasonShave.Azure.Communication.Service.CallAutomation.Extensions.Interfaces;

public interface ICanRemoveParticipant
{
    ICanRemoveParticipant RemoveParticipants(CommunicationIdentifier[] participantsToRemove);

    ICanRemoveParticipant RemoveParticipant<T>(string id)
        where T : CommunicationIdentifier;

    ValueTask ExecuteAsync();
}