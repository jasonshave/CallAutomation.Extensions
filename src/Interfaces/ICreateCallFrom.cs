// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

namespace CallAutomation.Extensions.Interfaces;

public interface ICreateCallFrom
{
    /// <summary>
    /// Defines which identity to use as the source of the call while offering a delegate to change options using <see cref="CallFromOptions"/>
    /// </summary>
    /// <param name="applicationId"></param>
    /// <returns></returns>
    ICreateCallWithCallbackUri From(string applicationId);
}