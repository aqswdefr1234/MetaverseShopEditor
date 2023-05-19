using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class SaveCurrentSceneData : MonoBehaviour
{
    private Transform objectSpace;

    public void SaveLoadedData()
    {
        objectSpace = BaseScene_OverallManager.objCreationSpace;
        string filePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/DataFolder";
        TransformData tfd;
        List<string> myName = new List<string>();
        List<Vector3> myPosition = new List<Vector3>();
        List<Vector3> myRotation = new List<Vector3>();
        List<Vector3> myScale = new List<Vector3>();
        List<string> myProductExplanation = new List<string>();
        List<string> path = new List<string>();

        ObjectSceneData[] osd = objectSpace.GetComponentsInChildren<ObjectSceneData>();
        string selectedRoom = GetComponent<RoomLoad>().roomName;

        foreach (ObjectSceneData d in osd)
        {
            myName.Add(d.myName);
            myPosition.Add(d.myPosition);
            myRotation.Add(d.myRotation);
            myScale.Add(d.myScale);
            myProductExplanation.Add(d.myProductExplanation);
            path.Add(d.path);
        }
        if(osd.Length == 0)
            tfd = new TransformData(myName, myPosition, myRotation, myScale, myProductExplanation, path, selectedRoom);
        else
            tfd = new TransformData(myName, myPosition, myRotation, myScale, myProductExplanation, path, selectedRoom);
        string jsonData = JsonUtility.ToJson(tfd);
        File.WriteAllText(filePath + "/" + "SceneData", jsonData);
    }
}
