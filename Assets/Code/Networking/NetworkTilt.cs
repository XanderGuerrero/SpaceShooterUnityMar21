using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(NetworkIdentity))]
public class NetworkTilt : MonoBehaviour
{

    [Header("Referenced Values")]
    [SerializeField]
    [GreyOut]
    private float oldTilt;

    [Header("Class References")]
    [SerializeField]
    private PlayerManager playermanager;

    private NetworkIdentity networkIdentity;
    private PlayerTilt player;
    //private Rigidbody rb;
    private float stillCounter = 0;


    // Start is called before the first frame update
    void Start()
    {
        networkIdentity = GetComponent<NetworkIdentity>();
        player = new PlayerTilt();
        player.zValueForTilt = 0;
        //rb = playermanager.GetComponent<Rigidbody>();
        //if we are not controlling the script, turn it off
        if (!networkIdentity.IsControlling())
        {
            enabled = false;

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (networkIdentity.IsControlling())
        {
            if (oldTilt != playermanager.GetLastTilt() /*|| oldTilt != (rb.velocity.x / 2) * -oldTilt*/)
            {
                /*oldBarrelRotation = playermanager.GetLastRotation();*/
                oldTilt = playermanager.GetLastTilt();
                stillCounter = 0;
                sendData();
            }
            else
            {
                stillCounter += Time.deltaTime;
                if (stillCounter >= 1)
                {
                    stillCounter = 0;
                    sendData();
                }
            }
        }
    }

    public void sendData()
    {
        //supdate the tilt to the most current value
        player.zValueForTilt = playermanager.GetLastTilt().TwoDecimals();

        //send the current tilt of the ship to the server
        //Debug.Log("send tilt data: " + player.zValueForTilt);
        networkIdentity.GetSocket().Emit("updateShipTilt", new JSONObject(JsonUtility.ToJson(player)));
    }
}
