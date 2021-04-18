#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class LevelDataEditor : ScriptableWizard
{
    public GameObject levelDataParent;

    [MenuItem("Tools/Level Data")]
    public static void CreateWizard()
    {
        DisplayWizard<LevelDataEditor>("Create level data for the server", "Create");
    }

    public void OnWizardUpdate()
    {
        isValid = levelDataParent != null;
        //if(levelDataParent == null)
        //{
        //    isValid = false;
        //}
    }
    public void OnWizardCreate()
    {
        List<LevelDataHelper> children = levelDataParent.GetComponentsInChildren<LevelDataHelper>().ToList();

        LevelData levelData = new LevelData();
        levelData.teamOneSpawn = new List<LevelDataElement>();
        levelData.TeamTwoSpawn = new List<LevelDataElement>();
        levelData.ItemSpawn = new List<LevelDataElement>();
        levelData.FreeForAllSpawn = new List<LevelDataElement>();


        children.ForEach(child  =>
        {
            var data = LevelDataElement.Default;
            data.radius = child.Radius;
            data.position.x = child.transform.position.x;
            data.position.y = child.transform.position.y;
            data.position.z = child.transform.position.z;


            switch (child.Type)
            {
                case LevelDataHelperType.Team_One_Spawn:
                    levelData.teamOneSpawn.Add(data);
                    break;
                case LevelDataHelperType.Team_Two_Spawn:
                    levelData.TeamTwoSpawn.Add(data);
                    break;
                case LevelDataHelperType.Item_Spawn:
                    levelData.ItemSpawn.Add(data);
                    break;
                case LevelDataHelperType.Free_For_All_Spawn:
                    levelData.FreeForAllSpawn.Add(data);
                    break;
            }
        });

        string exportData = JsonUtility.ToJson(levelData);
        Debug.Log(exportData);
    }
}
#endif