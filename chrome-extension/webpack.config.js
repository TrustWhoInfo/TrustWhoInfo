const path = require('path');

module.exports = {
  entry: './src/content.js',
  mode: "production",
  output: {
    path: path.resolve(__dirname, 'public'),
    filename: 'content.js',
  },
};