Module['UnityAPI'] = Module['UnityAPI'] || {}

Module['UnityAPI'].triggerInteractionMsg = function(kind, guid, flag, message) {

}

Module['UnityAPI'].getUserPosition = function() {
    let userPositionPtr = Module.dynCall_i(Module['UnityAPI'].getUserPositionPtr)
    return Pointer_stringify(userPositionPtr);
}

Module['UnityAPI'].getCurrentWorld = function() {
    let worldIdPtr = Module.dynCall_i(Module['UnityAPI'].getCurrentWorldPtr)
    return Pointer_stringify(worldIdPtr);
}

Module['UnityAPI'].setToken = function(token) {
  var strBufferSize = lengthBytesUTF8(token) + 1;
  var strBuffer = Module._malloc(strBufferSize);
  stringToUTF8(token, strBuffer, strBufferSize);
  Module.dynCall_vi(Module.UnityAPI.setTokenPtr, strBuffer);
  Module._free(strBuffer);
}

Module['UnityAPI'].pauseUnity = function(isPaused) {

}

Module['UnityAPI'].controlSound = function(isOn) {

}

Module['UnityAPI'].controlVolume = function(gain) {
  var strBufferSize = lengthBytesUTF8(gain) + 1;
  var strBuffer = Module._malloc(strBufferSize);
  stringToUTF8(gain, strBuffer, strBufferSize);
  Module.dynCall_vi(Module.UnityAPI.controlVolumePtr, strBuffer);
  Module._free(strBuffer);
}

Module['UnityAPI'].controlKeyboard = function(unityIsInControl) {

}

Module['UnityAPI'].lookAtWisp = function(userGuid) {
  var strBufferSize = lengthBytesUTF8(userGuid) + 1;
  var strBuffer = Module._malloc(strBufferSize);
  stringToUTF8(userGuid, strBuffer, strBufferSize);
  Module.dynCall_vi(Module.UnityAPI.lookAtWispPtr, strBuffer);
  Module._free(strBuffer);
}

Module['UnityAPI'].teleportToSpace = function(spaceGuid) {
  var strBufferSize = lengthBytesUTF8(spaceGuid) + 1;
  var strBuffer = Module._malloc(strBufferSize);
  stringToUTF8(spaceGuid, strBuffer, strBufferSize);
  Module.dynCall_vi(Module.UnityAPI.teleportToSpacePtr, strBuffer);
  Module._free(strBuffer);
}

Module['UnityAPI'].teleportToVector3 = function(x,y,z) {
    Module.dynCall_vfff(Module.UnityAPI.teleportToVector3Ptr, x,y,z);
}

Module['UnityAPI'].teleportToUser = function(userGuid) {
    console.log('teleporting to user: '+userGuid);
  var strBufferSize = lengthBytesUTF8(userGuid) + 1;
  var strBuffer = Module._malloc(strBufferSize);
  stringToUTF8(userGuid, strBuffer, strBufferSize);
  Module.dynCall_vi(Module.UnityAPI.teleportToUserPtr, strBuffer);
  Module._free(strBuffer);
}

Module['UnityAPI'].toggleMinimap = function() {
    Module.dynCall_v(Module.UnityAPI.toggleMinimapPtr);
}




