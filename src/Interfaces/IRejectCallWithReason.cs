// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;

namespace CallAutomation.Extensions.Interfaces;

public interface IRejectCallWithReason
{
    /// <summary>
    /// Specifies the reason for rejecting the call.
    /// </summary>
    /// <param name="reason"></param>
    /// <returns></returns>
    IRejectCall WithReason(CallRejectReason reason);
}