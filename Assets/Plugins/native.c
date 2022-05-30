#include <stdint.h>
#include "emscripten.h"
#include "native.h"
#include <stdio.h>

callback_GetWorldID cb_GetWorldID;
callback_GetUserPosition cb_GetUserPosition;
callback_GetVolumeMode cb_GetVolumeMode;
callback_SetIntState cb_SetIntState;
callback_GetIntState cb_GetIntState;
callback_SetStrState cb_SetStrState;
callback_GetStrState cb_GetStrState;
callback_TriggerInteractionMsg cb_TriggerInteractionMsg;

void set_callbacks(
	callback_GetWorldID p_GetWorldID,
	callback_GetUserPosition p_GetUserPosition,
	callback_GetVolumeMode p_GetVolumeMode,
	callback_TriggerInteractionMsg p_TriggerInteractionMsg
)
{
	cb_GetWorldID = p_GetWorldID;
	cb_GetUserPosition = p_GetUserPosition;
	cb_GetVolumeMode = p_GetVolumeMode;
	cb_TriggerInteractionMsg = p_TriggerInteractionMsg;
}

void set_stateCallbacks(
	callback_SetIntState p_SetIntState,
	callback_GetIntState p_GetIntState,
	callback_SetStrState p_SetStrState,
	callback_GetStrState p_GetStrState
)
{
	cb_SetIntState = p_SetIntState;
	cb_GetIntState = p_GetIntState;
	cb_SetStrState = p_SetStrState;
	cb_GetStrState = p_GetStrState;
}

const char * EMSCRIPTEN_KEEPALIVE extGetCurrentWorld () {
	return cb_GetWorldID();
}

const char * EMSCRIPTEN_KEEPALIVE extGetUserPosition () {
	return cb_GetUserPosition();
}

const char * EMSCRIPTEN_KEEPALIVE extGetVolumeMode () {
	return cb_GetVolumeMode();
}

void EMSCRIPTEN_KEEPALIVE extSetIntState(const char * guid, const char * label, int value) {
	cb_SetIntState(guid, label, value);
}

int EMSCRIPTEN_KEEPALIVE extGetIntState(const char * guid, const char * label) {
	return cb_GetIntState(guid, label);
}

void EMSCRIPTEN_KEEPALIVE extSetStrState(const char * guid, const char * label, char * value) {
	cb_SetStrState(guid, label, value);
}

char * EMSCRIPTEN_KEEPALIVE extGetStrState(const char * guid, const char * label) {
	return cb_GetStrState(guid, label);
}

void EMSCRIPTEN_KEEPALIVE extTriggerInteractionMsg(int kind, const char * guid, int flag, const char * message) {
	cb_TriggerInteractionMsg(kind, guid, flag, message);
}