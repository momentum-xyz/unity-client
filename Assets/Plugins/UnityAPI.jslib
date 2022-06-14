mergeInto(LibraryManager.library, {
    SetCallbacks: function(
                getUserPositionPtr, 
                getCurrentWorldPtr, 
                setTokenPtr, 
                pauseUnityPtr, 
                controlSoundPtr, 
                controlVolumePtr, 
                controlKeyboardPtr, 
                lookAtWispPtr, 
                toggleMinimapPtr, 
                teleportToSpacePtr, 
                teleportToUserPtr, 
                teleportToVector3Ptr, 
                triggerInteractionMsgPtr 
        ) {
        Module.UnityAPI.getUserPositionPtr = getUserPositionPtr
        Module.UnityAPI.getCurrentWorldPtr = getCurrentWorldPtr
        Module.UnityAPI.setTokenPtr = setTokenPtr
        Module.UnityAPI.pauseUnityPtr = pauseUnityPtr
        Module.UnityAPI.controlSoundPtr = controlSoundPtr
        Module.UnityAPI.controlVolumePtr = controlVolumePtr
        Module.UnityAPI.controlKeyboardPtr = controlKeyboardPtr
        Module.UnityAPI.lookAtWispPtr = lookAtWispPtr
        Module.UnityAPI.toggleMinimapPtr = toggleMinimapPtr
        Module.UnityAPI.teleportToSpacePtr = teleportToSpacePtr
        Module.UnityAPI.teleportToVector3Ptr = teleportToVector3Ptr
        Module.UnityAPI.teleportToUserPtr = teleportToUserPtr
        Module.UnityAPI.triggerInteractionMsgPtr = triggerInteractionMsgPtr
    }
});