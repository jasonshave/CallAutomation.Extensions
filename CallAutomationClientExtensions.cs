// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;
using JasonShave.Azure.Communication.Service.CallAutomation.Extensions.Helpers;
using JasonShave.Azure.Communication.Service.CallAutomation.Extensions.Interfaces;
using JasonShave.Azure.Communication.Service.CallAutomation.Sdk.Contracts;

namespace JasonShave.Azure.Communication.Service.CallAutomation.Extensions
{
    public static class CallAutomationClientExtensions
    {
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