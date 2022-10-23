// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication;

namespace CallAutomation.Extensions.Interfaces;

public interface ICallPstnFrom
{
    ICreateCallPstnWithOverride From<T>(string phoneNumber)
        where T : PhoneNumberIdentifier;
}

public interface ICreateCallPstnWithOverride : ICreateCallWithCallbackUri
{
    ICreateCallWithCallbackUri AsCommunicationUser(string communicationUserId);
}