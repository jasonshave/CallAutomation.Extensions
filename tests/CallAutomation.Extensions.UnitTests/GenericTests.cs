// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using AutoFixture;
using Azure.Communication;
using Azure.Communication.CallAutomation;
using CallAutomation.Contracts;
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
                    .WithContext(new CustomContext("abc123"))
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

        await client.Call(new CallTarget(new CommunicationUserIdentifier(""))).From("").WithCallbackUri("").ExecuteAsync();
    }

    public class CustomContext : OperationContext
    {
        public override string RequestId { get; }

        public CustomContext(string requestId) => RequestId = requestId;
    }
}
