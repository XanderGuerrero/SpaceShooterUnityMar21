using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoolDown
{

    private float length;
    private float currentTime;
    private bool onCooldown;

    public CoolDown(float Length = 0, bool StartWithCoolDown = false)
    {
        currentTime = 0;
        length = Length;
        onCooldown = StartWithCoolDown;
    }


    public void CooldownUpdate()
    {
        if (onCooldown)
        {
            currentTime += Time.deltaTime;

            if (currentTime >= length)
            {

                currentTime = 0;
                onCooldown = false;

            }
        }
    }

    public bool IsCoolDownOn()
    {
        return onCooldown;
    }


    // Start is called before the first frame update
    public void StartCoolDown()
    {
        onCooldown = true;
        currentTime = 0;
    }

}