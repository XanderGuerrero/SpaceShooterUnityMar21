using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamagePopUp : MonoBehaviour
{
    private TextMeshProUGUI textMesh;
    float moveYspeed = 10f;
    float disappearTimer;
    Color textColor;
    float disapearSpeed = 5f;
    Vector3 worldPosition;
    public GameObject popup;

    private void Awake()
    {
        textMesh = transform.GetComponent<TextMeshProUGUI>();
    }

    public void SetUpPopUp(float damageAmount, Vector3 _worldPosition)
    {
        worldPosition = _worldPosition;
        //Debug.Log("HEY MAN!!!! DAMAMGE POP UP");
        textMesh.SetText(damageAmount.ToString());
        textColor = textMesh.color;

        //start disappearing after this much time
        disappearTimer = .3f;
    }

    public void Update()
    {
       
        Vector3 enemyPos = Camera.main.WorldToScreenPoint(worldPosition);
       
        popup.transform.position = new Vector3(enemyPos.x, enemyPos.y, 0f);
        //popup.transform.rotation = Quaternion.identity;
        //popup.transform.LookAt(Camera.main.transform, Camera.main.transform.up);

        popup.transform.position += new Vector3(moveYspeed, moveYspeed) * Time.deltaTime; ;
        popup.transform.localScale += new Vector3(moveYspeed, moveYspeed) * Time.deltaTime;


        disappearTimer -= Time.deltaTime;
        if (disappearTimer < 0)
        {
            //start disappearing
            textColor.a -= disapearSpeed * Time.deltaTime;
            textMesh.color = textColor;
            if (textColor.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
