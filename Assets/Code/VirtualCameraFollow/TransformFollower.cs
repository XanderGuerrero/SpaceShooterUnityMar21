using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformFollower : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    [SerializeField]
    private Vector3 offsetPosition;

    [SerializeField]
    private Space offsetPositionSpace = Space.Self;

    [SerializeField]
    private bool lookAt = true;
        private GameObject player;
    bool firstTime = true;
    private void Update()
    {
        if (firstTime)
        {
            //Debug.Log("attaching camera to player" + NetworkClient.ClientID);
            //if (NetworkClient.ClientID != null)
            //{
                //Debug.Log("HI " + NetworkClient.ClientID);
                player = GameObject.Find("/[Server Spawned Objects]/Player(" + NetworkClient.ClientID + ")");
                //Debug.Log("HI 2 " + player.name);
                if (player != null)
                {
                //Debug.Log("attaching camera to player");
                target = player.transform;

                    //vcam.LookAt = cameraTarget;
                    //vcam.Follow = cameraTarget;

                    firstTime = false;
                }
            //}
        }
        Refresh();
    }

    public void Refresh()
    {
        if (target == null)
        {
            Debug.LogWarning("Missing target ref !", this);

            return;
        }

        // compute position
        if (offsetPositionSpace == Space.Self)
        {
            transform.position = target.TransformPoint(offsetPosition);
        }
        else
        {
            transform.position = target.position + offsetPosition;
        }

        // compute rotation
        if (lookAt)
        {
            Camera.main.transform.rotation = Quaternion.Euler(player.GetComponent<Rigidbody>().rotation.x, player.GetComponent<Rigidbody>().rotation.y, -player.GetComponent<Rigidbody>().rotation.z);
            //transform.LookAt(target);
        }
        else
        {
            transform.rotation = target.rotation;
        }
    }
}

