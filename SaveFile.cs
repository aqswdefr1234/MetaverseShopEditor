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
                sd.obj_Name[0] = fileNameInput.text;//obj_Name[0]���� foundation �ֻ��� ��ü�� �̸��� ����. ���ϸ�� ��ġ��Ų��.
                string jsonData = JsonUtility.ToJson(sd);
                SaveJsonFile(jsonData, fileNameInput.text);
                notifyText.text = "Save Success!";
            }
        }
        else
            notifyText.text = "Please Enter file name!!";

        
    }
    private void IsExistFolder(string folderPath)//���������� ����
    {
        if (!Directory.Exists(folderPath)) // ������ �������� ������
        {
            Directory.CreateDirectory(folderPath); // ���� ����
            Debug.Log("���� ���� �Ϸ�");
        }
    }
    private void SaveJsonFile(string jsonData, string fileName)
    {

        using (FileStream fs = new FileStream(path + "/" + fileName, FileMode.Create, FileAccess.Write))
        {
            //���Ϸ� ������ �� �ְ� ����Ʈȭ
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(jsonData);

            //bytes�� ���빰�� 0 ~ max ���̱��� fs�� ����
            fs.Write(bytes, 0, bytes.Length);
        }
    }
}
