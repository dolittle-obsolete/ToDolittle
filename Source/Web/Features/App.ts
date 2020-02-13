/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
import { PLATFORM } from 'aurelia-pal';

export class App {
    router : any;

    constructor() {
    }

    configureRouter(config : any, router : any) {
        config.options.pushState = true;
        config.title = 'TodoMVC'
        config.map([
            { route: ['', ':filter'], moduleId: PLATFORM.moduleName('todos') }
        ]);

        this.router = router;
    }
}
