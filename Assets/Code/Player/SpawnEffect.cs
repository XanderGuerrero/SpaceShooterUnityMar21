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

    void Start ()
    {
        shaderProperty = Shader.PropertyToID("_cutoff");
        children = GetComponentsInChildren<MeshRenderer>();
        foreach(MeshRenderer child in children.Skip(1))
        {
            child.material = respawnMaterial;
        }
   
        renderer1 = GetComponentsInChildren<Renderer>();

        ps = ps.GetComponent<ParticleSystem>();

        var main = ps.main;
        main.duration = spawnEffectTime;
        //ps.transform.position = this.transform.position;
        ps.Play();
    }

    void OnDisable()
    {
        foreach (MeshRenderer child in children.Skip(1))
        {
            child.material = respawnMaterial;
        }
     
        timer = 0;
        ps.Stop();
    }

    void OnEnable()
    {
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
    }

    void Update ()
    {
        if (timer < spawnEffectTime + pause)
        {
            timer += Time.deltaTime;
        }
        else
        {
            ps.Play();
        }

        foreach (Renderer rend in renderer1.Skip(1))
        {
            rend.material.SetFloat(shaderProperty, fadeIn.Evaluate(Mathf.InverseLerp(0, spawnEffectTime, timer)));
            var lerp = Mathf.PingPong(Time.time, 2) / 2;
            rend.material.SetFloat("_Blend", lerp);
        }

        if (ps.isPlaying == false)
        {
            ps.Stop();
            var i = 0;
            foreach (MeshRenderer child in children.Skip(1))
            {
                
                child.material = PlayerShipMaterials[i];
                i++;
            }
        }
    }
}
