using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager2 : MonoBehaviour
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
        Debug.Log("체인지 씬 클릭");
        UnityEngine.SceneManagement.SceneManager.LoadScene("sample");
    }

    void Start()
    {
        session_ = protobuf_session.getInstance();
        session_.init();
        //session_.connect(IP, PORT); 
        session_.scene_connected_callback = OnConnect;
        session_.scene_disconnected_callback = OnDisconnect;
    }

    void Update()
    {

    }

}
