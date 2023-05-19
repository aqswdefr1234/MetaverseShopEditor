using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TransformData
{
    public string myRoomName = "";
    public List<string> myName = new List<string>();
    public List<Vector3> myPosition = new List<Vector3>();
    public List<Vector3> myRotation = new List<Vector3>();
    public List<Vector3> myScale = new List<Vector3>();
    public List<string> myProductExplanation = new List<string>();
    public List<string> path = new List<string>();

    public TransformData(List<string> name, List<Vector3> position, List<Vector3> rotation, List<Vector3> scale, List<string> productExplanation, List<string> filePath, string roomName)
    {
        myName.AddRange(name);
        myPosition.AddRange(position);
        myRotation.AddRange(rotation);
        myScale.AddRange(scale);
        myProductExplanation.AddRange(productExplanation);
        path.AddRange(filePath);
        myRoomName = roomName;
    }
}
