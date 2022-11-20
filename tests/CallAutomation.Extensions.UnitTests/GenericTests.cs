// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using AutoFixture;
using Azure.Communication;
using Azure.Communication.CallAutomation;
using CallAutomation.Contracts;
using CallAutomation.Extensions.Interfaces;
using CallAutomation.Extensions.Models;

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
            .OnCallConnected(async (@event, callConnection, callMedia, callRecording, context) =>
            {
                // with custom context
                await callConnection.AddParticipant("")
                    .WithOptions(x => x.InvitationTimeoutInSeconds = 1)
                    .WithContext(new MyCustomOperationContext())
                    .OnAddParticipantsSucceeded(() => ValueTask.CompletedTask)
                    .ExecuteAsync();

                // or omit context and use built-in DefaultOperationContext implementation
                await callConnection.AddParticipant("")
                    .WithOptions(x => x.InvitationTimeoutInSeconds = 1)
                    .OnAddParticipantsSucceeded(() => ValueTask.CompletedTask)
                    .ExecuteAsync();

                await callMedia.ReceiveDtmfTone()
                    .FromParticipant("")
                    .WithPrompt("")
                    .WithOptions(x => x.AllowInterruptExistingMediaOperation = true)
                    .OnPress<One>(() => ValueTask.CompletedTask)
                    .OnFail<SilenceTimeout>(() => ValueTask.CompletedTask)
                    .ExecuteAsync();
            })
            .ExecuteAsync();

        await client
            .Call(new CallTarget(new CommunicationUserIdentifier("8:acs:guid_guid")))
            .From("appId")
            .WithCallbackUri("")
            .ExecuteAsync();
    }

    public class MyCustomOperationContext : IOperationContext
    {
        public string RequestId { get; } = Guid.NewGuid().ToString();

        public string? Payload => null;

        public string? PayloadType => null;
    }
}
