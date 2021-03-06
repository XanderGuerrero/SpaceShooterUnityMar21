using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileCollisionDestroy : MonoBehaviour
{
    [SerializeField]
    private NetworkIdentity networkIdentity;
    [SerializeField]
    private WhoActivateMe whoActivatedMe;
    


    public void OnCollisionEnter(Collision collision)
    {
        //collision.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward* 2000);
        //collision.gameObject.GetComponent<Rigidbody>().AddExplosionForce(2000, collision.gameObject.transform.position, 100);
        //get the identity of the object we collided with
        NetworkIdentity ni = collision.gameObject.GetComponent<NetworkIdentity>();

        if (ni == null || ni.GetID() != this.whoActivatedMe.GetActivator())
        {
            //Debug.Log("ObjCollidedWith: " + collision.gameObject.name);
            //Debug.Log("ObjCollidedWith: " + networkIdentity.GetID());
            networkIdentity.GetSocket().Emit("missileCollisionDestroy", new JSONObject(JsonUtility.ToJson(new IdData()
            {
                id = networkIdentity.GetID(),
                ObjCollidedWith = ni.GetID()

            })));
            this.gameObject.SetActive(false);
        }
        
    }
}
