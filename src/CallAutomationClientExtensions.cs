// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;
using CallAutomation.Contracts;
using CallAutomation.Extensions.Helpers;
using CallAutomation.Extensions.Interfaces;
using CallAutomation.Extensions.Models;

namespace CallAutomation.Extensions
{
    public static class CallAutomationClientExtensions
    {
        /// <summary>
        /// Creates a call to a <see cref="CallTarget"/>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="callTarget"></param>
        /// <returns></returns>
        public static ICreateCallFrom Call(this CallAutomationClient client, CallTarget callTarget)
        {
            var helper = new CallAutomationCreateCallHelper(client, callTarget);
            return helper;
        }

        /// <summary>
        /// Answers an <see cref="IncomingCall"/>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="incomingCall"></param>
        /// <returns></returns>
        public static IAnswerWithCallbackUri Answer(this CallAutomationClient client, IncomingCall incomingCall)
        {
            var helper = new CallAutomationAnswerHelper(client, incomingCall);
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
            var helper = new CallAutomationAnswerHelper(client, callNotification);
            return helper;
        }

        /// <summary>
        /// Rejects an <see cref="IncomingCall"/>
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
        /// Redirects an <see cref="IncomingCall"/>
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