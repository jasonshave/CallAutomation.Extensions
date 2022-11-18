// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

namespace CallAutomation.Extensions.Interfaces;

public interface IRedirectCall
{
    /// <summary>
    /// Redirects a call to an identity.
    /// </summary>
    /// <param name="rawId"></param>
    /// <returns></returns>
    IExecuteAsync ToParticipant(string rawId);
}