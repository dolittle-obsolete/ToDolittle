/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using Dolittle.Booting;
using Dolittle.Interaction.WebAssembly.Interop;

namespace Client
{

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start Dolittle");
            var bootResult = Bootloader.Configure(_ => _
                .WithEntryAssembly(typeof(Program).Assembly)
                .WithAssembliesSpecifiedIn(typeof(Program).Assembly)
                .SynchronousScheduling()
                .NoLogging()
                //.UseLogAppender(new CustomLogAppender())
            ).Start();

            Client.Glue.Bindings.Interop = bootResult.Container.Get<IJSRuntime>();
            Client.Glue.Bindings.Interop.Invoke("window._dolittleLoaded");
        }
    }
}