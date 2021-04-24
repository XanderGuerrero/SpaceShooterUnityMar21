using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class RadarCameraFollowPlayer : MonoBehaviour
{
    private Transform cameraTarget;
    private GameObject player;
    public static bool firstTime = true;
    private CinemachineVirtualCamera vcam;

    // Start is called before the first frame update
    void Start()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
        //StartCoroutine(ExampleCoroutine());
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
                
                if (player != null)
                {
                    //Debug.Log("HI 2 " + player.name);
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
