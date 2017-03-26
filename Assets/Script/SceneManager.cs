using network;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public protobuf_session session_;

    public void OnConnect(bool result)
    {
        if (result)
        {
            Debug.Log("접속 성공");
        }
        else
        {
            Debug.Log("접속 실패");
        }
    }

    void OnDisconnect()
    {
        Debug.Log("접속 끊김");
    }

    void Connect()
    {
        session_.connect();
    }

    void Disconnect()
    {
        session_.disconnect();
    }

    public void onClickConnectButton()
    {
        Debug.Log("접속 시도 버튼 클릭");
        Connect();
    }

    public void onClickDisconnectButton()
    {
        Debug.Log("연결 끊김 버튼 클릭");
        Disconnect();
    }

    public void onClickChangeSceneButton()
    {
        Debug.Log("로드 sample2 씬");
        UnityEngine.SceneManagement.SceneManager.LoadScene("sample2");
    }

    void RegisterProcessorHandler()
    {
        session_.processor_LOBBY_SC_LOG_IN = processor_LOBBY_SC_LOG_IN;
    }

    void Start()
    {
        session_ = protobuf_session.getInstance();
        session_.init();
        //session_.connect(IP, PORT); 
        session_.scene_connected_callback = OnConnect;
        session_.scene_disconnected_callback = OnDisconnect;

        RegisterProcessorHandler();
    }

    void Update()
    {
        session_.process_packet();

        //Debug.Log("update");
    }

    void Destroy()
    {

    }

    public void onClickSendLoginButton()
    {
        Debug.Log("로그인 패킷 보냄");
        LOBBY.CS_LOG_IN cs_log_in = new LOBBY.CS_LOG_IN();
        cs_log_in.Id = "으으앙";
        cs_log_in.Password = "12345";
     
        session_.send_protobuf(opcode.CS_LOG_IN, cs_log_in);
    }

    void processor_LOBBY_SC_LOG_IN(LOBBY.SC_LOG_IN read)
    {
        Debug.Log("패킷 로그인 받음");
        Debug.Log("Result: " + read.Result);
    }
}
