using System;
using System.IO;
//using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OBJ_Save : MonoBehaviour
{
    private string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/DataFolder/OBJ_File";
    [SerializeField] private TMP_Text notifyText;
    [SerializeField] private TMP_InputField fileNameInput;

    public void SaveButtonClick()
    {
        if (fileNameInput.text != "")
        {
            IsExistFolder(path);
            string fileName = fileNameInput.text;
            string filePath = path + "/" + fileName;

            if (File.Exists(filePath))
                notifyText.text = "A file with the same name already exists";
            else
            {
                Transform selectedOBJ = GetComponent<OBJReadAndLoad>().prefabTransform;
                if (selectedOBJ != null)
                {
                    notifyText.text = "Saving...";
                    ConvertImageData(selectedOBJ);
                    OBJ_DataCustomParsing objData = new OBJ_DataCustomParsing(selectedOBJ);
                    string jsonData = JsonUtility.ToJson(objData);
                    SaveFile(jsonData, fileNameInput.text);
                }
            }
        }
        else
            notifyText.text = "Please Enter file name!!";
    }
    private void IsExistFolder(string folderPath)//폴더없으면 생성
    {
        if (!Directory.Exists(folderPath)) // 폴더가 존재하지 않으면
        {
            Directory.CreateDirectory(folderPath); // 폴더 생성
            Debug.Log("폴더 생성 완료");
        }
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
    private void SaveFile(string jsonData, string fileName)
    {

        using (FileStream fs = new FileStream(path + "/" + fileName, FileMode.Create, FileAccess.Write))
        {
            //파일로 저장할 수 있게 바이트화
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(jsonData);

            //bytes의 내용물을 0 ~ max 길이까지 fs에 복사
            fs.Write(bytes, 0, bytes.Length);
        }
    }
}
