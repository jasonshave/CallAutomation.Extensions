// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;

namespace CallAutomation.Extensions.Interfaces;

public interface ICallAutomationClientElements
{
    CallAutomationClient Client { get; }

    CallConnection CallConnection { get; }

    CallMedia CallMedia { get; }

    CallRecording CallRecording { get; }
}