Module['PosBusRelay'] = Module['PosBusRelay'] || {};

Module['PosBusRelay'].RelayMessageToUnity = function(data) {

    if(data instanceof ArrayBuffer) {
        var dataBuffer = new Uint8Array(data);
				
		var buffer = Module._malloc(dataBuffer.length);
		Module.HEAPU8.set(dataBuffer, buffer);

        try {
			Module.dynCall_vii(Module.PosBusRelay.RelayMessageToUnityPtr,buffer, dataBuffer.length);
		} finally {
		    Module._free(buffer);
		}
    }
}

Module['PosBusRelay'].OnConnected  = function(guid) {

    var strBufferSize = lengthBytesUTF8(guid) + 1;
    var strBuffer = Module._malloc(strBufferSize);
    stringToUTF8(guid, strBuffer, strBufferSize);
    
    try {
        Module.dynCall_vi(Module.PosBusRelay.OnConnectedPtr, strBuffer);
    } finally {
        Module._free(strBuffer);
    }
}