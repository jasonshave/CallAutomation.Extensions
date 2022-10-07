// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication;
using JasonShave.Azure.Communication.CallAutomation.Extensions.Models;

namespace JasonShave.Azure.Communication.CallAutomation.Extensions.Interfaces;

public interface IRedirectToParticipant
{
    IRedirectCall ToParticipant<T>(string id)
        where T : CommunicationUserIdentifier;

    IRedirectCall ToParticipant<T>(string id, Action<PstnParticipantOptions> options)
        where T : PhoneNumberIdentifier;

    ValueTask ExecuteAsync();
}