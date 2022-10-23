// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;

namespace CallAutomation.Extensions.Models;

public sealed class CallAutomationClientElements
{
    private readonly string _callConnectionId;

    public CallAutomationClient Client { get; }

    public CallConnection CallConnection => Client.GetCallConnection(_callConnectionId);

    public CallMedia CallMedia => CallConnection.GetCallMedia();

    public CallRecording CallRecording => Client.GetCallRecording();

    public CallAutomationClientElements(CallAutomationClient client, string callConnectionId)
    {
        _callConnectionId = callConnectionId;
        Client = client;
    }
}