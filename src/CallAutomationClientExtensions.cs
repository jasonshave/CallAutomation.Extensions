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
        /// <summary>
        /// Initiates the outbound call sequence.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="id"></param>
        public static ICreateCallFromWithHandler Call(this CallAutomationClient client, string id)
        {
            var helper =
                new CallAutomationCreateCallHelper(client, id, Guid.NewGuid().ToString());
            return helper;
        }

        /// <summary>
        /// Initiates the answer call sequence.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="incomingCall"></param>
        /// <returns></returns>
        public static IAnswerWithCallbackUriWithHandler Answer(this CallAutomationClient client, IncomingCall incomingCall)
        {
            var helper = new CallAutomationAnswerHelper(client, incomingCall, incomingCall.CorrelationId);
            return helper;
        }

        /// <summary>
        /// Initiates the answer call sequence.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="callNotification"></param>
        /// <returns></returns>
        public static IAnswerCallHandling Answer(this CallAutomationClient client, CallNotification callNotification)
        {
            var helper = new CallAutomationAnswerHelper(client, callNotification, callNotification.CorrelationId);
            return helper;
        }

        /// <summary>
        /// Initiates the reject call sequence.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="incomingCall"></param>
        /// <returns></returns>
        public static IRejectCallWithReason Reject(this CallAutomationClient client, IncomingCall incomingCall)
        {
            var helper = new CallAutomationRejectHelper(client, incomingCall);
            return helper;
        }

        /// <summary>
        /// Initiates the redirect call sequence.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="incomingCall"></param>
        /// <returns></returns>
        public static IRedirectCall Redirect(this CallAutomationClient client, IncomingCall incomingCall)
        {
            var helper = new CallAutomationRedirectHelper(client, incomingCall);
            return helper;
        }
    }
}