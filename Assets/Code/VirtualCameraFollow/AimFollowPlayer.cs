using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class AimFollowPlayer : MonoBehaviour
{
    private Transform cameraTarget;
    private GameObject player;
    bool firstTime = true;
    private CinemachineVirtualCamera vcam;

    // Start is called before the first frame update
    void Start()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
    }

    void Update()
    {
        if (firstTime)
        {
            //Debug.Log("attaching camera to player" + NetworkClient.ClientID);
            if (NetworkClient.ClientID != null)
            {
                //Debug.Log("HI " + NetworkClient.ClientID);
                player = GameObject.Find("/[Server Spawned Objects]/Player(" + NetworkClient.ClientID + ")");
                //Debug.Log("HI 2 " + player.name);
                if (player != null)
                {
                    //Debug.Log("attaching camera to player");
                    cameraTarget = player.transform;

                    vcam.LookAt = cameraTarget;
                    vcam.Follow = cameraTarget;

                    firstTime = false;
                }
            }
        }
    }
}