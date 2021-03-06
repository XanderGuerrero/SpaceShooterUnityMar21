using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LockOnSystem : MonoBehaviour
{
    bool locked;
    public float viewRadius;
    public LayerMask targetMask;
    public LayerMask obstacleMask;
    public List<Collider> visibleTargets = new List<Collider>();
    public GameObject crosshair;
    public GameObject player;
    public GameObject go;
    public Transform target;
    //public List<GameObject> enemiesInGame = new List<GameObject>();
    public List<Collider> enemiesOnScreen = new List<Collider>();
    NetworkClient ScriptListofEnemies;
    int index = 0;
    public string enemyName = string.Empty;
    //public RectTransform mCanvas;
    //public Camera mCamera;


    // Start is called before the first frame update
    void Start()
    {
        crosshair.SetActive(false);
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
        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
 
            Vector3 enemyPos = Camera.main.WorldToScreenPoint(targetsInViewRadius[i].transform.position);
            //enemyPos.x *= mCanvas.rect.width / (float)Camera.main.pixelWidth;
            //enemyPos.y *= mCanvas.rect.height / (float)Camera.main.pixelHeight;
            bool isOnScreen = (enemyPos.z >= 0 &&
                enemyPos.x >= 0 && enemyPos.x <= Screen.width &&
                enemyPos.y >= 0 && enemyPos.y <= Screen.height) ? true : false;
            var distance = Vector3.Distance(player.transform.position, enemyPos);

            if (isOnScreen && !enemiesOnScreen.Contains(targetsInViewRadius[i]) && distance < 100000f)
            {
                enemiesOnScreen.Add(targetsInViewRadius[i]);
                if (!visibleTargets.Contains(targetsInViewRadius[i]))
                {

                    visibleTargets.Add(targetsInViewRadius[i]);
                }
                   
            }
  
            if (enemiesOnScreen.Contains(targetsInViewRadius[i]) && isOnScreen == false)
            {           
                //locked = false;
                target = null;
                crosshair.SetActive(false);
                enemiesOnScreen.Remove(targetsInViewRadius[i]);
             
            }
            //GameObject target = targetsInViewRadius[i].gameObject;

            //visibleTargets.Add(target);
        }

     
        //button A
        if (Input.GetButtonDown("Fire3") && !locked && enemiesOnScreen.Count > 0)
        {
            index = 0;
            if (enemiesOnScreen[0] != null)
            {
                enemyName = enemiesOnScreen[0].name;
                locked = true;
                crosshair.SetActive(true);
                Debug.Log("TARGET LOCKED");
            }
            
        }
        else if (Input.GetButtonDown("Fire3") && locked)
        {
            locked = false;
            enemyName = string.Empty;
            target = null;
            crosshair.SetActive(false);
            Debug.Log("TARGET UNLOCKED");
        }
        //Button B
        if (Input.GetButtonDown("Fire1"))
        {
            if (enemiesOnScreen.Count > 0)
            {
                if (index <= enemiesOnScreen.Count)
                {
                    if (index == enemiesOnScreen.Count)
                    {
                        index = 0;
                    }
                    else
                    {
                        enemyName = enemiesOnScreen[++index].name;
                        target = null;
                        Debug.Log("index" + index);
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
                    if(target == null)
                    {
                        target = visibleTargets.Find(item => item.name == enemyName).transform;
                        go = visibleTargets.Find(item => item.name == enemyName).gameObject;
                    }
                                  
                    Vector3 enemyPos = Camera.main.WorldToScreenPoint(target.transform.position);
                    //enemyPos.x *= mCanvas.rect.width / (float)Camera.main.pixelWidth;
                    //enemyPos.y *= mCanvas.rect.height / (float)Camera.main.pixelHeight;

                    bool isOnScreenFinalCheck = (enemyPos.z >= 0 &&
                    enemyPos.x >= 0 && enemyPos.x <= Screen.width &&
                    enemyPos.y >= 0 && enemyPos.y <= Screen.height) ? true : false;
      
                    if (isOnScreenFinalCheck)
                    {
                       // Debug.Log("target: " + target.transform.position);
                        //target = visibleTargets.Find(item => item.name == enemyName).transform;
                        // target = enemiesOnScreen[index].transform;
                        crosshair.transform.position = Vector3.Slerp(crosshair.transform.position, new Vector3(enemyPos.x, enemyPos.y, 0)/*Camera.main.WorldToScreenPoint(target.position)*/, Time.deltaTime * NetworkClient.SERVER_UPDATE_TIME);
                       // Debug.Log("target: " + target.transform.position);
                        //crosshair.transform.position = Camera.main.WorldToScreenPoint(target.position);
                        crosshair.SetActive(true);
                        //Debug.Log("target: " + target.transform.position);
                    }
                    if(go.activeSelf == false)
                    {
                        //Debug.Log("TURNING OFF: " );
                        locked = false;
                        enemyName = string.Empty;
                        target = null;
                        crosshair.SetActive(false);
                    enemiesOnScreen.Clear();
                        //Debug.Log("target: " + target.transform.position);
                    }
                //}
                //catch
                //{
                    //Debug.Log("CATCH");
                    //int removeIndex = visibleTargets.FindIndex(item => item.name == name);
                    //visibleTargets.RemoveAt(removeIndex);
                    //locked = false;
                    //Debug.Log("UNLOCKED");
                    //name = string.Empty;
                    //target = null;
                    //crosshair.SetActive(false);
                //    target = visibleTargets.Find(item => item.name == enemiesOnScreen[10].name).transform; 
                //    Vector3 enemyPos = Camera.main.WorldToScreenPoint(target.transform.position);
                //    bool isOnScreenFinalCheck = (enemyPos.z >= 0 &&
                //enemyPos.x >= 0 && enemyPos.x <= Screen.width &&
                //enemyPos.y >= 0 && enemyPos.y <= Screen.height) ? true : false;

                //    if (isOnScreenFinalCheck)
                //    {
                //        target = enemiesOnScreen.Find(item => item.name == name).transform;
                //        // target = enemiesOnScreen[index].transform;
                //        crosshair.transform.position = Vector3.Lerp(crosshair.transform.position, Camera.main.WorldToScreenPoint(target.position), 10f * Time.deltaTime);
                //        //crosshair.transform.position = Camera.main.WorldToScreenPoint(target.position);
                //        crosshair.SetActive(true);
                //    }
                //}


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
