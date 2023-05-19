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
    public string frontImagePath;//�����гο��� ������ �̹����� ���
    //public string backImagePath;
    public Texture2D frontImage;//����ĵ�������� ������ �̹����� ��
    public Texture2D backImage;
    public string frontImageData = "testfornt";//���� �信�� ������ �̹��� �����͸� �����´�.
    public string backImageData = "testback";

    public void SaveMeshData(Mesh _meshFront, Mesh _meshBack)
    {
        //SavedData sd = new SavedData(_meshFront.vertices, _meshFront.uv, _meshFront.normals, _meshFront.triangles, _meshBack.vertices, _meshBack.uv, _meshBack.normals, _meshBack.triangles, frontImageData, backImageData);
        //jsonData = JsonUtility.ToJson(sd);//�� �� �ΰ� ����
    }
    public void LocalSaveData()//�����ư ������ �߻�
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
    void SaveFile(string jsonData,string fileName)
    {
        
        using (FileStream fs = new FileStream(path + "/" + fileName + ".txt", FileMode.Create, FileAccess.Write))
        {
            //���Ϸ� ������ �� �ְ� ����Ʈȭ
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(jsonData);

            //bytes�� ���빰�� 0 ~ max ���̱��� fs�� ����
            fs.Write(bytes, 0, bytes.Length);
        }
    }
    void IsExistFolder(string folderPath)//���������� ����
    {
        if (!Directory.Exists(folderPath)) // ������ �������� ������
        {
            Directory.CreateDirectory(folderPath); // ���� ����
            Debug.Log("���� ���� �Ϸ�");
        }
    }
}
