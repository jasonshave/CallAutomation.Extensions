// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;
using CallAutomation.Contracts;
using CallAutomation.Extensions.Helpers;
using CallAutomation.Extensions.Interfaces;

namespace CallAutomation.Extensions
{
    public static class CallAutomationClientExtensions
    {
        public static ICreateCallFrom Call(this CallAutomationClient client, string id)
        {
            var helper =
                new CallAutomationCreateCallHelper(client, id, Guid.NewGuid().ToString());
            return helper;
        }

        public static IAnswerWithCallbackUri Answer(this CallAutomationClient client, IncomingCall incomingCall)
        {
            var helper = new CallAutomationAnswerHelper(client, incomingCall, incomingCall.CorrelationId);
            return helper;
        }

        public static IRejectCallWithReason Reject(this CallAutomationClient client, IncomingCall incomingCall)
        {
            var helper = new CallAutomationRejectHelper(client, incomingCall);
            return helper;
        }

        public static IRedirectCall Redirect(this CallAutomationClient client, IncomingCall incomingCall)
        {
            var helper = new CallAutomationRedirectHelper(client, incomingCall);
            return helper;
        }
    }
}