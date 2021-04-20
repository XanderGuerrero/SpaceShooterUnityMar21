#if UNITY_EDITOR
//above means if not in the unity editor, strip this away when not in the editor
// do not build this code only works with unity
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class LevelDataHelper : MonoBehaviour
{
    [SerializeField]
    private float radius = 5;

    [SerializeField]
    private LevelDataHelperType type;

    public float Radius
    {
        get
        {
            return radius;
        }
    }

    public LevelDataHelperType Type
    {
        get
        {
            return type;
        }
    }

    public void OnDrawGizmos()
    {
        Handles.color = TypeToColour(this.Type);
        Handles.DrawWireDisc(center: transform.position, normal: Vector3.up, radius);
    }

    private Color TypeToColour(LevelDataHelperType type)
    {
        switch (type)
        {
            case LevelDataHelperType.Team_One_Spawn:
                return Color.blue;
            case LevelDataHelperType.Team_Two_Spawn:
                return Color.green;
            case LevelDataHelperType.Item_Spawn:
                return Color.yellow;
            case LevelDataHelperType.Free_For_All_Spawn:
                return Color.cyan;
        }
        return Color.black;
    }

   
}

public enum LevelDataHelperType
{
    Team_One_Spawn = 0,
    Team_Two_Spawn,
    Item_Spawn,
    Free_For_All_Spawn

}

[Serializable]
public class LevelData
{
    //data representation of anything that will be in a level 
    public List<LevelDataElement> teamOneSpawn;
    public List<LevelDataElement> teamTwoSpawn;
    public List<LevelDataElement> itemSpawn;
    public List<LevelDataElement> freeForAllSpawn;
}

[Serializable]
public class LevelDataElement
{
    //data representation of anything that will be in a level 
    public float radius;
    public Vector3 position;

    public static LevelDataElement Default
    {
        get
        {
            LevelDataElement e = new LevelDataElement();
            e.radius = 5.0f;
            e.position = new Vector3();
            e.position.x = 0;
            e.position.y= 0;
            e.position.z = 0;

            return e;
        }
        
    }
}

//[Serializable]
//public class Vect3
//{
//    //data representation of anything that will be in a level 
//    //
//    public float x;
//    public float y;
//    public float z;
//}
#endif