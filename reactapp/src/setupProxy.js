const { createProxyMiddleware } = require('http-proxy-middleware');

const context = [
    "/getgame",
    "/creategame"
];

module.exports = function (app) {
    const appProxy = createProxyMiddleware(context, {
        target: 'https://localhost:7071',
        secure: false
    });

    app.use(appProxy);
};
