mergeInto(LibraryManager.library, {
    // This is called from Unity and sets a connection between functions on the Managed C# side
    // and JavaScript
    SetCallbacks: function(RelayMessageToUnityPtr, OnConnectedPtr) {
        console.log("Setting callback references");
        Module.PosBusRelay.RelayMessageToUnityPtr = RelayMessageToUnityPtr
        Module.PosBusRelay.OnConnectedPtr = OnConnectedPtr
    },
    RelayMsgToPosBus: function(dataPtr, length) {
        var dataInBytes = Module.HEAPU8.buffer.slice(dataPtr, dataPtr + length)
        console.log(dataInBytes);
    }
});