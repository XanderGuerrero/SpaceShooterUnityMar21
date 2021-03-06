using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBoundary : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -100f, 100f), Mathf.Clamp(transform.position.y, -100f, 100f), transform.position.z);
    }
}
