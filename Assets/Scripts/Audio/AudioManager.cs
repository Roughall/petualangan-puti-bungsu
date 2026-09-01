using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Database")]
    public AudioDatabase database;

    [Header("Audio Mixer")]
    public AudioMixer masterMixer;

    [Header("Mixer Groups")]
    public AudioMixerGroup bgmGroup;
    public AudioMixerGroup sfxGroup;
    public AudioMixerGroup uiGroup;
    public AudioMixerGroup ambientGroup;

    private AudioSource bgmSource;
    private AudioSource sfxSource;
    private AudioSource uiSource;
    private AudioSource ambientSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            CreateAudioSources();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void CreateAudioSources()
    {
        bgmSource = CreateSource("BGM", bgmGroup, true);

        sfxSource = CreateSource("SFX", sfxGroup, false);

        uiSource = CreateSource("UI", uiGroup, false);

        ambientSource = CreateSource("Ambient", ambientGroup, true);
    }

    AudioSource CreateSource(string name, AudioMixerGroup group, bool loop)
    {
        GameObject obj = new GameObject(name);

        obj.transform.SetParent(transform);

        AudioSource source = obj.AddComponent<AudioSource>();

        source.outputAudioMixerGroup = group;

        source.loop = loop;

        source.playOnAwake = false;

        return source;
    }

    //--------------------------------------------------------

    

    //--------------------------------------------------------

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    //--------------------------------------------------------

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null)
            return;

        sfxSource.PlayOneShot(clip);
    }

    //--------------------------------------------------------

    public void PlayUI(AudioClip clip)
    {
        if (clip == null)
            return;

        uiSource.PlayOneShot(clip);
    }

    //--------------------------------------------------------

    public void PlayAmbient(AudioClip clip)
    {
        if (clip == null)
            return;

        ambientSource.clip = clip;

        ambientSource.Play();
    }

    public void PlayWorld(WorldData world)
{
    if(world == null)
        return;

    Play(world.bgm);
}

    //--------------------------------------------------------

    public void StopAmbient()
    {
        ambientSource.Stop();
    }

    //--------------------------------------------------------

    public void SetMasterVolume(float value)
    {
        masterMixer.SetFloat("MasterVolume", value);
    }

    public void SetBGMVolume(float value)
    {
        masterMixer.SetFloat("BGMVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        masterMixer.SetFloat("SFXVolume", value);
    }

    public void SetUIVolume(float value)
    {
        masterMixer.SetFloat("UIVolume", value);
    }

    public void Play(AudioID id)
{
    AudioClip clip = database.GetClip(id);

    if (clip == null)
    {
        Debug.LogWarning("Audio tidak ditemukan : " + id);
        return;
    }

    AudioCategory category = database.GetCategory(id);

    switch (category)
    {
        case AudioCategory.Music:

            bgmSource.clip = clip;
            bgmSource.loop = true;
            bgmSource.Play();

            break;

        case AudioCategory.SFX:

            sfxSource.PlayOneShot(clip);

            break;

        case AudioCategory.UI:

            uiSource.PlayOneShot(clip);

            break;

        case AudioCategory.Ambient:

            ambientSource.clip = clip;
            ambientSource.loop = true;
            ambientSource.Play();

            break;
    }
}

public void Stop(AudioCategory category)
{
    switch(category)
    {
        case AudioCategory.Music:

            bgmSource.Stop();

            break;

        case AudioCategory.Ambient:

            ambientSource.Stop();

            break;
    }
}
}