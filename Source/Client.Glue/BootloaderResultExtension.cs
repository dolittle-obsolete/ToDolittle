/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Booting;
using Dolittle.Interaction.WebAssembly.Interop;

namespace Dolittle.Booting
{
    public static class BootloaderResultExtension
    {
        public static BootloaderResult Started(this BootloaderResult bootResult)
        {
            Client.Glue.Bindings.Interop = bootResult.Container.Get<IJSRuntime>();
            Client.Glue.Bindings.Interop.Invoke("window._dolittleLoaded");
            return bootResult;
        }
    }
}