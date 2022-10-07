// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;

namespace JasonShave.Azure.Communication.CallAutomation.Extensions.Interfaces;

public interface IRejectCallWithReason
{
    IRejectCall WithReason(CallRejectReason reason);
}