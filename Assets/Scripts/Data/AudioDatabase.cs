using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioDatabase", menuName = "Game Data/Audio Database")]
public class AudioDatabase : ScriptableObject
{
    public List<AudioData> audioList;

    public AudioClip GetClip(AudioID id)
    {
        foreach (AudioData audio in audioList)
        {
            if (audio.audioID == id)
                return audio.clip;
        }

        return null;
    }

    public AudioCategory GetCategory(AudioID id)
{
    foreach (AudioData audio in audioList)
    {
        if (audio.audioID == id)
            return audio.category;
    }

    return AudioCategory.SFX;
}
}

[System.Serializable]
public class AudioData
{
    public AudioID audioID;

    public AudioCategory category;

    public AudioClip clip;
}

