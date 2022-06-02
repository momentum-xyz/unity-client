using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using System;
using AOT;

namespace Odyssey.Networking
{
    public interface IPosBusRelay
    {
        public Action<string> OnPosBusConnected { get; set; }
        public Action<byte[]> OnPosBusMessage { get; set; }
        public void Init();
        public void SendMsg(byte[] data, int length);
    }

    // The class containing the callbacks, we are going to reference from JavaScript must be static!
    public static class NativePosBusRelay
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        public static PosBusRelay Instance = null;

        public delegate void RelayMessageToUnityCallback(IntPtr msgPtr, int msgSize);
        public delegate void OnConnectedCallback(IntPtr guidPtr);

        [DllImport("__Internal")]
        public static extern void SetCallbacks(RelayMessageToUnityCallback callback, OnConnectedCallback connectedCallback);

        [DllImport("__Internal")]
        public static extern void RelayMsgToPosBus(byte[] dataPtr, int dataLength);

        [MonoPInvokeCallback(typeof(RelayMessageToUnityCallback))]
        public static void RelayMessageToUnity(IntPtr data, int size)
        {

            Debug.Log("Got Message with size: " + size);

            byte[] msgInBytes = new byte[size];
            Marshal.Copy(data, msgInBytes, 0, size);

            Instance.OnPosBusMessage?.Invoke(msgInBytes);

        }

        [MonoPInvokeCallback(typeof(OnConnectedCallback))]
        public static void OnConnected(IntPtr guidPtr)
        {
            Debug.Log("Connected...");
            string guid = Marshal.PtrToStringAuto(guidPtr);
            Instance.OnPosBusConnected?.Invoke(guid);
        }
#endif
    }

    public class PosBusRelay : IPosBusRelay
    {
        public Action<string> OnPosBusConnected { get; set; }
        public Action<byte[]> OnPosBusMessage { get; set; }

        public void Init()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            NativePosBusRelay.Instance = this;
            NativePosBusRelay.SetCallbacks(NativePosBusRelay.RelayMessageToUnity, NativePosBusRelay.OnConnected);
#endif
        }

        public void SendMsg(byte[] data, int length)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            NativePosBusRelay.RelayMsgToPosBus(data, length);
#endif
        }

    }
}
