using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyExplosion : MonoBehaviour
{
    [SerializeField]
    private NetworkIdentity networkIdentity;
    [SerializeField]
    private WhoActivateMe whoActivatedMe;
    public float timeToDestroy = 2;
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
        yield return new WaitForSeconds(timeToDestroy);
        //GameObject explosion = Instantiate(Explosion, transform.position, Quaternion.identity);
        //Debug.Log("destroy the explosion NOW");


        networkIdentity.GetSocket().Emit("DestroyExplosion", new JSONObject(JsonUtility.ToJson(new IdData()
        {
            //this explosions id will be sent to server
            id = this.networkIdentity.GetID()

        })));
    }
}
