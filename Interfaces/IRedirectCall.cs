﻿// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication;
using JasonShave.Azure.Communication.Service.CallAutomation.Extensions.Models;

namespace JasonShave.Azure.Communication.Service.CallAutomation.Extensions.Interfaces;

public interface IRedirectCall : ICanExecuteAsync
{
    ICanExecuteAsync ToParticipant<T>(string id)
        where T : CommunicationUserIdentifier;

    ICanExecuteAsync ToParticipant<T>(string id, Action<PstnParticipantOptions> options)
        where T : PhoneNumberIdentifier;
}