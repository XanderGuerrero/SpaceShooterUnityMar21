using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkTransform : MonoBehaviour
{
    [SerializeField]
    [GreyOut]
    private Vector3 oldposition;
    private Vector3 oldrotation;
    private NetworkIdentity networkIdentity;
    private Player player;

    private float stillCounter = 0;

    public void Start()
    {
        networkIdentity = GetComponent<NetworkIdentity>();
        oldposition = transform.position;
        player = new Player();
        player.position = new Position();
        player.position.x = 0;
        player.position.y = 0;
        //player.position.z = 0;

        //if networkIdentity.IsControlling() is false...
        //turn off transform sending
        if (!networkIdentity.IsControlling())
        {
            enabled = false;
        }
    }


    public void Update()
    {
        if (networkIdentity.IsControlling())
        {
            //if the players position is different
            // we are going to send data
            if (oldposition != transform.position)
            {
                oldposition = transform.position;

                stillCounter = 0;
                SendData();
            }
            //otherwise if standing still, we are going to count
            else
            {
                stillCounter += Time.deltaTime;
                //if the counter is greater than one second, reset the counter
                //and tell the server we aree still here even though our position hasnt changed
                if (stillCounter >= 1)
                {
                    stillCounter = 0;
                    SendData();
                }
            }
        }
    }


    //calls the socket to send player position and other data back to the server
    private void SendData()
    {
        //update player info
        //using round function to use point 3 decimal places to reduce data
        player.position.x = transform.position.x.TwoDecimals();//Mathf.Round(transform.position.x * 1000.0f) / 1000.0f;
        player.position.y = transform.position.y.TwoDecimals();//Mathf.Round(transform.position.y * 1000.0f) / 1000.0f;
        player.position.z = transform.position.z.TwoDecimals();

        //Debug.Log("send position data: " + player.position.z);


        //send the players postional data back to the server
        //wrap data in a json object
        networkIdentity.GetSocket().Emit("updatePosition", new JSONObject(JsonUtility.ToJson(player)));
    }
    //void OnGUI()
    //{
    //    GUILayout.Label("send position data: " + player.position.x);
    //    GUILayout.Label("send position data: " + player.position.y);
    //    GUILayout.Label("send position data: " + player.position.z);
    //}


}
