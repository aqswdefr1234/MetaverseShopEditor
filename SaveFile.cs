using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SaveFile : MonoBehaviour
{
    [SerializeField] private TMP_Text notifyText;
    [SerializeField] private TMP_InputField fileNameInput;
    [SerializeField] private Transform foundation;

    private string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/DataFolder/CreatedCloth";
    private SavedData sd;
    public void SaveButton()
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
                sd = new SavedData(foundation);
                sd.obj_Name[0] = fileNameInput.text;//obj_Name[0]에는 foundation 최상위 객체의 이름이 들어간다. 파일명과 일치시킨다.
                string jsonData = JsonUtility.ToJson(sd);
                SaveJsonFile(jsonData, fileNameInput.text);
                notifyText.text = "Save Success!";
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
    private void SaveJsonFile(string jsonData, string fileName)
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
