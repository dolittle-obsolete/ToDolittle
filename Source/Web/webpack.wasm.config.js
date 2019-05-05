const path = require('path');
require('dotenv').config();
const CopyWebpackPlugin = require('copy-webpack-plugin');
const { ServiceWorkerGenerator } = require('@dolittle/webassembly.webpack');
const HtmlWebpackPlugin = require('html-webpack-plugin');

process.env.DOLITTLE_WEBPACK_ROOT = path.resolve('../Core');
process.env.DOLITTLE_WEBPACK_OUT = path.resolve('../Core/wwwroot');
process.env.DOLITTLE_FEATURES_DIR = path.resolve('./Features');
process.env.DOLITTLE_COMPONENT_DIR = path.resolve('./Components');

const config = require('@dolittle/build.aurelia/webpack.config.js')();

let plugins = config.plugins.filter(_ => _ instanceof HtmlWebpackPlugin);
if( plugins && plugins.length == 1 ) plugins[0].options.metadata.wasm = true;

config.plugins.push(new CopyWebpackPlugin([
    { from: '../Client/publish/managed/**/*', to: 'managed', flatten: true },
    { from: '../Client/publish/mono.js', to: 'mono.js', flatten: true },
    { from: '../Client/publish/mono.wasm', to: 'mono.wasm', flatten: true },
    { from: './manifest.json', to: 'manifest.json', flatten: true },
    { from: './dolittle192.png', to: 'dolittle192.png', flatten: true },
    { from: './dolittle512.png', to: 'dolittle512.png', flatten: true }
]));

config.plugins.push(new ServiceWorkerGenerator({
    assembliesFileFolder: path.join(process.cwd(), '../Client/publish'),
    outputFolder: path.join(process.cwd(), '../Core/wwwroot')
}));

module.exports = config;
