using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDestory : MonoBehaviour
{
    [SerializeField]
    private NetworkIdentity networkIdentity;
    [SerializeField]
    private WhoActivateMe whoActivatedMe;
    private float dist;
    //private IdData CollisionData;
    //private string networkIDofCollidedObject;



    // Start is called before the first frame update
    //void Start()
    //{
    //    CollisionData = new IdData();
    //}



    public void OnCollisionEnter(Collision collision)
    {
        //collision.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.left * 50000);
        //get the identity of the object we collided with
        NetworkIdentity ni = collision.gameObject.GetComponent<NetworkIdentity>();

        //networkIDofCollidedObject = ni.ToString();
        //if (ni == null)
        //{
        //    CollisionData.collisionObjectsNetID = "environment";
        //    CollisionData.id = this.networkIdentity.GetID();
        //}
        //else
        //{
        //    string nameOfCollisionObj = ni.ToString().Substring(0, ni.ToString().IndexOf('('));
        //    string stringBeforeChar = ni.ToString().Substring(ni.ToString().IndexOf('('), ni.ToString().IndexOf(')'));
        //    stringBeforeChar = stringBeforeChar.Substring(stringBeforeChar.IndexOf('('), stringBeforeChar.ToString().IndexOf(')'));
        //    stringBeforeChar = stringBeforeChar.Substring(stringBeforeChar.LastIndexOf('(') + 1);
        //    CollisionData.collisionObjectsNetID = stringBeforeChar;
        //    CollisionData.name = nameOfCollisionObj;
        //    var Dist = collision.gameObject.transform.position - collision.other.gameObject.transform.position;

            if (ni == null || ni.GetID() != this.whoActivatedMe.GetActivator())
            {
                //Debug.Log("whoActivatedMe: " + whoActivatedMe.GetActivator());
                //CollisionData.distance = 0;
                //CollisionData.id = this.networkIdentity.GetID();
                ////Debug.Log("asteroid position: " + collision.gameObject.transform.position);
                //CollisionData.position.x = collision.gameObject.transform.position.x;
                //CollisionData.position.y = collision.gameObject.transform.position.y;
                //CollisionData.position.z = collision.gameObject.transform.position.z;

                // networkIdentity.GetSocket().Emit("collisionDestroy", new JSONObject(JsonUtility.ToJson(CollisionData)));
                networkIdentity.GetSocket().Emit("collisionDestroy", new JSONObject(JsonUtility.ToJson(new IdData() {
                id = networkIdentity.GetID(),
                ObjCollidedWith = ni.GetID()

                })));
                this.gameObject.SetActive( false);
            }
       // }
    }
}

