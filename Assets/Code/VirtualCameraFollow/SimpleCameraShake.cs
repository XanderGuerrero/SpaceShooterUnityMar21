using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCameraShake : MonoBehaviour
{

    public float shakeDuration = 0.3f;
    public float shakeAmplitude = 1.2f;
    public float shakeFrequency = 2.0f;

    private float shakeElapsedTime = 0f;

    //Cinemachine shake
    public CinemachineVirtualCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin virtualCameraNoise;


    // Start is called before the first frame update
    void Start()
    {
        //get the virtual camera noise profile
        if(virtualCamera != null)
        {
            virtualCameraNoise = virtualCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();

        }
    }

    // Update is called once per frame
    void Update()
    {
        //if I hit the rt trigger
        if(Input.GetAxis("RightTriggerFire") > 0)
        {
            //Debug.Log("hi!!!");
            shakeElapsedTime = shakeDuration;
        }
        //if the cinemachine component is not set, avoid update
        if (virtualCamera != null || virtualCameraNoise != null)
        {
            //if shake  effect is still playing
            if(shakeElapsedTime > 0)
            {
                //set cinemachine camera noise paramters
                virtualCameraNoise.m_AmplitudeGain = shakeAmplitude;
                virtualCameraNoise.m_FrequencyGain = shakeFrequency;

                //update shake timer
                shakeElapsedTime -= Time.deltaTime;
            }
            else
            {
                //if camera shake effect is over, reset variables
                virtualCameraNoise.m_AmplitudeGain = 3f;
                virtualCameraNoise.m_FrequencyGain = 0.01f;
            }
        }
    }
}
