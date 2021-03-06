using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    //[Header("Data")]
    //[SerializeField]
    public float tilt;
    private float speed = 50;
    private Transform PlayerTransform;
    private Rigidbody rb;
    private Vector3 movement;
    [SerializeField]
    private Transform bulletSpawnPoint1;
    [SerializeField]
    private Transform bulletSpawnPoint2;
    private int alternateBulletShotSpawn = 0;
    float xRotation = 0f;
    float yRotation = 0f;
    //float zRotation = 0f;
    public float currentXrotation;
    public float currentYrotation;
    public float currentZrotation;
    public float rotationYveloctiy = 15f;
    public float rotationXveloctiy = 15f;
    public float rotationZveloctiy = 15f;
    //shooting
    private BulletData bulletData;
    private CoolDown shootingCoolDown;
    public float RotationalControl;
    public float Acceleration;
    public float Speed;
    public int AccelerationCounter = 0;
    //[Header("Class References")]
    //[SerializeField]
    //private Rigidbody rb;

    [Header("Class References")]
    [SerializeField]
    private NetworkIdentity networkIdentity;

    //private float lastRotation;
    private float lastTilt;


    void Start()
    {
        //screenRect = new Rect(0, 0, Screen.width, Screen.height);
        //mainCamera = FindObjectOfType<Camera>();
        shootingCoolDown = new CoolDown(.15f);
        bulletData = new BulletData();
        bulletData.position = new Position();
        bulletData.direction = new Position();
        ////controller = GetComponent<CharacterController>();
       rb = GetComponent<Rigidbody>();
    }



    public void Update()
    {
        //if (!screenRect.Contains(Input.mousePosition))
        //    return;
        if (networkIdentity.IsControlling())
        {
            checkShooting();
        }
    }

    void FixedUpdate()
    {
        //if (!screenRect.Contains(Input.mousePosition))
        //    return;
        if (networkIdentity.IsControlling())
        {
            //movementUpDown();
            checkMovement();
            //checkAiming();
            //ClampingSpeedValues();
            checkAiming();
            //swerve();
            checkTilt();
            checkShooting();
            //rb.AddRelativeForce(Vector3.up * 100);
            rb.rotation = Quaternion.Euler(
            new Vector3(currentXrotation, currentYrotation, 0/*-currentZrotation*/)
            );
            //Debug.Log("rotation of ship: " + rb.rotation);
        }
    }

    private void checkAiming()
    {

        //if (!screenRect.Contains(Input.mousePosition))
        //    return;


        ////get input
        //Vector3 Linputs = InputManager.MainLeftJoystick();
        Vector3 Rinputs = InputManager.MainRightJoystick();
        Vector3 Linputs = InputManager.MainLeftJoystick();
        //zRotation -= Linputs.x;

        xRotation -= Rinputs.y;

        yRotation += Rinputs.x;

        currentXrotation = Mathf.SmoothDamp(currentXrotation, xRotation, ref rotationXveloctiy, 0.001f);

        currentYrotation = Mathf.SmoothDamp(currentYrotation, yRotation, ref rotationYveloctiy, 0.001f);

        //currentZrotation = Mathf.SmoothDamp(currentZrotation, zRotation, ref rotationZveloctiy, 0.15f);


    }

    private void checkShooting()
    {
        shootingCoolDown.CooldownUpdate();
        if ((Input.GetAxis("RightTriggerFire") > 0) && (!shootingCoolDown.IsCoolDownOn()))
        {
            shootingCoolDown.StartCoolDown();

            if (alternateBulletShotSpawn == 0)
            {
                //define bullet in first shot spawn
                bulletData.activator = NetworkClient.ClientID;
                bulletData.position.x = bulletSpawnPoint1.position.x.TwoDecimals();
                bulletData.position.y = bulletSpawnPoint1.position.y.TwoDecimals();
                bulletData.position.z = bulletSpawnPoint1.position.z.TwoDecimals();
                bulletData.direction.x = bulletSpawnPoint1.forward.x.TwoDecimals();
                bulletData.direction.y = bulletSpawnPoint1.forward.y.TwoDecimals();
                bulletData.direction.z = bulletSpawnPoint1.forward.z.TwoDecimals();
                //Debug.Log("send bullet data: " + bulletSpawnPoint1.forward.z.TwoDecimals());
                alternateBulletShotSpawn = 1;
            }
            else
            {
                //define bullet in second shotspawn
                bulletData.activator = NetworkClient.ClientID;
                bulletData.position.x = bulletSpawnPoint2.position.x.TwoDecimals();
                bulletData.position.y = bulletSpawnPoint2.position.y.TwoDecimals();
                bulletData.position.z = bulletSpawnPoint2.position.z.TwoDecimals();
                bulletData.direction.x = bulletSpawnPoint2.forward.x.TwoDecimals();
                bulletData.direction.y = bulletSpawnPoint2.forward.y.TwoDecimals();
                bulletData.direction.z = bulletSpawnPoint2.forward.z.TwoDecimals();
                Debug.Log("send bullet data: " + bulletSpawnPoint2.forward.z.TwoDecimals());
                alternateBulletShotSpawn = 0;
            }
            //send the bullet
            networkIdentity.GetSocket().Emit("fireBullet", new JSONObject(JsonUtility.ToJson(bulletData)));
        }
    }

    //access to get the last rotation
    public float GetLastTilt()
    {
        return lastTilt;
    }

    //not our player but setting their rotations on our side
    public void SetTilt(float value)
    {
        rb.rotation = Quaternion.Euler(0.0f, 0.0f, value);
    }

    private void checkMovement()
    {
        //Vector3 Linputs = InputManager.MainLeftJoystick();
        //transform.position += new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0) * speed * Time.deltaTime;
        //Debug.Log("foraward: " + Input.GetAxis("Fire2"));
        //movement using rigidbody
        //moving forward
        if (Input.GetAxis("Fire2") == 1f)
        {
            //    Vector3 Direction = transform.position - rb.position;//problem here not an actual direction
            //Direction.Normalize();

            //float cross = Vector3.Cross(Direction, transform.forward).z;
            //    rb.angularVelocity = RotationalControl * new Vector3(0,0, cross);

            //Vector3 Velocity = transform.forward * Acceleration;
            //    rb.AddForce(Velocity);

            //float thrustForce = Vector3.Dot(rb.velocity, transform.forward);

            //Vector3 relforce = transform.forward * thrustForce;
            //    rb.AddForce(relforce);

            //if(rb.velocity.magnitude > Speed)
            //{
            //    rb.velocity = rb.velocity.normalized * Speed;
            //}
            //AccelerationCounter += AccelerationCounter - 1;
            //Debug.Log("AccelerationCounter: " + AccelerationCounter);
            //Vector3 Vel = transform.forward * (AccelerationCounter * Acceleration);
            //rb.AddForce(Vel);
            //calculate the direction
            //
            //float Dir = Vector3.Dot(rb.velocity, (Vector3.forward));

            // Debug.Log("foraward: " + Input.GetAxis("Fire2"));
            //rb.AddRelativeForce(Vector3.forward * Speed);
            //if(Acceleration > 0)
            //{
            //    if(Dir > 0)
            //    {
            //        rb.rotation += Vector3 * RotationalControl * (rb.velocity.magnitude / Speed);
            //    }
            //    else
            //    {
            //        rb.rotation -= 
            //    }
            //}
            //movement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0.0f);

            rb.velocity = transform.forward * speed;
            rb.AddRelativeForce(Vector3.forward * 50000);
        }
        //movement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0.0f);

        rb.velocity = new Vector3(0,0,0) * speed;

    }

    private void checkTilt()
    {
        //ship tilt on moving left and right
        var Tilt = (rb.velocity.y) * -tilt;
        lastTilt = Tilt;
        rb.rotation = Quaternion.Euler(0.0f, 0.0f, Tilt);

        
    }
}
