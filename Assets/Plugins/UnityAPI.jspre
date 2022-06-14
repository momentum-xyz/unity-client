Module['UnityAPI'] = Module['UnityAPI'] || {}

Module['UnityAPI'].triggerInteractionMsg = function(kind, guid, flag, message) {

}

Module['UnityAPI'].getUserPositionCallback = function() {
    console.log(Module['UnityAPI'].getUserPositionPtr);
    return "";
}

Module['UnityAPI'].getCurrentWorldCallback = function() {

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
  var strBufferSize = lengthBytesUTF8(userGuid) + 1;
  var strBuffer = Module._malloc(strBufferSize);
  stringToUTF8(userGuid, strBuffer, strBufferSize);
  Module.dynCall_vi(Module.UnityAPI.teleportToUserPtr, strBuffer);
  Module._free(strBuffer);
}

Module['UnityAPI'].toggleMinimap = function() {

}




