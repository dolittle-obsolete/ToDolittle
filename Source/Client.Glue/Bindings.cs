/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.DependencyInversion;
using Dolittle.Interaction.WebAssembly.Interop;
using Read.TodoItem;

namespace Client.Glue
{
    public class Bindings : ICanProvideBindings
    {
        public static IJSRuntime Interop;
        public void Provide(IBindingProviderBuilder builder)
        {
            builder.Bind<ListUpdated>().To(list => Interop.Invoke("window._listUpdated", list));
        }
    }
}