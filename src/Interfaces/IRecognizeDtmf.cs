// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication;

namespace CallAutomation.Extensions.Interfaces;

public interface IRecognizeDtmf
{
    ICanChooseRecognizeOptions FromParticipant<T>(string id)
        where T : CommunicationIdentifier;
}