mergeInto(LibraryManager.library, {
    // This is called from Unity and sets a connection between functions on the Managed C# side
    // and JavaScript
    SetCallbacks: function(RelayMessagePtr, OnConnectedPtr) {
        console.log("Setting callback references");
        Module.PosBusRelay.RelayMessagePtr = RelayMessagePtr
        Module.PosBusRelay.OnConnectedPtr = OnConnectedPtr
    }
});