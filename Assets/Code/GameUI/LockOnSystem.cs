using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.UI;

public class LockOnSystem : MonoBehaviour
{  
    public GameObject player;
    public float viewRadius;
    public LayerMask targetMask;
    public List<GameObject> squareTargetLock = new List<GameObject>();
    public List<Collider> enemiesOnScreen = new List<Collider>();
    int count = 0;
    bool locked = false;


    void Start()
    {
   
        foreach (GameObject obj in squareTargetLock)
        {
            obj.SetActive(false);
        }
     
        StartCoroutine("FindTargetsWithDelay", 0f);
    }

    //void Awake()
    //{
    //    this.gameObject.SetActive(true);
    //    foreach (GameObject obj in squareTargetLock)
    //    {
    //        obj.SetActive(false);
    //    }

    //    StartCoroutine("FindTargetsWithDelay", 0f);
    //}

    void Update()
    {
        if (player.gameObject.activeInHierarchy ==  false)
        {
            Debug.Log("TURN OFF BRO");
            foreach (GameObject obj in squareTargetLock)
            {
                obj.SetActive(false);
            }
            StopCoroutine("FindTargetsWithDelay");
        }
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
        
    }

    void FindVisibleTargets()
    {
        //find all the enemies within the overlap shpere and put into an array
        Collider[] targetsInViewRadius = Physics.OverlapSphere(player.transform.position, viewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            //Debug.Log("HEY");

            //get screen coordinates
            Vector3 enemyPos = Camera.main.WorldToScreenPoint(targetsInViewRadius[i].transform.position);


            //check if obj is on the screen in real time
            bool isOnScreen = (enemyPos.z >= 0 &&
                enemyPos.x >= 0 && enemyPos.x <= Screen.width &&
                enemyPos.y >= 0 && enemyPos.y <= Screen.height) ? true : false;
            //var distance = Vector3.Distance(player.transform.position, enemyPos);

            //if this obj is on screen and not in the enemiesOnScreen list
            if (isOnScreen && !enemiesOnScreen.Contains(targetsInViewRadius[i]))
            {
                if (count < 5)
                {
                    //Debug.Log("hi ");
                    enemiesOnScreen.Add(targetsInViewRadius[i]);
                    count++;
                }

            }

            //if the ai is on the list of on screen enemies but is not on screen
            if (enemiesOnScreen.Contains(targetsInViewRadius[i]) && !isOnScreen)
            {
         
                var ind = enemiesOnScreen.IndexOf(targetsInViewRadius[i]);
                squareTargetLock[ind].SetActive(false);
     

                //remove the ai
                enemiesOnScreen.Remove(targetsInViewRadius[i]);

                if (count == 0)
                {
                    count = 0;
                }
                else
                {
                    --count;
                }
            }
        }

        //button A
        //if the butoon a is pressed is the system is not locked and there are enemies on screen
        if (Input.GetButtonDown("Fire3") && !locked && enemiesOnScreen.Count > 0)
        {
            locked = true;

            Debug.Log("TARGETS LOCKED");      
        }

        //button A again
        else if (Input.GetButtonDown("Fire3") && locked)
        {
            for (int i = 0; i < squareTargetLock.Count; i++)
            {
                squareTargetLock[i].transform.position = Vector3.zero;
                squareTargetLock[i].SetActive(false);
            }

            locked = false;

            Debug.Log("TARGET UNLOCKED");
        }


        if (locked)
        {
            if (enemiesOnScreen.Any())
            {
                //loop through all the enemy on screen we have in our array
                //these are the enemy names we have filtered in our search area
                //using the overlapshpere
                for (int i = 0; i < enemiesOnScreen.Count; i++)
                {
                    Vector3 enemyPos = Camera.main.WorldToScreenPoint(enemiesOnScreen[i].transform.position);

                    bool isOnScreenFinalCheck = (enemyPos.z >= 0 &&
                    enemyPos.x >= 0 && enemyPos.x <= Screen.width &&
                    enemyPos.y >= 0 && enemyPos.y <= Screen.height) ? true : false;

                    if (isOnScreenFinalCheck)
                    {
                        squareTargetLock[i].transform.position = Vector3.Slerp(squareTargetLock[i].transform.position, new Vector3(enemyPos.x, enemyPos.y, 0), Time.deltaTime * NetworkClient.SERVER_UPDATE_TIME * 5);
                        squareTargetLock[i].GetComponent<Image>().color = Color.red;
                        squareTargetLock[i].SetActive(true);
                    }

                    //Debug.Log(i);
                    if (enemiesOnScreen[i].gameObject.activeInHierarchy == false)
                    {
                        squareTargetLock[i].SetActive(false);
                    }
                }
            }
            else
            {
                foreach (GameObject target in squareTargetLock)
                {
                    target.SetActive(false);
                }
            }
        }
    }
}

