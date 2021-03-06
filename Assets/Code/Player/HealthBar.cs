using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    //public Text healthText;
    public Image healthBar;
    public float health = 100;
    public float maxHealth = 100;
    public float lerpspeed = .75f ;
    public Gradient gradient;
    private float previousHealth;

    public void Start()
    {
        health = maxHealth;
        previousHealth = maxHealth;
    }

    public void Update()
    {
        //healthText.text = "health: " + health + "%";
        
        healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, (health/maxHealth), lerpspeed * Time.deltaTime * NetworkClient.SERVER_UPDATE_TIME);
        
        Color healthColor = Color.Lerp(gradient.Evaluate((health / maxHealth)), gradient.Evaluate((previousHealth / maxHealth)), lerpspeed * Time.deltaTime * NetworkClient.SERVER_UPDATE_TIME);
        //Color healthColor = Color.Lerp(Color.red, Color.green, (health / maxHealth));
        //Color healthColor = gradient.Evaluate((health / maxHealth));
        healthBar.color = healthColor;
        previousHealth = health;
    }

    void healthBarFiller()
    {
        healthBar.fillAmount = health / maxHealth;
    }

    public void SetHealth(float _health)
    {
        health  = _health;
        

    }

    public void SetMaxHealth(int health)
    {
        //slider.value = health;
        //slider.maxValue = health;
    }
}
