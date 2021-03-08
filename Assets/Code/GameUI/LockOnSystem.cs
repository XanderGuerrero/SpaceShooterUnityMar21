using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.UI;

public class LockOnSystem : MonoBehaviour
{
    bool locked;
    public float viewRadius;
    public LayerMask targetMask;
    public LayerMask obstacleMask;
    public List<Collider> visibleTargets = new List<Collider>();
    public List<GameObject> squareTargetLock = new List<GameObject>();
    public GameObject player;
    private List<GameObject> go = new List<GameObject>();
    public List<Transform> targets = new List<Transform>();
    //public List<GameObject> enemiesInGame = new List<GameObject>();
    public List<Collider> enemiesOnScreen = new List<Collider>();
    NetworkClient ScriptListofEnemies;
    int index = 0;
    public List<string> enemyName = new List<string>();
     int count = 0;
    //public RectTransform mCanvas;
    //public Camera mCamera;


    // Start is called before the first frame update
    void Start()
    {
        foreach(GameObject obj in squareTargetLock)
        {
            obj.SetActive(false);
        }
        //crosshair.SetActive(false);
        StartCoroutine("FindTargetsWithDelay", 0f);

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
        //enemiesOnScreen.Clear();
        //Debug.Log("hi " );
        Collider[] targetsInViewRadius = Physics.OverlapSphere(player.transform.position, viewRadius, targetMask);
        //Debug.Log("hi ");
        if (targetsInViewRadius.Any())
        {
           
            for (int i = 0; i < targetsInViewRadius.Length; i++)
            {
                //Debug.Log("HEY");

                Vector3 enemyPos = Camera.main.WorldToScreenPoint(targetsInViewRadius[i].transform.position);
                //enemyPos.x *= mCanvas.rect.width / (float)Camera.main.pixelWidth;
                //enemyPos.y *= mCanvas.rect.height / (float)Camera.main.pixelHeight;
                bool isOnScreen = (enemyPos.z >= 0 &&
                    enemyPos.x >= 0 && enemyPos.x <= Screen.width &&
                    enemyPos.y >= 0 && enemyPos.y <= Screen.height) ? true : false;
                var distance = Vector3.Distance(player.transform.position, enemyPos);

                if (isOnScreen && !enemiesOnScreen.Contains(targetsInViewRadius[i]) && distance < 100000f)
                {
                   if(count <= 4)
                    {
                        //Debug.Log("hi ");
                        enemiesOnScreen.Insert(count, targetsInViewRadius[i]);
                        count++;                    
                    }
                    if (!visibleTargets.Contains(targetsInViewRadius[i]))
                    {
                        //Debug.Log("hi ");
                        visibleTargets.Add(targetsInViewRadius[i]);
                    }

                }

                if (enemiesOnScreen.Contains(targetsInViewRadius[i]) && isOnScreen == false)
                {
                    //Debug.Log("hi ");
                    //locked = false;
                    //targets = null;
                    var ind = enemiesOnScreen.IndexOf(targetsInViewRadius[i]);
                    squareTargetLock[ind].SetActive(false);
                    //Debug.Log(ind);
                    enemiesOnScreen.Remove(targetsInViewRadius[i]);
                    //enemyName.Remove(targetsInViewRadius[i].name);
                    --count;
                    //Debug.Log(count);
                }
 

                //GameObject target = targetsInViewRadius[i].gameObject;

                //visibleTargets.Add(target);
            }
        }

     
        //button A
        if (Input.GetButtonDown("Fire3") && !locked && enemiesOnScreen.Count > 0)
        {
            index = 0;
            if (enemiesOnScreen.Any())
            {
                //Debug.Log(enemiesOnScreen.Count);
                for (int i = 0; i < enemiesOnScreen.Count; i++)
                {
                    if (enemiesOnScreen[i] != null)
                    {
                        //Debug.Log(i);
                        
                        //Debug.Log(enemiesOnScreen[i].name);
                        //Debug.Log(enemyName.Count());
                        //Debug.Log(enemyName[index].ToString());
                        //Debug.Log(enemyName.Count());
                        enemyName.Add( enemiesOnScreen[i].name);
                        //Debug.Log(enemyName[i].ToString());
                        squareTargetLock[i].SetActive(true);
                        Debug.Log(i);
                    }
                }
                //index = 0;
                //enemyName = enemiesOnScreen[0].name;
                locked = true;
                if(enemiesOnScreen.Any())
                {
                    Debug.Log("TARGETS LOCKED");
                }
                else
                {
                    Debug.Log("TARGETS LOCKED but no enemies to lock on to");
                }
                //crosshair[0].SetActive(true);
                

                //foreach (string str in enemyName)
                //{
                //    Debug.Log(str);
                //}
            }
            
        }
        //button A again
        else if (Input.GetButtonDown("Fire3") && locked)
        {
            for (int i = 0; i < squareTargetLock.Count; i++)
            {
                
                squareTargetLock[i].SetActive(false);
            }
            enemyName.Clear();
            locked = false;
            //enemyName = string.Empty;
            targets.Clear();// = null;

            //crosshair[0].SetActive(false);
            Debug.Log("TARGET UNLOCKED");
        }


        //Right Bumper - switch targets
        if (Input.GetButtonDown("Fire1"))
        {
            if (enemiesOnScreen.Count > 0)
            {
                if (index < enemiesOnScreen.Count)
                {
                    if (index == enemiesOnScreen.Count)
                    {
                        index = 0;
                    }
                    else
                    {
                        var ind = ++index;
                        //enemyName[0] = enemiesOnScreen[4].name;
                        if (!enemyName.Contains(enemiesOnScreen[ind].name))
                        {
                            enemyName.Insert(0, (visibleTargets.Find(item => item.name == enemiesOnScreen[ind].name).name));
                            enemyName.RemoveAt(enemyName.Count - 1);
                            Debug.Log("index" + index);
                            Debug.Log(enemiesOnScreen.Count);
                        }             
                        
                    }
                

                    
                }

            }
        }

        if (locked)
        {
            if (!enemiesOnScreen.Any())
            {
                locked = false;
            }
            else
            {
                //loop through all the enemy names we have in our array
                //these are the enemy names we have filtered in our search area
                //using the overlapshpere
                for(int i = 0; i < enemyName.Count; i++)
                {
                    //Debug.Log(!targets.Any());
                    //Debug.Log(targets.Count);
                    //if the list is empty
                    if (targets.Count < 5 || !targets.Any())
                    {
                        //Debug.Log(!targets.Any());
                       //add the ai transform to the list of targets
                       //Debug.Log("Hey");
                        //if we have a name to find info on
                        if(enemyName[i] != null)
                        {
                            //add to the list of targets
                            targets.Add(visibleTargets.Find(item => item.name == enemyName[i]).transform);
                            go.Add(visibleTargets.Find(item => item.name == enemyName[i]).gameObject);
                            //Debug.Log(go[i].name);
                        }
                     
                    }
                    //Debug.Log("Hey");
                    //Debug.Log(i);
                    //Debug.Log(targets[i].transform.position);
                    //Vector3[] enemyPos = new Vector3[5];
                    Vector3 enemyPos = Camera.main.WorldToScreenPoint(targets[i].transform.position);
                    //enemyPos.x *= mCanvas.rect.width / (float)Camera.main.pixelWidth;
                    //enemyPos.y *= mCanvas.rect.height / (float)Camera.main.pixelHeight;
                    //Debug.Log(enemyPos);
                    //Debug.Log(i);
                    bool isOnScreenFinalCheck = (enemyPos.z >= 0 &&
                    enemyPos.x >= 0 && enemyPos.x <= Screen.width &&
                    enemyPos.y >= 0 && enemyPos.y <= Screen.height) ? true : false;
                    //Debug.Log(enemyPos.ToString());
                    //Debug.Log("Hey");
                    if (isOnScreenFinalCheck)
                    {
                        //Debug.Log("Hey");
                        // Debug.Log("target: " + target.transform.position);
                        //target = visibleTargets.Find(item => item.name == enemyName).transform;
                        // target = enemiesOnScreen[index].transform;
                        squareTargetLock[i].transform.position = Vector3.Slerp(squareTargetLock[i].transform.position, new Vector3(enemyPos.x, enemyPos.y, 0), Time.deltaTime * NetworkClient.SERVER_UPDATE_TIME);
                        // Debug.Log("target: " + target.transform.position);
                        //Debug.Log("Hey " + squareTargetLock[i].transform.position);
                        //crosshair.transform.position = Camera.main.WorldToScreenPoint(target.position);
    
                        
                        squareTargetLock[i].SetActive(true);
                        //Debug.Log("Hey " + enemyName[i].ToString());
                        //Debug.Log(index);
                        //Debug.Log(enemyName.Count);
                        //Debug.Log(i);
                        //Debug.Log("target: " + target.transform.position);
                    }
                    if (go[i].activeSelf == false)
                    {
                        Debug.Log("TURNING OFF: " );
                        locked = false;
                        //enemyName.RemoveAt(i);// = string.Empty;
                        targets.RemoveAt(i);// = null;
                        enemyName.Clear();
                        foreach (GameObject target in squareTargetLock)
                        {
                            target.SetActive(false);
                            //Debug.Log(str);
                        }
                        //squareTargetLock.ElementAt(i).SetActive(false);
                        //enemiesOnScreen.Clear();
                        //enemiesOnScreen.RemoveAt(i);
                        //Debug.Log("target: " + target.transform.position);
                    }
                    //if (go[i].activeSelf == false)
                    //{
                    //    Debug.Log(i);
                    //    Debug.Log("TURNING OFF: " );
                    //    //locked = false;
                    //    enemyName[i] = string.Empty;
                    //    targets[i] = null;
                    //    squareTargetLock[i].SetActive(false);
                    //    //enemiesOnScreen.Clear();
                    //    //break;
                    //    //Debug.Log("target: " + target.transform.position);
                    //    i = 0;
                    //    //shift the elements over and add new ai target to the end of the list
                    //}
                    //Debug.Log("HEY");
                    //Debug.Log(index);
                }
                //Debug.Log("HEY");
            }
        }
    }



    //    void FixedUpdate()
    //{
    //    visibleTargets.Clear();
    //    Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

    //    for (int i = 0; i < targetsInViewRadius.Length; i++)
    //    {
    //        GameObject target = targetsInViewRadius[i].gameObject;
      
    //        visibleTargets.Add(target);       
    //    }


    //    if (visibleTargets.Count > 0)
    //    {
    //        //Debug.Log("I have things in the list: " + visibleTargets.Count);
    //        for(int i = 0; i < visibleTargets.Count; i++)
    //        {
    //            //Debug.Log("visibleTargets[i].transform.position: " + visibleTargets[i].transform.position);
    //            Vector3 enemyPos = Camera.main.WorldToScreenPoint(visibleTargets[i].transform.position);
    //            //Debug.Log("enemyPos " + enemyPos);
    //            bool isOnScreen = (enemyPos.z > 0 &&
    //                enemyPos.x > 0 && enemyPos.x < Screen.width &&
    //                enemyPos.y > 0 && enemyPos.y < Screen.height) ? true : false;

    //            //Debug.Log("isOnScreen" + isOnScreen);
    //            if (isOnScreen && !enemiesOnScreen.Contains(visibleTargets[i]))
    //            {
    //                //Debug.Log("add target to list"  );
    //                enemiesOnScreen.Add(visibleTargets[i]);
                  
    //            }
    //            else if (enemiesOnScreen.Contains(visibleTargets[i]) && !isOnScreen)
    //            {
    //                locked = false;
    //                enemiesOnScreen.Remove(visibleTargets[i]);
    //                //Debug.Log("locked " + locked);
    //            }
             
                
    //        }
            

    //    }
    //    //Debug.Log(Input.GetButtonDown("Fire1") + " " + locked + " " + enemiesOnScreen.Count);
    //    if (Input.GetButtonDown("Fire1") && !locked && enemiesOnScreen.Count > 0)
    //    {
    //        //Debug.Log("HI2");
    //        //Debug.Log("pressing Fire1");
    //        index = 0;
    //        locked = true;
    //        crosshair.SetActive(true);
    //    }
    //    else if (Input.GetButtonDown("Fire1") && locked)
    //    {
    //        //Debug.Log("pressing Fire1");
    //        locked = false;
    //        target = null;
    //        crosshair.SetActive(false);
    //    }

    //    if (Input.GetButtonDown("Fire4"))
    //    {
    //        Debug.Log("pressing fire4");
    //        if (enemiesOnScreen.Count > 0)
    //        {
    //            index++;
    //            if (index >= enemiesOnScreen.Count)
    //            {
    //                index = 0;
    //            }

    //        }
    //    }

    //    if (locked)
    //    {
    //        target = enemiesOnScreen[index].transform;
    //        crosshair.transform.position = Camera.main.WorldToScreenPoint(target.position);
    //    }
    //}
}
