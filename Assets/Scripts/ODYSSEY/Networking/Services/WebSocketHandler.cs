using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Odyssey;
using HybridWebSocket;
using System;

namespace Odyssey.Networking
{
    public enum WebsocketHandlerState
    {
        Open,
        Closed,
        Closing,
        Unknown
    }

    public enum WebsocketHandlerCloseCode
    {
        Normal,
        Abnormal
    }

    public interface IWebsocketsHandler
    {
        public Action OnOpen { get; set; }
        public Action<WebsocketHandlerCloseCode> OnClose { get; set; }
        public Action<string> OnError { get; set; }
        public Action<byte[]> OnMessage { get; set; }
        public bool IsInit { get; set; }
        public void Init(string url);
        public void Dispose();
        public void Connect();
        public void Close();
        public void Send(byte[] data);
        public WebsocketHandlerState GetState();
    }

    public class HybridWS : IWebsocketsHandler
    {
        public Action OnOpen { get; set; }
        public Action<WebsocketHandlerCloseCode> OnClose { get; set; }
        public Action<string> OnError { get; set; }
        public Action<byte[]> OnMessage { get; set; }
        public bool IsInit { get; set; } = false;

        private WebSocket websocket;

        public HybridWS()
        {

        }

        public void Init(string url)
        {
            if (IsInit) return;

            websocket = WebSocketFactory.CreateInstance(url);

            websocket.OnOpen += OnWSOpen;
            websocket.OnError += OnWSError;
            websocket.OnClose += OnWSClose;
            websocket.OnMessage += OnWSMessage;

            IsInit = true;

        }

        public void Dispose()
        {
            websocket.OnOpen -= OnWSOpen;
            websocket.OnError -= OnWSError;
            websocket.OnClose -= OnWSClose;
            websocket.OnMessage -= OnWSMessage;

            IsInit = false;
        }

        public void Close()
        {
            websocket.Close();
        }

        public WebsocketHandlerState GetState()
        {
            switch (websocket.GetState())
            {
                case WebSocketState.Open:
                    return WebsocketHandlerState.Open;
                case WebSocketState.Closing:
                    return WebsocketHandlerState.Closing;
                case WebSocketState.Closed:
                    return WebsocketHandlerState.Closed;
                default:
                    return WebsocketHandlerState.Unknown;

            }
        }

        public void Connect()
        {
            websocket.Connect();
        }

        public void Send(byte[] data)
        {
            websocket.Send(data);
        }

        void OnWSOpen()
        {
            OnOpen?.Invoke();
        }

        void OnWSError(string err)
        {
            OnError?.Invoke(err);
        }

        void OnWSClose(WebSocketCloseCode code)
        {
            switch (code)
            {
                case WebSocketCloseCode.Normal:
                    OnClose?.Invoke(WebsocketHandlerCloseCode.Normal);
                    break;
                default:
                    OnClose?.Invoke(WebsocketHandlerCloseCode.Abnormal);
                    break;
            }
        }

        void OnWSMessage(byte[] msgInBytes)
        {
            OnMessage?.Invoke(msgInBytes);
        }
    }

    public class ReactWS : IWebsocketsHandler
    {
        public Action OnOpen { get; set; }
        public Action<WebsocketHandlerCloseCode> OnClose { get; set; }
        public Action<string> OnError { get; set; }
        public Action<byte[]> OnMessage { get; set; }
        public bool IsInit { get; set; } = false;

        private IMomentumContext _c;

        public ReactWS(IMomentumContext ctx)
        {
            _c = ctx;
        }

        public void Close()
        {

        }

        public void Connect()
        {

        }

        public void Dispose()
        {

        }

        public WebsocketHandlerState GetState()
        {
            return WebsocketHandlerState.Open;
        }

        public void Init(string url)
        {
            /*
            _c.Get<IReactPosBusClient>().OnPosBusMessage -= OnReactPosBusMessage;
            _c.Get<IReactPosBusClient>().OnPosBusMessage += OnReactPosBusMessage;

            IsInit = true;
            */
        }

        void OnReactPosBusMessage(byte[] data)
        {
            /*
            Debug.Log("Got message in ReactWS: " + data.Length);

            OnMessage?.Invoke(data);
            */
        }

        public void Send(byte[] data)
        {

        }
    }

}
