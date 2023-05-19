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
    private void IsExistFolder(string folderPath)//���������� ����
    {
        if (!Directory.Exists(folderPath)) // ������ �������� ������
        {
            Directory.CreateDirectory(folderPath); // ���� ����
            Debug.Log("���� ���� �Ϸ�");
        }
    }
    private void ConvertImageData(Transform selectedOBJ)//��ư Ŭ���� �̹��� ������ string���� ��ȯ�Ѵ�. ������ ������ �� �ؾ� ���ʿ��� ����� ��������.
    {
        
        if (selectedOBJ != null)
        {
            foreach (Transform child in selectedOBJ)
            {
                string texturePath = child.GetComponent<ChildTextureString>().childTexturePath;
                if (texturePath != "")
                {
                    byte[] imageData = File.ReadAllBytes(texturePath);//Texture2D.EncodeToPNG�� ����ϸ� �ؽ��İ� ����Ǿ�����.
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
            //���Ϸ� ������ �� �ְ� ����Ʈȭ
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(jsonData);

            //bytes�� ���빰�� 0 ~ max ���̱��� fs�� ����
            fs.Write(bytes, 0, bytes.Length);
        }
    }
}
