// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication;

namespace CallAutomation.Extensions.Interfaces;

public interface ICanPlayMedia
{
    IPlayMediaCallback ToParticipant<T>(string id)
        where T : CommunicationIdentifier;

    ValueTask ExecuteAsync();
}