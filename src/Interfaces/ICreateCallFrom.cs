// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

namespace CallAutomation.Extensions.Interfaces;

public interface ICreateCallFrom
{
    /// <summary>
    /// Defines which identity to use as the source of the call.
    /// </summary>
    /// <param name="applicationId"></param>
    /// <returns></returns>
    ICreateCallWithCallbackUri From(string applicationId);
}