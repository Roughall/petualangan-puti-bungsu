using UnityEngine;

[CreateAssetMenu(menuName = "Game/World Data", fileName = "WorldData")]
public class WorldData : ScriptableObject
{
    [Header("Identity")]
    public WorldID worldID;

    public string sceneName;

    [Header("Default Spawn")]
    public Vector2 defaultSpawn;

    [Header("Camera")]
    public Vector2 cameraMinBounds;
    public Vector2 cameraMaxBounds;

    [Header("Audio")]
    public AudioID bgm;

    [Header("World Info")]
    public string displayName;

    [Header("World Type")]
    public WorldType worldType;

    [Header("Lighting")]
    public Color ambientColor = Color.white;

    public Sprite previewImage;
}