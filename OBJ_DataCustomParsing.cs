using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OBJ_DataCustomParsing
{
    public List<string> obj_Name = new List<string>();//인덱스 0번에는 파일 명이 들어가 있다.
    public List<Vector3> obj_Vertices = new List<Vector3>();
    public List<Vector2> obj_Uvs = new List<Vector2>();
    public List<int> obj_Polygon = new List<int>();
    public List<string> obj_Texture = new List<string>();//디코딩으로 문자열로 만들었기 때문에 string형
    public List<Color32> obj_color32 = new List<Color32>();//material의 컬러값
    public List<int> child_VerticesCount = new List<int>();
    public List<int> child_UVCount = new List<int>();
    public List<int> child_TrianglesCount = new List<int>();

    public OBJ_DataCustomParsing(Transform obj)
    {
        Mesh mesh = new Mesh();
        obj_Name.Add(obj.name);
        foreach (Transform child in obj)
        {
            obj_Name.Add(child.name);
            mesh = child.GetComponent<MeshFilter>().mesh;
            obj_Vertices.AddRange(mesh.vertices);
            obj_Uvs.AddRange(mesh.uv);
            obj_Polygon.AddRange(mesh.triangles);
            obj_Texture.Add(child.GetComponent<ChildTextureString>().childTextureData);
            obj_color32.Add(child.GetComponent<Renderer>().material.color);
            child_VerticesCount.Add(mesh.vertices.Length);
            child_UVCount.Add(mesh.uv.Length);
            child_TrianglesCount.Add(mesh.triangles.Length);
        }
        
    }
}
