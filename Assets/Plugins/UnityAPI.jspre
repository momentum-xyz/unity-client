
class Helpers {
    static stringToPointer(text) {

        const bufferSize = lengthBytesUTF8(text) + 1;
        const buffer = _malloc(bufferSize);
        stringToUTF8(text, buffer, bufferSize);

        return buffer;
    }
}

class UnityAPI {

    triggerInteractionMsg(kind, guid, flag, message) {

        var guidPtr = Helpers.stringToPointer(guid);
        var messagePtr = Helpers.stringToPointer(message);

        Module.dynCall_viiii(Module.UnityAPI.triggerInteractionMsgPtr, kind, guidPtr, flag, messagePtr);

        Module._free(guidPtr);
        Module._free(messagePtr);
    }

    getUserPosition() {
        let userPositionPtr = Module.dynCall_i(Module['UnityAPI'].getUserPositionPtr)
        return UTF8ToString(userPositionPtr);
    }

    getCurrentWorld() {
        let worldIdPtr = Module.dynCall_i(Module['UnityAPI'].getCurrentWorldPtr)
        return UTF8ToString(worldIdPtr);
    }

    setToken(token) {
        var tokenPtr = Helpers.stringToPointer(token);
        Module.dynCall_vi(Module.UnityAPI.setTokenPtr, tokenPtr);
        Module._free(tokenPtr);
    }

    pauseUnity(isPaused) {
        Module.dynCall_vi(Module.UnityAPI.pauseUnityPtr, isPaused ? 1 : 0);
    }

    controlSound(isOn) {
        Module.dynCall_vi(Module.UnityAPI.controlSoundPtr, isOn ? 1 : 0);
    }

    controlVolume(gain) {
        var gainPtr = Helpers.stringToPointer(gain);
        Module.dynCall_vi(Module.UnityAPI.controlVolumePtr, gainPtr);
        Module._free(gainPtr);
    }

    controlKeyboard(unityIsInControl) {
        Module.dynCall_vi(Module.UnityAPI.controlKeyboardPtr, unityIsInControl ? 1 : 0);
    }

    lookAtWisp(userGuid) {
        var userGuidPtr = Helpers.stringToPointer(userGuid);
        Module.dynCall_vi(Module.UnityAPI.lookAtWispPtr, userGuid);
        Module._free(userGuid);
    }

    teleportToSpace(spaceGuid) {
        var spaceGuidPtr = Helpers.stringToPointer(spaceGuid);
        Module.dynCall_vi(Module.UnityAPI.teleportToSpacePtr, spaceGuidPtr);
        Module._free(spaceGuidPtr);
    }

    teleportToVector3(positionString) {
        var tmp = positionString.substr(1,positionString.length-1).split(',');
        Module.dynCall_vfff(Module.UnityAPI.teleportToVector3Ptr, parseFloat(tmp[0]), parseFloat(tmp[1]), parseFloat(tmp[2]));
    }

   teleportToUser(userGuid) {
        var userGuidPtr = Helpers.stringToPointer(userGuid);
        Module.dynCall_vi(Module.UnityAPI.teleportToUserPtr, userGuidPtr);
        Module._free(userGuidPtr);
    }

   toggleMinimap() {
        Module.dynCall_v(Module.UnityAPI.toggleMinimapPtr);
    }
}

Module['UnityAPI'] = new UnityAPI();



