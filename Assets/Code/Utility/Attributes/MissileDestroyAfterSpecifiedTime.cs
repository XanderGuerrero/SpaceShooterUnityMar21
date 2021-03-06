using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileDestroyAfterSpecifiedTime : MonoBehaviour
{
    [SerializeField]
    private NetworkIdentity networkIdentity;
    [SerializeField]
    private WhoActivateMe whoActivatedMe;
    public float timeToExplode = 3;
    private ExplosionData explosionData;


    // Start is called before the first frame update
    void Start()
    {
        explosionData = new ExplosionData();
        explosionData.position = new Position();
        StartCoroutine(wait());
    }

    // Update is called once per frame
    IEnumerator wait()
    {
        yield return new WaitForSeconds(timeToExplode);
        //GameObject explosion = Instantiate(Explosion, transform.position, Quaternion.identity);
        //Debug.Log("this.networkIdentity.GetID(): " + this.networkIdentity.GetID());


        //define the explosion
        //who activated it
        //whats its position
        explosionData.activator = this.networkIdentity.GetID(); //NetworkClient.ClientId;//the player
        explosionData.position.x = this.transform.position.x.TwoDecimals();
        explosionData.position.y = this.transform.position.y.TwoDecimals();
        explosionData.position.z = this.transform.position.z.TwoDecimals();
        //explosionData.id = 

        Debug.Log("destory this MISSILE networkIdentity: " + NetworkClient.ClientID);
        //Debug.Log(" explosionData.id: " + this.networkIdentity.GetID());
        networkIdentity.GetSocket().Emit("MissileDestroy", new JSONObject(JsonUtility.ToJson(explosionData)));
        //{
        //    //this bullets id will be sent to server
        //    id = this.networkIdentity.GetID()

        //})));

        //networkIdentity.GetSocket().Emit("DestroyExplosion", new JSONObject(JsonUtility.ToJson(explosionData)));
        //Destroy(this.gameObject, 1.5f);
        //Destroy(explosion, 1.5f);
    }
}
