// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using AutoFixture;
using Azure.Communication.CallAutomation;
using CallAutomation.Contracts;
using CallAutomation.Extensions.Interfaces;

namespace CallAutomation.Extensions.UnitTests;

public class GenericTests
{
    [Fact]
    public async Task Test1()
    {
        var fixture = new Fixture();
        var incomingCall = fixture.Create<IncomingCall>();
        var client = new CallAutomationClient("");

        await client.Answer(incomingCall)
            .WithCallbackUri("")
            .OnCallConnected(async (@event, callConnection, callMedia, callRecording) =>
            {
                await callConnection.AddParticipant("")
                    .WithOptions(x => x.InvitationTimeoutInSeconds = 1)

                    .OnAddParticipantsSucceeded(() => ValueTask.CompletedTask)
                    .ExecuteAsync();
            })
            .ExecuteAsync();
    }

    public class CustomContext : IOperationContext
    {
        public string? RequestId { get; set; }
    }
}