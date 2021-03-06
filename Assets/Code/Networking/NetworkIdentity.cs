using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;


public class NetworkIdentity : MonoBehaviour
{

    [Header("Helpful Values")]
    [SerializeField]
    [GreyOut]
    private string id;

    [SerializeField]
    [GreyOut]
    private bool isControlling;

    //so we can send events and position to the server
    //network transform will ask for socket to send the updated position
    private SocketIOComponent socket;

    public void Awake()
    {
        isControlling = false;
    }

    public void SetControllerID(string ID)
    {
        id = ID;
        //true or false value
        isControlling = (NetworkClient.ClientID == ID) ? true : false;// check incoming id against the one saved from the server

    }

    //set the socket coming in from the network client
    //to this class
    public void SetScoketReference(SocketIOComponent Socket)
    {
        socket = Socket;
    }
    public string GetID()
    {
        return id;
    }

    public bool IsControlling()
    {
        return isControlling;
    }

    public SocketIOComponent GetSocket()
    {
        return socket;
    }

}
