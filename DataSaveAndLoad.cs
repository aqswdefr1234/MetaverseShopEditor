using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

public class DataSaveAndLoad : MonoBehaviour
{
    private string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/DataFolder";//using System;
    private string jsonData = "";
    [SerializeField] private TMP_InputField fileNameInput;
    [SerializeField] private TMP_Text notifyText;
    [SerializeField] private Transform foundationParent;
    public string frontImagePath;//세팅패널에서 선택한 이미지의 경로
    //public string backImagePath;
    public Texture2D frontImage;//세팅캔버스에서 선택한 이미지가 들어감
    public Texture2D backImage;
    public string frontImageData = "testfornt";//폴더 뷰에서 선택한 이미지 데이터를 가져온다.
    public string backImageData = "testback";

    public void SaveMeshData(Mesh _meshFront, Mesh _meshBack)
    {
        //SavedData sd = new SavedData(_meshFront.vertices, _meshFront.uv, _meshFront.normals, _meshFront.triangles, _meshBack.vertices, _meshBack.uv, _meshBack.normals, _meshBack.triangles, frontImageData, backImageData);
        //jsonData = JsonUtility.ToJson(sd);//앞 뒤 두개 존재
    }
    public void LocalSaveData()//저장버튼 누르면 발생
    {
        if (fileNameInput.text != "")
        {
            IsExistFolder(path);
            string iniFileName = fileNameInput.text;
            string filePath = path + "/" + iniFileName;

            if (File.Exists(filePath + ".txt"))
                notifyText.text = "A file with the same name already exists";
            else
            {
                notifyText.text = "Save!";
                Debug.Log(jsonData.Length);
                SaveFile(jsonData, iniFileName);
            }
        }
        else
            notifyText.text = "Please Enter file name!!";
    }
    private void ConvertImageData(Transform selectedOBJ)//버튼 클릭시 이미지 정보를 string으로 변환한다. 마지막 저장할 때 해야 불필요한 계산이 없어진다.
    {

        if (selectedOBJ != null)
        {
            foreach (Transform child in selectedOBJ)
            {
                string texturePath = child.GetComponent<ChildTextureString>().childTexturePath;
                if (texturePath != "")
                {
                    byte[] imageData = File.ReadAllBytes(texturePath);//Texture2D.EncodeToPNG를 사용하면 텍스쳐가 압축되어진다.
                    child.GetComponent<ChildTextureString>().childTextureData = Convert.ToBase64String(imageData);
                }
                else
                    child.GetComponent<ChildTextureString>().childTextureData = "null";
            }
        }
    }
    void SaveFile(string jsonData,string fileName)
    {
        
        using (FileStream fs = new FileStream(path + "/" + fileName + ".txt", FileMode.Create, FileAccess.Write))
        {
            //파일로 저장할 수 있게 바이트화
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(jsonData);

            //bytes의 내용물을 0 ~ max 길이까지 fs에 복사
            fs.Write(bytes, 0, bytes.Length);
        }
    }
    void IsExistFolder(string folderPath)//폴더없으면 생성
    {
        if (!Directory.Exists(folderPath)) // 폴더가 존재하지 않으면
        {
            Directory.CreateDirectory(folderPath); // 폴더 생성
            Debug.Log("폴더 생성 완료");
        }
    }
}
