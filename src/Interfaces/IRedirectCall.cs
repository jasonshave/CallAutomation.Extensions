// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

namespace CallAutomation.Extensions.Interfaces;

public interface IRedirectCall : ICanExecuteAsync
{
    /// <summary>
    /// Redirects a call to an identity.
    /// </summary>
    /// <param name="rawId"></param>
    /// <returns></returns>
    ICanExecuteAsync ToParticipant(string rawId);
}