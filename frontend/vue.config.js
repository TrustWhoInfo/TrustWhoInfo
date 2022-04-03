module.exports = {
    devServer: {
      proxy: {
        '^/api': {
          target: 'https://localhost:7071',
          changeOrigin: true
        },
      }
    }
  }