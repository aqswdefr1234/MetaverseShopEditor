using UnityEngine;

public class Room1LightMap : MonoBehaviour
{
    public int lightMapCount = 1;
    // ����Ʈ���� ������ ������Ʈ�� Mesh Renderer ������Ʈ
    public MeshRenderer body_meshRenderer;
    // ������ ����Ʈ�� �ؽ���
    public Texture2D body_LightmapTexture;
    //dir
    public Texture2D body_LightmapDir;
    // ����Ʈ���� ũ�� �� ��ġ ������ �����ϴ� �迭
    public Vector4 body_tilingAndOffset;

    private LightmapData[] newLightmaps;
    void Start()//������Ʈ�� static�����־����
    {
        newLightmaps = new LightmapData[lightMapCount];
        LightMapInsert(lightMapCount - 1, body_meshRenderer, body_LightmapTexture, body_LightmapDir, body_tilingAndOffset);
        LightmapSettings.lightmaps = newLightmaps;
    }
    void LightMapInsert(int lightMapIndex, MeshRenderer meshRenderer, Texture2D lightmapTexture, Texture2D lightmapDir, Vector4 tilingAndOffset)
    {
        newLightmaps[lightMapIndex] = new LightmapData();
        newLightmaps[lightMapIndex].lightmapColor = lightmapTexture;
        newLightmaps[lightMapIndex].lightmapDir = lightmapDir;
        // ������Ʈ�� Lightmap Index ����
        meshRenderer.lightmapIndex = lightMapIndex;
        // ����Ʈ�� UVs ����
        meshRenderer.lightmapScaleOffset = tilingAndOffset;
        meshRenderer.realtimeLightmapIndex = 0;

        // ����Ʈ�� �ؽ��� ����
        //meshRenderer.material.SetTexture("_LightMap", lightmapTexture);
    }
}
