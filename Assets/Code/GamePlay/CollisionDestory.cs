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
    AudioManager audioSource;
    public AudioClip BulletImpact;




    // Start is called before the first frame update
    void Start()
    {
        //audioSource = FindObjectOfType<AudioManager>();
    }


    public void OnCollisionEnter(Collision collision)
    {
        //collision.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.left * 50000);
        //get the identity of the object we collided with
        NetworkIdentity ni = collision.gameObject.GetComponent<NetworkIdentity>();


        if (ni == null || ni.GetID() != this.whoActivatedMe.GetActivator())
        {
            // networkIdentity.GetSocket().Emit("collisionDestroy", new JSONObject(JsonUtility.ToJson(CollisionData)));
            networkIdentity.GetSocket().Emit("collisionDestroy", new JSONObject(JsonUtility.ToJson(new IdData() {
            id = networkIdentity.GetID(),
            ObjCollidedWith = ni.GetID()

            })));
            this.gameObject.SetActive( false);
            //audioSource.PlaySFX(BulletImpact, .3f);
        }
    }
}

