typedef const char * (*callback_GetWorldID)();
typedef const char * (*callback_GetUserPosition)();
typedef const char * (*callback_GetVolumeMode)();
typedef void (*callback_SetIntState)(const char * guid, const char * label, int value);
typedef int (*callback_GetIntState)(const char * guid, const char * label);
typedef void (*callback_SetStrState)(const char * guid, const char * label, char * value);
typedef char * (*callback_GetStrState)(const char * guid, const char * label);
typedef void (*callback_TriggerInteractionMsg)(int kind, const char * guid, int flag, const char * message);