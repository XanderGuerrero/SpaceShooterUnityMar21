using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhoActivateMe : MonoBehaviour
{
    public string whoactivatedMe;

    public void SetActivator(string activator)
    {
        whoactivatedMe = activator;
    }

    public string GetActivator()
    {
        return whoactivatedMe;
    }
}