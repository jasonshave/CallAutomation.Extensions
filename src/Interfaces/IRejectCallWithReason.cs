﻿// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;

namespace CallAutomation.Extensions.Interfaces;

public interface IRejectCallWithReason
{
    IRejectCall WithReason(CallRejectReason reason);
}