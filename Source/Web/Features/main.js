import environment from './environment';
import { PLATFORM } from 'aurelia-pal';
import * as Bluebird from 'bluebird';

import * as assemblies from '../../Client/publish/assemblies.json';

// remove out if you don't want a Promise polyfill (remove also from webpack.config.js)
Bluebird.config({ warnings: { wForgottenReturn: false } });

export function configure(aurelia) {
   
    aurelia.use
        .standardConfiguration()

    if( window._wasmEnabled === true ) {
        aurelia.use.plugin(PLATFORM.moduleName('@dolittle/webassembly.aurelia'), {
            entryPoint: "[Client] Client.Program:Main",
            assemblies: assemblies.default,
            offline: false
        });
        window.print = (message) => {
            console.log(message);
        };   
    }

    if (environment.debug) {
        aurelia.use.developmentLogging();
    }
    aurelia.start().then(() => aurelia.setRoot(PLATFORM.moduleName('app')));

}
