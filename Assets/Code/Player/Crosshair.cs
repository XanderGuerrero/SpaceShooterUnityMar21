using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;



public class Crosshair : MonoBehaviour
{
    //public LayerMask targetMask;
    //public List<Collider> visibleTargets = new List<Collider>();
    public GameObject playerCrosshair;
    public GameObject player;
    //private Transform target;
    // Start is called before the first frame update

    public void SetUp(Vector3 position, Vector3 direction)
    {
        player.transform.position = position;
        player.transform.forward = direction;
    }


    void Update()
    {
        Ray ray = new Ray(player.transform.position, player.transform.forward);
        var newPos = ray.GetPoint(5000f);// player.transform.position.z + 5000f;
        //Debug.Log(newPos);
        //var targetPosition = new Vector3(player.transform.position.x, player.transform.position.y, newPos);
        Vector3 crosshairPositionOnScreen = Camera.main.WorldToScreenPoint(newPos);

        playerCrosshair.transform.position = Vector3.Lerp(playerCrosshair.transform.position, new Vector3(crosshairPositionOnScreen.x, crosshairPositionOnScreen.y, 0)/*Camera.main.WorldToScreenPoint(target.position)*/, Time.deltaTime * NetworkClient.SERVER_UPDATE_TIME);

        playerCrosshair.SetActive(true);


    }

}

//try the below out - https://forum.unity.com/threads/solved-position-crosshair-with-raycast.505672/

//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class CrosshairTest : MonoBehaviour
//{


//    public RecTransform crosshair;
//    public Camera cam;
//    public Transform muzzle;

//    void Update()
//    {
//        RaycastHit hit;
//        if (Physics.Raycast(muzzle.transform.position, muzzle.transform.forward, out hit))
//        {
//            if (hit.collider)
//            {
//                crosshair.position = cam.WorldToScreenPoint(hit.point);
//            }
//        }
//    }
//}
