using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "ServerObjects", menuName = "ScriptableObjects/ServerObjects", order = 3)]
public class ServerObjects : ScriptableObject
{
    public List<ServerObjectData> Objects;

    public ServerObjectData GetObjectByName(string Name)
    {
        return Objects.SingleOrDefault(x => x.Name == Name);
    }

}

[Serializable]
public class ServerObjectData
{
    public string Name = "New Object";
    public GameObject Prefab;
    public int Count = 100;
}
