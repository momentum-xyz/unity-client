'use strict';

const express = require('express');
const app = express();
const port = 4040;


const contetTypesFromExtension = {
    ".data.gz": "application/octet-stream",
    ".wasm.gz": "application/wasm",
    ".js.gz": "application/javascript",
    ".symbols.json.gz": "application/octet-stream",
    ".data.br": "application/octet-stream",
    ".wasm.br": "application/wasm",
    ".js.br": "application/javascript",
    ".symbols.json.br": "application/octet-stream",
}

app.use(function (req, res, next) {
    Object.entries(contetTypesFromExtension).forEach(([ext, contetType]) => {
        if (req.path.includes(ext)) {
            res.header("Content-Type", contetType);
        }
    });

    if (req.path.includes('.gz')) {
        res.header("Content-Encoding", "gzip")
    }
    if (req.path.includes('.br')) {
        res.header("Content-Encoding", "br")
    }

    next();
})
app.use(express.static('build'));

app.listen(port, function () {
    console.log("Listening on port " + port);
});