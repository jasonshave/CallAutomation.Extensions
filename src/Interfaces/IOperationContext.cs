// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

namespace CallAutomation.Extensions.Interfaces
{
    public interface IOperationContext
    {
        /// <summary>
        /// Unique identifier for the Request
        /// </summary>
        public string RequestId { get; }

        public string Payload { get; }

        public Type PayloadType { get; }
    }
}
