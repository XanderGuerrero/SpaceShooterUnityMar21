using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManagerTest : MonoBehaviour
{
    private Transform PlayerTransform;
    private Rigidbody rb;
    private Vector3 movement;
    [SerializeField]
    private Transform bulletSpawnPoint1;
    [SerializeField]
    private Transform bulletSpawnPoint2;
    [SerializeField]
    private Transform missileSpawnPoint1;
    [SerializeField]
    private Transform missileSpawnPoint2;
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
    private CoolDown shootingCoolDownMissile;
    public float ForwardSpeed = 300;
    private float activeForwardSpeed = 2f;
    private float forwardAcceleration = 75f;
    public float lookRateSpeed = 90f;
    private float rollInput;
    public float rollSpeed = 2f;
    public float rollAAcceleration = 2f;
    [Header("Class References")]
    [SerializeField]
    private NetworkIdentity networkIdentity;
    private float lastTilt;
    private MissileData missileData;
    private Transform MissileLocalTransform;
    LockOnSystem target;
    private GameObject engineLight;
    private Light engLight;
    private float intensitySmooth = 2f;
    public ParticleSystem muzzleFlashRT;
    public ParticleSystem muzzleFlashLT;
    AudioManager audioSource;
    public AudioClip PlayerShot;
    public AudioClip Missile;
    public AudioClip EngineSound;
    public AudioClip EngineShutdownSound;
    //public float volume = 0.5f;
    private IEnumerator coroutine;

    void Start()
    {
        //get the targets Id
        target = this.gameObject.GetComponentInChildren<LockOnSystem>();
        shootingCoolDown = new CoolDown(.15f);
        bulletData = new BulletData();
        bulletData.position = new Position();
        bulletData.direction = new Position();
        rb = GetComponent<Rigidbody>();
        shootingCoolDownMissile = new CoolDown(.15f);
        missileData = new MissileData();
        missileData.position = new Position();
        missileData.direction = new Position();
        missileData.target = new Position();
        MissileLocalTransform = this.GetComponent<Transform>();
        engineLight = this.gameObject.transform.GetChild(1).gameObject;
        engLight = engineLight.GetComponentInChildren<Light>();
        audioSource = FindObjectOfType<AudioManager>();
    }



    public void Update()
    {
        if (networkIdentity.IsControlling())
        {
            checkShooting();
        }
    }

    void FixedUpdate()
    {
        if (networkIdentity.IsControlling())
        {
            checkMovement();

            checkAiming();
   
            checkTilt();
            //checkShooting();

            //deltaRotation = Quaternion.Euler(
            //new Vector3(currentXrotation * lookRateSpeed * Time.deltaTime, currentYrotation * lookRateSpeed * Time.deltaTime, -rollInput * rollSpeed * Time.deltaTime)
            //);

            //rb.MoveRotation(deltaRotation);
            transform.Rotate(new Vector3(-currentXrotation * lookRateSpeed * Time.deltaTime, currentYrotation * lookRateSpeed * Time.deltaTime, -rollInput * rollSpeed * Time.deltaTime), Space.Self);
        }
    }

    private void checkAiming()
    {
        Vector3 Rinputs = InputManager.MainRightJoystick();

        xRotation = Rinputs.y;

        yRotation = Rinputs.x;
        //Debug.Log("trying to LOOK: " + xRotation + " " + yRotation);
        currentXrotation =  Mathf.SmoothDamp(currentXrotation, xRotation, ref rotationXveloctiy, 0.1f);

        currentYrotation =  Mathf.SmoothDamp(currentYrotation, yRotation, ref rotationYveloctiy, 0.1f);
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
                muzzleFlashRT.Play();
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
                //3Debug.Log("send bullet data: " + bulletSpawnPoint2.forward.z.TwoDecimals());
                muzzleFlashLT.Play();
                alternateBulletShotSpawn = 0;
            }
            //send the bullet
            networkIdentity.GetSocket().Emit("fireBullet", new JSONObject(JsonUtility.ToJson(bulletData)));
            //audioSource.PlayOneShot(clip, volume);
            audioSource.PlaySFX(PlayerShot, .3f);
        }

        shootingCoolDownMissile.CooldownUpdate();
        if ((Input.GetButtonDown("Fire4")) && (!shootingCoolDownMissile.IsCoolDownOn()) && target.enemiesOnScreen.Count > 0)
        {
            //Debug.Log("pressing right bumper to fire missile");
            shootingCoolDownMissile.StartCoolDown();
            coroutine = WaitAndFire(0.25f);
            StartCoroutine(coroutine);
            

        }
    }

    void OnDisable()
    {
        RadarCameraFollowPlayer.firstTime = true;
        AimFollowPlayer.firstTime = true;
        Debug.Log("stoping missile coroutine");
        StopCoroutine("WaitAndFire");
    }

    IEnumerator WaitAndFire(float waitTime)
    {
        if (target.enemiesOnScreen.Count > 0)
        {
       
            for (int i = 0; i < target.enemiesOnScreen.Count; i++)
            {
                yield return new WaitForSeconds(waitTime);
                if (alternateBulletShotSpawn == 0)
                {
                    //define bullet in first shot spawn
                    missileData.activator = NetworkClient.ClientID;
                    GameObject obj = target.enemiesOnScreen[i].gameObject;
                    NetworkIdentity Hey = obj.GetComponent<NetworkIdentity>(); ;
                    missileData.targetId = Hey.GetID();
                    //Debug.Log("target.visibleTargets[0].gameObject.GetComponent<NetworkIdentity>().GetID(): " + missileData.targetId);        
                    missileData.target.x = target.enemiesOnScreen[i].gameObject.transform.position.x;
                    missileData.target.y = target.enemiesOnScreen[i].gameObject.transform.position.y;
                    missileData.target.z = target.enemiesOnScreen[i].gameObject.transform.position.z;
                    //target.visibleTargets.RemoveAt(0);
                    //Debug.Log("PlayerFieldOfView.visibleTargets[0].x: " + missileData.target.x);   //.name.ToString());
                    missileData.position.x = missileSpawnPoint1.position.x.TwoDecimals();
                    missileData.position.y = missileSpawnPoint1.position.y.TwoDecimals();
                    missileData.position.z = missileSpawnPoint1.position.z.TwoDecimals();
                    missileData.direction.x = missileSpawnPoint1.forward.x.TwoDecimals();
                    missileData.direction.y = missileSpawnPoint1.forward.y.TwoDecimals();
                    missileData.direction.z = missileSpawnPoint1.forward.z.TwoDecimals();
                    //Debug.Log("send bullet data: " + bulletSpawnPoint1.forward.z.TwoDecimals());
                    alternateBulletShotSpawn = 1;
                    //Debug.Log("FIRE MISSILE " + i);
                }
                else
                {
                    //define bullet in second shotspawn
                    missileData.activator = NetworkClient.ClientID;
                    //missileData.targetId = target.visibleTargets[0].gameObject.GetComponent<NetworkIdentity>().GetID();
                    GameObject obj = target.enemiesOnScreen[i].gameObject;
                    NetworkIdentity Hey = obj.GetComponent<NetworkIdentity>(); ;
                    // NetworkIdentity Hey = target.visibleTargets[0].gameObject.GetComponent<NetworkIdentity>(); ;
                    missileData.targetId = Hey.GetID();
                    //Debug.Log("PlayerFieldOfView.visibleTargets[0].name.ToString(): " + target.visibleTargets[0].GetComponent<NetworkIdentity>().GetID());   //.name.ToString());
                    missileData.target.x = target.enemiesOnScreen[i].gameObject.transform.position.x;
                    missileData.target.y = target.enemiesOnScreen[i].gameObject.transform.position.y;
                    missileData.target.z = target.enemiesOnScreen[i].gameObject.transform.position.z;
                    //target.visibleTargets.RemoveAt(0);
                    missileData.position.x = missileSpawnPoint2.position.x.TwoDecimals();
                    missileData.position.y = missileSpawnPoint2.position.y.TwoDecimals();
                    missileData.position.z = missileSpawnPoint2.position.z.TwoDecimals();
                    missileData.direction.x = missileSpawnPoint2.forward.x.TwoDecimals();
                    missileData.direction.y = missileSpawnPoint2.forward.y.TwoDecimals();
                    missileData.direction.z = missileSpawnPoint2.forward.z.TwoDecimals();
                    //3Debug.Log("send bullet data: " + bulletSpawnPoint2.forward.z.TwoDecimals());
                    alternateBulletShotSpawn = 0;
                    //Debug.Log("FIRE MISSILE " + i);
                }
                //send the bullet
                //Debug.Log("trying to LOOK: "+ missileData);
                networkIdentity.GetSocket().Emit("fireMissile", new JSONObject(JsonUtility.ToJson(missileData)));
                audioSource.PlaySFX(Missile, .5f);
            }
            //print("WaitAndPrint " + Time.time);
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
        if (Input.GetAxis("Fire2") == 1f)
        {

            engLight.intensity = Mathf.Lerp(engLight.intensity, 2f, Time.deltaTime * NetworkClient.SERVER_UPDATE_TIME);
            activeForwardSpeed =  Mathf.Lerp(activeForwardSpeed, Input.GetAxisRaw("Fire2") * ForwardSpeed, forwardAcceleration * Time.deltaTime);
            rb.velocity += transform.forward * activeForwardSpeed * Time.deltaTime;
        }
        else if (Input.GetAxis("Fire2") < .75f)
        {

            engLight.intensity = Mathf.Lerp(engLight.intensity, .5f, Time.deltaTime * NetworkClient.SERVER_UPDATE_TIME);
        }
    }

    private void checkTilt()
    {
      
        //moving right
        if ((InputManager.MainLeftJoystick().x > 0.2f) && (InputManager.MainLeftJoystick().x <= 1f))
        {
            rollInput = Mathf.Lerp(rollInput, InputManager.MainLeftJoystick().x, rollAAcceleration * Time.deltaTime);
        }
        //moving left
        if ((InputManager.MainLeftJoystick().x < -0.2f) && (InputManager.MainLeftJoystick().x >= -1f))
        {
            rollInput = Mathf.Lerp(rollInput, InputManager.MainLeftJoystick().x, rollAAcceleration * Time.deltaTime);
        }
        if((InputManager.MainLeftJoystick().x < 0.2f) && (InputManager.MainLeftJoystick().x > -0.2f))
        {
            //Debug.Log("checkTilt left: " + InputManager.MainLeftJoystick().x);
            rollInput = Mathf.Lerp(rollInput, 0, 10 * Time.deltaTime);
         
        }
       
    }

}
