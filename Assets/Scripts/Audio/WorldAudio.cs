using UnityEngine;

public class WorldAudio : MonoBehaviour
{
    public AudioID bgm;

    void Start()
    {
        AudioManager.Instance.Play(bgm);
    }
}