// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

namespace CallAutomation.Extensions.Interfaces;

public interface ICreateCallWithCallbackUri
{
    /// <summary>
    /// The callback <see cref="Uri"/> is created from the <see cref="string"/> input and used for mid-call event callbacks.
    /// </summary>
    /// <param name="callbackUri"></param>
    ICreateCallHandling WithCallbackUri(string callbackUri);
}