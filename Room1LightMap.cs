using UnityEngine;

public class Room1LightMap : MonoBehaviour
{
    public int lightMapCount = 1;
    // 라이트맵을 적용할 오브젝트의 Mesh Renderer 컴포넌트
    public MeshRenderer body_meshRenderer;
    // 적용할 라이트맵 텍스쳐
    public Texture2D body_LightmapTexture;
    //dir
    public Texture2D body_LightmapDir;
    // 라이트맵의 크기 및 위치 정보를 저장하는 배열
    public Vector4 body_tilingAndOffset;

    private LightmapData[] newLightmaps;
    void Start()//오브젝트의 static꺼져있어야함
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
        // 오브젝트의 Lightmap Index 설정
        meshRenderer.lightmapIndex = lightMapIndex;
        // 라이트맵 UVs 설정
        meshRenderer.lightmapScaleOffset = tilingAndOffset;
        meshRenderer.realtimeLightmapIndex = 0;

        // 라이트맵 텍스쳐 적용
        //meshRenderer.material.SetTexture("_LightMap", lightmapTexture);
    }
}
