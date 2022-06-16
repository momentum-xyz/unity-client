// cmd enum
const cmd = {
	INIT: 'INIT',
	CONNECT: 'CONNECT',
	DISCONNECT: 'DISCONNECT',
	INTERACTION: 'INTERACTION',
	LOG: 'LOG',
	SEND: 'SEND',
	WS_MSG: 'WS_MSG',
};

const interactions = {
	HIGH_FIVE: 2,
	WOW: 3,
}

let pbworker;

document.PB = document.PB || {}

if (!window || !window.Worker) {
	console.log('Workers are not supported in this browser.');
} else {

    document.PB.Connect = function() {
        console.log("Connecting to worker..");

        var decoded = decodeToken(getToken());
        console.log(decoded);

        // Connect to WS
        document.PB.pbworker.postMessage({
            cmd: cmd.CONNECT,
            url: 'wss://dev.odyssey.ninja/posbus',
            token: getToken(),
        });
    }

    document.PB.SendMsg = function(data) {
        console.log("Sending..");
        console.log(data);

        document.PB.pbworker.postMessage({
            cmd: cmd.SEND,
            bytes: data
        });
    }

	document.PB.pbworker = new Worker('/wasm/pb.worker.js');

	document.PB.pbworker.postMessage({cmd: cmd.INIT});

	document.PB.pbworker.onmessage = e => {
		if (e.data.type) {
			switch (e.data.type) {
				case cmd.INIT:
                    console.log("worker: INITIALIZED!")
					break;
				case cmd.CONNECT:
                    console.log(e);
					if (e.data.result) {
						console.log('worker: connected');
                        window.unityInstance.Module.PosBusRelay.OnConnected("86853efb-1051-4a31-8ca2-d3948a0a81b3");  
					} else {
						console.log('connection failed');
					}
					break;
				case cmd.SEND:
					console.log('message sent');
					break;
				case cmd.WS_MSG:
				//	console.log('message from WS: ', e.data); // Blob
                    e.data.msg.arrayBuffer().then(function(a) {
                        window.unityInstance.Module.PosBusRelay.RelayMessageToUnity(a)
                    });
                    
					// TODO: handle message
					break;
				case 'DISCONNECTED':
					console.log('WS Disconnected.. terminating worker');
					document.PB.pbworker.terminate();
					break;
				default:
					break;
			}
		}
	}
}


// TODO: remove this
function getToken() {
	return authToken;
}

function decodeToken(token) {
    var split = token.split('.');

    var decode = split[1];
    var padLength = 4 - decode.length % 4
    
    if(padLength < 4) {
        for(var i=0; i < padLength; ++i) {
            decode += "="
        }
    }
    
    var json = JSON.parse(atob(decode))

    return json;
}
