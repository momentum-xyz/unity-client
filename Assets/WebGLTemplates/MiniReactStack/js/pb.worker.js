'use strict';

importScripts('polyfill.js');
importScripts('wasm_exec.js');

let wasmInstance;
// let wasmModule;
let wasmInitialized = false;

const initOK = {type: 'INIT', result: true};

const go = new Go();

var posbus = {};

posbus.receive = ev => {
	postMessage({
		type: 'WS_MSG',
		msg: ev.data
	});
}

onmessage = async ev => {
	if (ev.data.cmd) {
		switch (ev.data.cmd) {
			case 'INIT':
				if (wasmInitialized) break;
				console.log('Initializing wasm');
				WebAssembly.instantiateStreaming(fetch('main.wasm'), go.importObject)
					.then(result => {
						wasmInstance = result.instance;
						// wasmModule = result.module; // TODO: do we need ?
						wasmInitialized = true;
						console.log('Wasm initialized');
						go.run(wasmInstance)
							.then(res => console.log(res))
							.catch(err => console.error(err));
						postMessage(initOK);
					})
					.catch(err => console.error(err));
				break;
			case 'CONNECT':
				console.log('Received Connect command');
				let connected = posbus.makeWSConnection(ev.data.url, ev.data.token);
				console.log("Connected: ", connected);
				if (!connected) {
					postMessage({type: 'CONNECT', result: false});
					postMessage('DISCONNECTED');
					break;
				}
				postMessage({type: 'CONNECT', result: true});
				break;
			case 'SEND':
				let result = posbus.sendBytes(ev.data.bytes);
				break;
			case 'INTERACTION':
				console.log('Interacting with posbus controller', ev.data);
				let res = posbus.triggerInteraction(
					ev.data.args.type,
					ev.data.args.target,
					ev.data.args.flag,
					ev.data.args.message,
				);
				console.log("Interaction result: ", res);
				postMessage({type: 'INTERACTION', result: res});
				break;
		}
	}
}
