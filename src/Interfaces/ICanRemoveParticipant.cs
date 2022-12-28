// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication;

namespace CallAutomation.Extensions.Interfaces;

public interface ICanRemoveParticipant
{
    ICanRemoveParticipant RemoveParticipants(CommunicationIdentifier[] participantsToRemove);

    ICanRemoveParticipant RemoveParticipant(string rawId);
}