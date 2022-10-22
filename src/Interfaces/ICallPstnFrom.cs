// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication;

namespace CallAutomation.Extensions.Interfaces;

public interface ICallPstnFrom
{
    ICallPstnWithOverride From<T>(string phoneNumber)
        where T : PhoneNumberIdentifier;
}

public interface ICallPstnWithOverride : ICallWithCallbackUri
{
    ICallWithCallbackUri AsCommunicationUser(string communicationUserId);
}