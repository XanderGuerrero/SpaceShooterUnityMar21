using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerScore : MonoBehaviour
{
    [SerializeField] private Transform PointsPopUp;
    //private TextMeshPro textMesh;
    private TMP_Text m_TextComponent;
    //float moveYspeed = .5f;
    //float disappearTimer;
    //Color textColor;
    //float disapearSpeed = 3f;

    private void Awake()
    {
        m_TextComponent = PointsPopUp.GetComponent<TMP_Text>();
    }

    public void SetPlayerScore(float damageAmount)
    {
        //Debug.Log("about to change the score");
        m_TextComponent.text = damageAmount.ToString();
        //textColor = m_TextComponent.color;
        //disappearTimer = 0.3f;
    }

    //public void Update()
    //{

    //    transform.position += new Vector3(0, moveYspeed) * Time.deltaTime * NetworkClient.SERVER_UPDATE_TIME;

    //    disappearTimer -= Time.deltaTime;
    //    if (disappearTimer < 0)
    //    {
    //        //start disappearing
    //        textColor.a -= disapearSpeed * Time.deltaTime;
    //        m_TextComponent.color = textColor;
    //        if (textColor.a < 0)
    //        {
    //            Destroy(gameObject);
    //        }
    //    }
    //}
}
