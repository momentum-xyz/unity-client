using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using System;
using AOT;

namespace Odyssey.Networking
{
    public interface IReactPosBusClient
    {
        public Action<string> OnPosBusConnected { get; set; }
        public Action<byte[]> OnPosBusMessage { get; set; }
        public void Init();
    }

    // The class containing the callbacks, we are going to reference from JavaScript must be static!
    public static class NativeReactPosBusClient
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        public static ReactPosBusClient Instance = null;

        public delegate void OnMessageCallback(IntPtr msgPtr, int msgSize);
        public delegate void OnConnectedCallback(IntPtr guidPtr);

        [DllImport("__Internal")]
        public static extern void SetCallbacks(OnMessageCallback callback, OnConnectedCallback connectedCallback);

        [MonoPInvokeCallback(typeof(OnMessageCallback))]
        public static void RelayPosBusMessage(IntPtr data, int size)
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

    public class ReactPosBusClient : IReactPosBusClient
    {
        public Action<string> OnPosBusConnected { get; set; }
        public Action<byte[]> OnPosBusMessage { get; set; }

        public void Init()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            NativeReactPosBusClient.Instance = this;
            NativeReactPosBusClient.SetCallbacks(NativeReactPosBusClient.RelayPosBusMessage, NativeReactPosBusClient.OnConnected);
#endif
        }

    }
}
