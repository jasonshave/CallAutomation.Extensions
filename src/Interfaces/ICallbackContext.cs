// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallAutomation.Extensions.Interfaces
{
    public interface ICallbackContext<T>
    {
        /// <summary>
        /// Setting custom context.
        /// </summary>
        /// <param name="context"></param>
        /// <returns><see cref="ICallbackContext"/></returns>
        T WithContext(IOperationContext context);
    }
}
