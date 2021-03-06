using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputManager
{

    public static float MainHorizontal()
    {
        float r = 0.0f;
        r += Input.GetAxisRaw("J_MainHorizontal");
        r += Input.GetAxisRaw("K_MainHorizontal");

        return Mathf.Clamp(r, -1.0f, 1.0f);
    }

    public static float MainVertical()
    {
        float r = 0.0f;
        r += Input.GetAxisRaw("J_MainVertical");
        r += Input.GetAxisRaw("K_MainVertical");

        return Mathf.Clamp(r, -1.0f, 1.0f);
    }

    public static Vector3 MainJoystick()
    {
        return new Vector3(MainHorizontal(), 0, MainVertical());
    }

    public static float LeftJoyStick()
    {
        float r = 0.0f;
        r += Input.GetAxisRaw("J_MainHorizontal");
        r += Input.GetAxisRaw("J_MainVertical");

        return Mathf.Clamp(r, -1.0f, 1.0f);
    }
    public static float RightJoyStickHorizontal()
    {
        float r = 0.0f;
        r += Input.GetAxisRaw("K_MainHorizontal");
        //Debug.Log("RJH: " + r);
        return Mathf.Clamp(r, -1.0f, 1.0f);
    }

    public static float RightJoyStickVertical()
    {
        float r = 0.0f;
        r += Input.GetAxisRaw("K_MainVertical");
        //Debug.Log("RJV: " + r);
        return Mathf.Clamp(r, -1.0f, 1.0f);
    }

    public static float LeftJoyStickHorizontal()
    {
        float r = 0.0f;
        r += Input.GetAxisRaw("J_MainHorizontal");

        return Mathf.Clamp(r, -1.0f, 1.0f);
    }

    public static float LeftJoyStickVertical()
    {
        float r = 0.0f;
        r += Input.GetAxisRaw("J_MainVertical");

        return Mathf.Clamp(r, -1.0f, 1.0f);
    }

    public static Vector3 MainRightJoystick()
    {
        return new Vector3(RightJoyStickHorizontal(), RightJoyStickVertical(), 0);
    }
    public static Vector3 MainLeftJoystick()
    {
        return new Vector3(LeftJoyStickHorizontal(), LeftJoyStickVertical(), 0);
    }

    public static bool AButton()
    {
        return Input.GetButtonDown("A_Button");
    }
    public static bool BButton()
    {
        return Input.GetButtonDown("B_Button");
    }
    public static bool XButton()
    {
        return Input.GetButtonDown("X_Button");
    }
    public static bool YButton()
    {
        return Input.GetButtonDown("Y_Button");
    }
    public static float RightTrigger()
    {
        float r = 0.0f;
        r += Input.GetAxisRaw("RightTrigger");

        return Mathf.Clamp(r, 0.0f, 1.0f);
    }
}
