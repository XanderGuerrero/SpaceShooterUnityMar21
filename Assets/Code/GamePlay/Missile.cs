using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    //public Transform target;
    //private float rocketTurnSpeed;
    //private float rocketSpeed;
    //private float randomOffset;

    //private float timerSinceLaunch_Contor;
    //private float objectLifeTimerValue;

    //// Use this for initialization
    //void Start()
    //{
    //    if(target == null)
    //    {
    //        Debug.Log("no transform");
    //    }
    //    rocketTurnSpeed = 50.0f;
    //    rocketSpeed = 45f;
    //    randomOffset = 0.0f;

    //    timerSinceLaunch_Contor = 0;
    //    objectLifeTimerValue = 10;
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    timerSinceLaunch_Contor += Time.deltaTime;

    //    if (target != null)
    //    {
    //        if (timerSinceLaunch_Contor > 1)
    //        {
    //            if ((target.position - transform.position).magnitude > 50)
    //            {
    //                randomOffset = 100.0f;
    //                rocketTurnSpeed = 90.0f;
    //            }
    //            else
    //            {
    //                randomOffset = 5f;
    //                //if close to target
    //                if ((target.position - transform.position).magnitude < 10)
    //                {
    //                    rocketTurnSpeed = 180.0f;
    //                }
    //            }
    //        }

    //        Vector3 direction = target.position - transform.position + Random.insideUnitSphere * randomOffset;
    //        direction.Normalize();
    //        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction), rocketTurnSpeed * Time.deltaTime);
    //        transform.Translate(Vector3.forward * rocketSpeed * Time.deltaTime);
    //    }

    //    if (timerSinceLaunch_Contor > objectLifeTimerValue)
    //    {
    //        Destroy(transform.gameObject, 1);
    //    }
    //}
    [Header("setUp")]
    public Vector3 rocketTarget;
    public string targetId;
    public Vector3 missilePosition;
    public Rigidbody rocketRgb;
    public float turnSpeed =15;
    public float rocketFlySpeed;

    private Transform rocketLocalTransform;
    private Vector3 direction;
    private float speed;
    private Vector3 movement;
    public NetworkClient scriptInstance = null;

    public float Speed
    {
        set
        {
            speed = value;
            //Debug.Log("Please set the Rocket speed: " + speed);
        }
    }

    private void Start()
    {
        rocketRgb = GetComponent<Rigidbody>();     
        rocketFlySpeed = speed;
        //GameObject tempObj = GameObject.Find("[Code - Networking ]");
        //scriptInstance = tempObj.GetComponent<NetworkClient>();

        //Access dictio  variable from ScriptA
        
    }

    private void Update()
    {
        //rocketTarget = scriptInstance.serverObjects[targetId].transform.position;


        Vector3 direction = rocketTarget - transform.position;
        //Debug.Log("direction: " + direction);
        direction.Normalize();
        float rot = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        //Debug.Log("rot: " + rot);
        //float pitch = -Mathf.Asin(direction.y) * Mathf.Rad2Deg;
        ////Debug.Log("pitch: " + pitch);
        //if ((rot != rot) || (pitch != pitch))
        //{
        //    //Debug.Log("NAN: pitch: " + pitch);
        //    return;
        //}
        float pitch = -Mathf.Asin(direction.y) * Mathf.Rad2Deg;
        Vector3 currentRotation = new Vector3(pitch, rot, 0);
        //Vector3 currentRotation = new Vector3(/*pitch*/0, rot, 0);
        rocketRgb.rotation = Quaternion.Euler(currentRotation );
        //float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        movement = direction;
        transform.position = transform.position + (direction * speed * NetworkClient.SERVER_UPDATE_TIME * Time.deltaTime);
        transform.LookAt(rocketTarget);
        //Debug.Log("speed: " + speed);
    }

    private void FixedUpdate()
    {
        //if (!rocketRgb) { return; }
        //rocketRgb.MovePosition(transform.position + (direction * speed * NetworkClient.SERVER_UPDATE_TIME * Time.deltaTime));
        //Debug.Log("Please set the Rocket Target transform forawrd" + rocketLocalTransform.forward);
        //send forward vector and speed
        //RocketRgb.velocity = rocketLocalTransform.forward * speed * NetworkClient.SERVER_UPDATE_TIME * Time.deltaTime;

        //turn rocket towards target
        //send direction from server
        //rocketTargetRot will be sent from the server containing the visible targets from fieldofvieww class
        //var rocketTargetRot = Quaternion.LookRotation(RocketTarget.position - rocketLocalTransform.position);
        //RocketRgb.MoveRotation(Quaternion.RotateTowards(rocketLocalTransform.rotation, rocketTargetRot, turnSpeed));
    }
}
