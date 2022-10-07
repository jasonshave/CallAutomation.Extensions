// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication;

namespace JasonShave.Azure.Communication.CallAutomation.Extensions.Models
{
    [Serializable]
    public sealed class IncomingCall
    {
        //public CommunicationIdentifier To { get; set; } = default!;

        //public CommunicationIdentifier From { get; set; } = default!;

        public string CallerDisplayName { get; set; } = default!;

        public string IncomingCallContext { get; set; } = default!;

        public string CorrelationId { get; set; } = default!;
    }
}
