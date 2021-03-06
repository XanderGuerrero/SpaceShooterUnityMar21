using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class IndicatorPanel : MonoBehaviour
{
    public float viewRadius;
    //[Range(0, 360)]
    //public float viewAngle;
    public LayerMask targetMask;
    public LayerMask obstacleMask;
    //public float scanArea = 60;
    public List<GameObject> visibleTargets = new List<GameObject>();
    public Transform SqareTarget;

    void Start()
    {
        StartCoroutine("IndicateTargetsWithDelay",0);
    }


    IEnumerator IndicateTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    void FindVisibleTargets()
    {
        visibleTargets.Clear();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);



        foreach (Collider obj in targetsInViewRadius)
        {
            Debug.Log("inside the indicator");
            //get the objects screen position
            Vector3 screenPos = Camera.main.WorldToScreenPoint(obj.transform.position);
            //check if the objects screen position is actually on the screen
            //if it is, set the indicator properties and draw the indicator over the object
            if (screenPos.z > 0 &&
                screenPos.x > 0 && screenPos.x < Screen.width &&
                screenPos.y > 0 && screenPos.y < Screen.height)
            {
                GameObject target = obj.gameObject;
                visibleTargets.Add(target);


                visibleTargets = visibleTargets.OrderBy(x => Vector3.Distance(this.transform.position, x.transform.position)).ToList();
                Vector3 screenPosition = Camera.main.WorldToScreenPoint(visibleTargets[0].transform.position);
                SqareTarget.transform.localPosition = screenPosition;

            }
        }


        //for (int i = 0; i < targetsInViewRadius.Length; i++)
        //{
        //    GameObject target = targetsInViewRadius[i].gameObject;
        //    float cosAngle = Vector3.Dot((target.transform.position - this.transform.position).normalized, this.transform.forward);
        //    float angle = Mathf.Acos(cosAngle) * Mathf.Rad2Deg;
        //    if (angle < viewAngle)
        //    {
        //        Vector3 dirToTarget = (target.transform.position - transform.position).normalized;
        //        if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle)
        //        {
        //            float dstToTarget = Vector3.Distance(transform.position, target.transform.position);
        //            if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
        //            {
        //                visibleTargets.Add(target);

        //                visibleTargets = visibleTargets.OrderBy(x => Vector3.Distance(this.transform.position, x.transform.position)).ToList();

        //            }
        //        }
        //        //visibleTargets.Add(target);
        //    }

            //foreach (GameObject visibleTarget in visibleTargets)
            //{
            //    if ((angle > viewAngle) && visibleTargets.Contains(target))
            //    {
            //        visibleTargets.Remove(target);
            //    }
            //}

            //Vector3 dirToTarget = (target.position - transform.position).normalized;
            //if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            //{
            //    float dstToTarget = Vector3.Distance(transform.position, target.position);
            //    if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
            //    {
            //        visibleTargets.Add(target);
            //        Debug.Log("visibleTargets" + visibleTargets.Count);
            //    }
            //}
            //else
            //{
            //    if (visibleTargets.Contains(target))
            //        visibleTargets.Remove(target);
            //}
        //}
    }


    //public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    //{
    //    if (!angleIsGlobal)
    //    {
    //        angleInDegrees += transform.eulerAngles.y;
    //    }
    //    return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    //}
    //public Transform SqareTarget;
    ////public Transform ArrowTarget;
    // Transform[] ObjectsInGame;
    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}


    //void LateUpdate()
    //{
    //    Paint();
    //}

    //void Paint()
    //{
    //    ObjectsInGame = FindObjectsOfType(typeof(GameObject)) as Transform[];
    //    if(ObjectsInGame == null)
    //    {
    //        Debug.Log("Im empty");
    //    }
    //    else
    //    {
    //        foreach (Transform obj in ObjectsInGame)
    //        {
    //            //get the objects screen position
    //            Vector3 screenPos = Camera.main.WorldToScreenPoint(obj.transform.position);
    //            //check if the objects screen position is actually on the screen
    //            //if it is, set the indicator properties and draw the indicator over the object
    //            if (screenPos.z > 0 &&
    //                screenPos.x > 0 && screenPos.x < Screen.width &&
    //                screenPos.y > 0 && screenPos.y < Screen.height)
    //            {
    //                SqareTarget.transform.localPosition = screenPos;
    //            }
    //        }
    //    }

    //}
}
