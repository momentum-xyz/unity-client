using TMPro;
using UnityEngine;

namespace PosBusAPI
{
    using System;
    using Odyssey.Networking;

    public enum Msg : UInt32
    {
        NONE = 0,
        FlatBufferMessage = 0x49a0b0b9, // used
        UsersPositions = 0x1FE5B46F, // used
        GoneUsers = 0x3327c20c, // used
        SendPosition = 0xfbf6b89f,
        RemoveStaticObjects = 0x06383502, // used
        SetStaticObjectPosition = 0x300A3883, // used
        ActiveObjectsPositions = 0xfff1b390,
        TriggerTransitionalEffectsOnObject = 0xE0A9E0A7,
        TriggerTransitionalEffectsOnPosition = 0x3597729E,
        TriggerTransitionalBridgingEffectsOnObject = 0xE45A7B03,
        TriggerTransitionalBridgingEffectsOnPosition = 0xF6AB754D,
        TriggerInteraction = 0x2C0A16A0,
        SimpleNotification = 0x3CADFD52,
        RelayToReact   = 0xB5BBCFA2,
        Signal = 0x6A8634A3,
        SwitchWorld = 0x7D40FD67

    }

    public enum Destination : byte
    {
        Unity = 0b01,
        React = 0b10,
        Both = 0b11
    }

    public enum NotificationType : UInt32
    {
        None = 0,
        Wow = 1,
        HighFive = 2,
        Generic = 999,
        Legacy = 1000
    }

    public enum TriggerType : UInt32
    {
        None = 0,
        Wow = 1,
        HighFive = 2,
        EnteredSpace = 3,
        LeftSpace = 4, 
        TriggerStake = 5
    }

    public enum SignalType : UInt32
    {
        SignalNone = 0,
        SignalDualConnection = 1,
        SignalReady = 2
    }

public struct Decoder
    {
        public static Guid getUUID(byte[] buf, int offset)
        {
            var rawGuid = new byte[16];
            Buffer.BlockCopy(buf, offset, rawGuid, 0, 16);
            SwapBytes(ref rawGuid);
            return new Guid(rawGuid);
        }

        public static void setUUID(Guid id, byte[] buf, int offset)
        {
            var rawGuid = id.ToByteArray();
            SwapBytes(ref rawGuid);
            Buffer.BlockCopy(rawGuid, 0, buf, offset, 16);
        }

        public static void SwapBytes(ref byte[] buf)
        {
            (buf[3], buf[0]) = (buf[0], buf[3]);
            (buf[2], buf[1]) = (buf[1], buf[2]);
            (buf[4], buf[5]) = (buf[5], buf[4]);
            (buf[6], buf[7]) = (buf[7], buf[6]);

        }

    }
    public class PosBusMsg
    {
        // void __init(byte[] _bb);
        protected byte[] __b;
        public byte[] Buffer { get { return __b; } }
        public PosBusMsg(byte[] b) { __b = b; }
        public PosBusMsg() { }
        public PosBusMsg(PosBusMsg b) { __b = b.__b; }
        public PosBusAPI.Msg MsgType
        {
            get
            {
                UInt64 header = BitConverter.ToUInt32(__b, 0);
                UInt32 checker = BitConverter.ToUInt32(__b, __b.Length - 4);
                return ((header == (~checker)) ? (PosBusAPI.Msg)header : PosBusAPI.Msg.NONE);
            }
        }
        public void SetMsgType(PosBusAPI.Msg type)
        {
            System.Buffer.BlockCopy(BitConverter.GetBytes((UInt32)type), 0, __b, 0, 4);
            System.Buffer.BlockCopy(BitConverter.GetBytes(~(UInt32)type), 0, __b, __b.Length - 4, 4);
        }
    }
    public class FlatBufferMessage : PosBusMsg
    {
        public FlatBufferMessage(PosBusMsg b) { __b = b.Buffer; }
        public FlatBufferMessage(global::FlatBuffers.ByteBuffer buf)
        {
            var datalen = buf.Length - buf.Position;
            __b = new byte[datalen + 8];
            SetMsgType(Msg.FlatBufferMessage);
            buf.ToMemoryStream(buf.Position, datalen).Read(__b, 4, datalen);
        }

        public API.FlatBuffMsg Message()
        {
            var len = __b.Length - 8;
            var content = new byte[len];
            Array.Copy(__b, 4, content, 0, len);
            return API.FlatBuffMsg.GetRootAsFlatBuffMsg(new global::FlatBuffers.ByteBuffer(content));
        }


    }

    public class UsersPositionsMsg : PosBusMsg
    {
        public UsersPositionsMsg(PosBusMsg b) { __b = b.Buffer; }
        public uint NUsers() { return BitConverter.ToUInt32(__b, 4); }
        public Guid Id(int i)
        {
            return Decoder.getUUID(__b, 8 + 28 * i);
        }
        public UnityEngine.Vector3 Position(int i)
        {
            return new UnityEngine.Vector3(BitConverter.ToSingle(__b, 8 + 28 * i + 16), BitConverter.ToSingle(__b, 8 + 28 * i + 20), BitConverter.ToSingle(__b, 8 + 28 * i + 24));
        }
        public PosBusPosMsg GetElement(int i)
        {
            return new PosBusPosMsg(Id(i), Position(i));
        }
    }

    public class SignalMsg : PosBusMsg
    {
        public SignalMsg(PosBusMsg b) { __b = b.Buffer; }

        public SignalMsg(SignalType s)
        {
            var msglen = 4;
            __b = new byte[msglen + 8];
            SetMsgType(Msg.Signal);
            BitConverter.GetBytes((UInt32)s).CopyTo(__b, 4);
        }
        public uint Signal() { return BitConverter.ToUInt32(__b, 4); }
    }

    public class GoneUsersMsg : PosBusMsg
    {
        public GoneUsersMsg(PosBusMsg b) { __b = b.Buffer; }
        public uint NUsers() { return BitConverter.ToUInt32(__b, 4); }
        public Guid Id(int i)
        {
            return Decoder.getUUID(__b, 8 + 16 * i);
        }
    }

    public class SendPositionMsg : PosBusMsg
    {
        public SendPositionMsg(PosBusMsg b) { __b = b.Buffer; }
        public SendPositionMsg()
        {
            __b = new byte[20];
            SetMsgType(Msg.SendPosition);
        }
        public SendPositionMsg(in UnityEngine.Vector3 pos) : this()
        {
            SetPostion(pos);
        }
        public void SetPostion(in UnityEngine.Vector3 pos)
        {
            System.Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, __b, 4, 4);
            System.Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, __b, 8, 4);
            System.Buffer.BlockCopy(BitConverter.GetBytes(pos.z), 0, __b, 12, 4);
        }
        public UnityEngine.Vector3 Postion()
        {
            return new UnityEngine.Vector3(BitConverter.ToSingle(__b, 4), BitConverter.ToSingle(__b, 8), BitConverter.ToSingle(__b, 12));
        }

    }

    public class TriggerInteractionMsg : PosBusMsg
    {
        public TriggerInteractionMsg(PosBusMsg b) { __b = b.Buffer; }
        public TriggerInteractionMsg(UInt32 kind, Guid targetId, Int32 flag, string message)
        {
            var msglen = 28 + message.Length;
            __b = new byte[msglen + 8];
            SetMsgType(Msg.TriggerInteraction);
            BitConverter.GetBytes(kind).CopyTo(__b, 4);
            Decoder.setUUID(targetId, __b, 8);
            BitConverter.GetBytes(flag).CopyTo(__b, 24);
            BitConverter.GetBytes((UInt32)message.Length).CopyTo(__b, 28);
            System.Buffer.BlockCopy(System.Text.Encoding.ASCII.GetBytes(message), 0, __b, 32, message.Length);
        }

    }

    public class SimpleNotificationMsg : PosBusMsg
    {
        public SimpleNotificationMsg(PosBusMsg b) { __b = b.Buffer; }
        public PosBusSimpleNotificationMsg Get(){
            var obj = new PosBusSimpleNotificationMsg();
            obj.Destination =(PosBusAPI.Destination)__b[4];
            obj.Kind = BitConverter.ToUInt32(__b, 5);
            obj.Flag=BitConverter.ToInt32(__b, 9);
            var slen = (Int32)BitConverter.ToUInt32(__b, 13);
            obj.Message=System.Text.Encoding.ASCII.GetString(__b, 17,slen);
            return obj;
        }

    }
    public class RelayToReactMsg : PosBusMsg
    {
        public RelayToReactMsg(PosBusMsg b) { __b = b.Buffer; }
        public PosBusRelayToReactMsg Get(){
            var obj = new PosBusRelayToReactMsg();
            var toffs = 4;
            var tlen = (Int32)BitConverter.ToUInt32(__b, toffs);
            obj.Target=System.Text.Encoding.ASCII.GetString(__b, toffs+4,tlen);
            var moffs = toffs +4+ tlen;
            var mlen = (Int32)BitConverter.ToUInt32(__b, moffs);
            obj.Message=System.Text.Encoding.ASCII.GetString(__b, moffs+4,mlen);
            return obj;
        }
    }


    public class SwitchWorldMsg : PosBusMsg
    {
        public SwitchWorldMsg(PosBusMsg b) { __b = b.Buffer; }
        public SwitchWorldMsg()
        {
            __b = new byte[24];
            SetMsgType(Msg.SwitchWorld);
        }
        public SwitchWorldMsg(in Guid id) : this()
        {
            SetWorld(id);
        }
        public void SetWorld(in Guid id)
        {
            Decoder.setUUID(id, __b, 4);
        }
        public Guid World(in UnityEngine.Vector3 pos)
        {
            return Decoder.getUUID(__b, 4);
        }

    }

    public class SetStaticObjectPositionMsg : PosBusMsg
    {
        public SetStaticObjectPositionMsg(PosBusMsg b) { __b = b.Buffer; }
        public Guid Id()
        {
            return Decoder.getUUID(__b, 4);
        }
        public UnityEngine.Vector3 Position()
        {
            return new UnityEngine.Vector3(BitConverter.ToSingle(__b, 20), BitConverter.ToSingle(__b, 24), BitConverter.ToSingle(__b, 28));
        }
        public PosBusSetStaticObjectPositionMsg Get()
        {
            return new PosBusSetStaticObjectPositionMsg(Id(), Position());
        }
    }
    public class RemoveStaticObjectsMsg : PosBusMsg
    {
        public RemoveStaticObjectsMsg(PosBusMsg b) { __b = b.Buffer; }
        public uint NObjects() { return BitConverter.ToUInt32(__b, 4); }
        public Guid ObjectId(int i) { return Decoder.getUUID(__b, 8 + 16 * i); }
    }

    public class SetActiveObjectPositionMsg : PosBusMsg
    {
        public SetActiveObjectPositionMsg(PosBusMsg b) { __b = b.Buffer; }
        public Guid TypeId()
        {
            return Decoder.getUUID(__b, 4);
        }
        public uint NObjects() { return BitConverter.ToUInt32(__b, 20); }
        public Guid Id(int i)
        {
            return Decoder.getUUID(__b, 24 + 28 * i);
        }
        public UnityEngine.Vector3 Position(int i)
        {
            return new UnityEngine.Vector3(BitConverter.ToSingle(__b, 24 + 28 * i + 16), BitConverter.ToSingle(__b, 24 + 28 * i + 20), BitConverter.ToSingle(__b, 24 + 28 * i + 24));
        }
    }

    public class TriggerTransitionalEffectsOnObjectMsg : PosBusMsg
    {
        public TriggerTransitionalEffectsOnObjectMsg(PosBusMsg b) { __b = b.Buffer; }
        public uint NEffects() { return BitConverter.ToUInt32(__b, 4); }
        private const int array_offset = 36;
        private const int array_start = 8;

        public Guid Emmiter(int i)
        {
            return Decoder.getUUID(__b, array_start + array_offset * i);
        }
        public Guid Object(int i)
        {
            return Decoder.getUUID(__b, array_start + array_offset * i + 16);
        }

        public UInt32 Effect(int i)
        {
            return BitConverter.ToUInt32(__b, array_start + array_offset * i + 32);
        }

        public PosBusTransitionalEffectOnObjectMsg GetElement(int i)
        {
            return new PosBusTransitionalEffectOnObjectMsg(Emmiter(i), Object(i), Effect(i));
        }
    }

    public class TriggerTransitionalBridgingEffectsOnObjectMsg : PosBusMsg
    {
        public TriggerTransitionalBridgingEffectsOnObjectMsg(PosBusMsg b) { __b = b.Buffer; }
        public uint NEffects() { return BitConverter.ToUInt32(__b, 4); }
        private const int array_offset = 52;
        private const int array_start = 8;

        public Guid Emmiter(int i)
        {
            return Decoder.getUUID(__b, array_start + array_offset * i);
        }
        public Guid ObjectFrom(int i)
        {
            return Decoder.getUUID(__b, array_start + array_offset * i + 16);
        }
        public Guid ObjectTo(int i)
        {
            return Decoder.getUUID(__b, array_start + array_offset * i + 32);
        }
        public UInt32 Effect(int i)
        {
            return BitConverter.ToUInt32(__b, array_start + array_offset * i + 48);
        }
        public PosBusTransitionalBridgingEffectOnObjectMsg GetElement(int i)
        {
            return new PosBusTransitionalBridgingEffectOnObjectMsg(Emmiter(i), ObjectFrom(i), ObjectTo(i), Effect(i));
        }
    }

    public class TriggerTransitionalEffectsOnPositionMsg : PosBusMsg
    {
        public TriggerTransitionalEffectsOnPositionMsg(PosBusMsg b) { __b = b.Buffer; }
        public uint NEffects() { return BitConverter.ToUInt32(__b, 4); }
        private const int array_offset = 32;
        private const int array_start = 8;

        public Guid Emmiter(int i)
        {
            return Decoder.getUUID(__b, array_start + array_offset * i);
        }
        public UnityEngine.Vector3 Position(int i)
        {
            int pos = array_start + array_offset * i + 16;
            return new UnityEngine.Vector3(BitConverter.ToSingle(__b, pos), BitConverter.ToSingle(__b, pos + 4), BitConverter.ToSingle(__b, pos + 8));
        }

        public UInt32 Effect(int i)
        {
            return BitConverter.ToUInt32(__b, array_start + array_offset * i + 28);
        }

        public PosBusTransitionalEffectOnPositionMsg GetElement(int i)
        {
            return new PosBusTransitionalEffectOnPositionMsg(Emmiter(i), Position(i), Effect(i));
        }
    }

    public class TriggerTransitionalBridgingEffectsOnPositionMsg : PosBusMsg
    {
        public TriggerTransitionalBridgingEffectsOnPositionMsg(PosBusMsg b) { __b = b.Buffer; }
        public uint NEffects() { return BitConverter.ToUInt32(__b, 4); }
        private const int array_offset = 44;
        private const int array_start = 8;

        public Guid Emmiter(int i)
        {
            return Decoder.getUUID(__b, array_start + array_offset * i);
        }
        public UnityEngine.Vector3 PositionFrom(int i)
        {
            int pos = array_start + array_offset * i + 16;
            return new UnityEngine.Vector3(BitConverter.ToSingle(__b, pos), BitConverter.ToSingle(__b, pos + 4), BitConverter.ToSingle(__b, pos + 8));
        }
        public UnityEngine.Vector3 PositionTo(int i)
        {
            int pos = array_start + array_offset * i + 28;
            return new UnityEngine.Vector3(BitConverter.ToSingle(__b, pos), BitConverter.ToSingle(__b, pos + 4), BitConverter.ToSingle(__b, pos + 8));
        }

        public UInt32 Effect(int i)
        {
            return BitConverter.ToUInt32(__b, array_start + array_offset * i + 40);
        }
        public PosBusTransitionalBridgingEffectOnPositionMsg GetElement(int i)
        {
            return new PosBusTransitionalBridgingEffectOnPositionMsg(Emmiter(i), PositionFrom(i), PositionTo(i), Effect(i));
        }
    }

}




