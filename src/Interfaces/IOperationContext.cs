// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallAutomation.Extensions.Interfaces
{
    public interface IOperationContext
    {
        /// <summary>
        /// Unique identifier for the Request
        /// </summary>
        public string? RequestId { get; set; }
    }
}
