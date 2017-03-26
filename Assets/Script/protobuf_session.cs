﻿using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using UnityEngine.UI;
using Google.Protobuf;

public class protobuf_session : MonoBehaviour
{
    private static protobuf_session instance_ = null;

    const  String IP = "127.0.0.1";
    const int PORT = 3000;

    static private network_module network_module_;

    public delegate void scene_on_connected(bool r);
    public delegate void scene_on_disconnected();
    public scene_on_connected    scene_connected_callback = null;
    public scene_on_disconnected scene_disconnected_callback = null;

    //public int recv_count = 0;

    private Int16 head_size = sizeof(Int16);
    private Int16 opcode_size = sizeof(Int16);

    public static protobuf_session getInstance()
    {
        if (instance_ == null)
        {
            GameObject protobuf_session = new GameObject("protobuf_session");
            instance_ = protobuf_session.AddComponent<protobuf_session>();
        }
        return instance_;
    }

    private static void write_head(Stream buf, long size)
    {
        byte[] head_bytes = BitConverter.GetBytes(size);
        buf.Write(head_bytes, 0, sizeof(System.Int16));
    }

    private static void write_opcode(Stream buf, Int16 opcode)
    {
        byte[] opcode_bytes = BitConverter.GetBytes(opcode);
        buf.Write(opcode_bytes, 0, sizeof(System.Int16));
    }

    public void on_connected(bool r)
    {
        if (r)
        {
            //Debug.Log("접속 성공");
            if (scene_connected_callback != null)
            {
                scene_connected_callback(r);
            }
            //LOBBY.CS_LOG_IN cs_log_in = new LOBBY.CS_LOG_IN();
            //cs_log_in.Id = "do not know mac address";
            //cs_log_in.Password = "abcde";

            /*
            for (var i = 0; i < 1; ++i)
            {
                send_protobuf((short)packet.opcode.CS_LOG_IN, cs_log_in);
            }
            */
        }
        else
        {
            scene_connected_callback(r);
            //Debug.Log("접속 실패");
        }
    }

    void on_disconnected()
    {
        //Debug.Log("접속 끓킴!!!!");
        if (scene_disconnected_callback != null)
        {
            scene_disconnected_callback();
        }
    }

    public void connect(String host = IP, int port = PORT)
    {
        network_module_.connect(IP, PORT);
    }

    public void disconnect()
    {
        network_module_.disconnect();
    }

    public void send_protobuf(network.opcode op, IMessage protobuf)
    {
        Int16 opcode = (Int16)op;
        MemoryStream protobuf_stream = new MemoryStream();
        protobuf.WriteTo(protobuf_stream);

        MemoryStream packet = new MemoryStream();

        Int16 len = (Int16)(head_size + opcode_size + (Int16)protobuf_stream.Length);

        write_head(packet, len);
        write_opcode(packet, opcode);

        byte[] protobuf_buffer;
        protobuf_buffer = protobuf_stream.ToArray();

        packet.Write(protobuf_buffer, 0, (int)protobuf_stream.Length);

        network_module_.send_packet(packet);
    }

    public void init()
    {
        //GameObject protobuf_session = new GameObject("protobuf_session");
        //network_module_ = new network_module();
        if (network_module_ == null)
        {
            network_module_ = new network_module();
            network_module_.init();
            network_module_.connected_callback = on_connected;
            network_module_.disconnected_callback = on_disconnected;
        }
    }

    struct PROTOBUF_PACKET
    {
        public network.opcode op;
        public global::Google.Protobuf.IMessage protobuf;
    }


    // SC_LOGIN
    public delegate void CB_LOBBY_SC_LOG_IN(LOBBY.SC_LOG_IN read);
    public CB_LOBBY_SC_LOG_IN processor_LOBBY_SC_LOG_IN = null;

    // SC_SET_NICKNAME
    public delegate void CB_LOBBY_SC_SET_NICKNAME(LOBBY.SC_SET_NICKNAME read);
    public CB_LOBBY_SC_SET_NICKNAME processor_LOBBY_SC_SET_NICKNAME = null;


    // 패킷 처리
    public void process_packet()
    {
        while (network_module_.recv_stream_queue.Count > 0)
        {
            var packet_stream = network_module_.recv_stream_queue.Dequeue();

            // 패킷 스트림에서 대가리 2바이트 짤라서 opcode 가져와야함
            byte[] packet_buffer = packet_stream.ToArray();

            Int16 _opcode = BitConverter.ToInt16(packet_buffer, 0);
            var protobuf_stream = new MemoryStream();
            var protobuf_size = packet_stream.Length - sizeof(Int16);
            protobuf_stream.Write(packet_buffer, sizeof(Int16), (int)protobuf_size);

            byte[] proto_buffer = protobuf_stream.ToArray();

            
            try
            {
                if ((network.opcode)_opcode == network.opcode.SC_LOG_IN)
                {
                    LOBBY.SC_LOG_IN read = LOBBY.SC_LOG_IN.Parser.ParseFrom(proto_buffer);
                    if (processor_LOBBY_SC_LOG_IN != null)
                    {
                        processor_LOBBY_SC_LOG_IN(read);
                    }
                }
                else if ((network.opcode)_opcode == network.opcode.SC_SET_NICKNAME)
                {
                    LOBBY.SC_SET_NICKNAME read = LOBBY.SC_SET_NICKNAME.Parser.ParseFrom(proto_buffer);
                    if (processor_LOBBY_SC_SET_NICKNAME != null)
                    {
                        processor_LOBBY_SC_SET_NICKNAME(read);
                    }
                }
                
            }
            catch(Exception e)
            {
                Debug.Log("protobuf 읽다가 에러");
            }
            
        }
    }

    void OnApplicationQuit()
    {
        disconnect();
    }


}
