using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpawnEffect : MonoBehaviour {

    public float spawnEffectTime = 2;
    public float pause = 1;
    public AnimationCurve fadeIn;
    MeshRenderer mesh;
    public ParticleSystem ps;
    float timer = 0;
    Renderer _renderer;
    public Material[] PlayerShipMaterials;
    public Material respawnMaterial;
    int shaderProperty;
    Material[] mats;
    MeshRenderer[] children;
    Renderer[] renderer1;
    AudioManager audioSource;
    bool firstTime = true;
    void Start ()
    {
        audioSource = FindObjectOfType<AudioManager>();
        shaderProperty = Shader.PropertyToID("_cutoff");
        children = GetComponentsInChildren<MeshRenderer>();
        foreach(MeshRenderer child in children.Skip(1))
        {
            child.material = respawnMaterial;
        }
   
        renderer1 = GetComponentsInChildren<Renderer>();

        ps = ps.GetComponent<ParticleSystem>();

        //var main = ps.main;
        //main.duration = spawnEffectTime;
        //ps.transform.position = this.transform.position;
        ps.Play();
        //audioSource.PlaySFX2("Alien Ship Hum Loop 1", .7f);
    }

    void OnDisable()
    {
        audioSource.StopSFX2("Alien Ship Hum Loop 1");
        //audioSource.StopCoroutine("Alien Ship Hum Loop 1");
        foreach (MeshRenderer child in children.Skip(1))
        {
            child.material = respawnMaterial;
        }
        firstTime = true;
        timer = 0;
        ps.Stop();
        
    }

    void OnEnable()
    {
        audioSource = FindObjectOfType<AudioManager>();
        shaderProperty = Shader.PropertyToID("_cutoff");
        children = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer child in children.Skip(1))
        {
            child.material = respawnMaterial;
        }
   
        renderer1 = GetComponentsInChildren<Renderer>();
        ps = ps.GetComponent <ParticleSystem>();

        var main = ps.main;
        main.duration = spawnEffectTime;
        ps.Play();
        //audioSource.PlaySFX2("Alien Ship Hum Loop 1", .7f);
        //firstTime = false;
    }

    void Update ()
    {
        //if (timer < spawnEffectTime + pause)
        //{
        //    timer += Time.deltaTime;
        //}
        //else
        //{
        //    ps.Play();
        //}

        foreach (Renderer rend in renderer1.Skip(1))
        {
            rend.material.SetFloat(shaderProperty, fadeIn.Evaluate(Mathf.InverseLerp(0, spawnEffectTime, timer)));
            var lerp = Mathf.PingPong(Time.time, 2) / 2;
            rend.material.SetFloat("_Blend", lerp);
        }

        if (ps.isPlaying == false && firstTime == true)
        {

            firstTime = false;
            ps.Stop();
            audioSource.PlaySFX2("Alien Ship Hum Loop 1", .7f);
            var i = 0;
            foreach (MeshRenderer child in children.Skip(1))
            {
                
                child.material = PlayerShipMaterials[i];
                i++;
            }
            
        }
    }
}
