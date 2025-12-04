const path = require('path');

module.exports = {
    entry: {
        ZXingLib: './src/zxingLib.js',
        ZXingBrowser: './src/zxingBrowser.js',
    },
    output: {
        filename: '[name].bundle.js',
        path: path.resolve(__dirname, '../wwwroot/lib'),
        library: '[name]',
    }
};