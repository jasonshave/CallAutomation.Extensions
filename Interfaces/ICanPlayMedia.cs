// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication;

namespace JasonShave.Azure.Communication.Service.CallAutomation.Extensions.Interfaces;

public interface ICanPlayMedia
{
    IPlayMediaCallback ToParticipant<T>(string id)
        where T : CommunicationIdentifier;

    ValueTask ExecuteAsync();
}