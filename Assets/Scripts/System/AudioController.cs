using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioController : MonoBehaviour
{
    [System.Serializable]
    public struct AudioData
    {
        public AudioClip clip;
        public string name;

        [Range(0.1f, 1f)]
        public float volume;
    }

    public AudioMixer audioMixer;
    [SerializeField] AudioClip _menuMusic;
    [SerializeField] AudioClip _gameMusic;

    public AudioData[] sounds;
    public AudioSource soundsAudioSource;

    [SerializeField]
    private AudioSource musicAudioSource;

    public static AudioController Instance { get; private set; }


    [SerializeField]
    private float generalVolume;

    private float musicVolume;
    private float sfxVolume;


    [Header("Music")]
    //Fade time.
    public float fadeTime = 2f;

    //Pitch change time.
    public float pitchTime = 1f;

    //Min pitch value to change the sound when displaying the end-game menu.
    public float pitchSlow = .6f;

    private Coroutine fadeCorutine;
    private Coroutine pitchCorutine;

    void Awake()
    {
        if (Instance != this && Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        ApplyAllVolumes(); // ✅ Nuevo método para aplicar todos los volúmenes
        generalVolume = PlayerPrefs.GetFloat("GeneralMusic", 1f);
        musicVolume = PlayerPrefs.GetFloat("Music", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFX", 1f);

        SetMusicForScene(SceneManager.GetActiveScene().name);
        if (!musicAudioSource.isPlaying)
            musicAudioSource.Play();
    }

    private void ApplyAllVolumes()
    {
        // Aplicar volúmenes según tu enfoque elegido
        SetMixerVolume("GeneralMusic", generalVolume);
        SetMixerVolume("Music", musicVolume);
        SetMixerVolume("SFX", sfxVolume);
    }

    private void SetMixerVolume(string parameterName, float linearVolume)
    {
        if (audioMixer != null)
        {
            // Conversión: 0-1 lineal a -80dB a 0dB
            float dB = linearVolume > 0.0001f ? 20f * Mathf.Log10(linearVolume) : -80f;
            audioMixer.SetFloat(parameterName, dB);
        }
    }

    public void SetMusicForScene(string sceneName)
    {
        if (musicAudioSource == null) Debug.LogError("musicAudioSource no asignado en AudioController");
        if (_menuMusic == null || _gameMusic == null) Debug.LogWarning("Clips de música no asignados");
        if (sceneName == "MainMenu" || sceneName == "Achievements")
        {
            if (musicAudioSource.clip == _menuMusic) return;
            if (fadeCorutine != null) StopCoroutine(fadeCorutine);
            fadeCorutine = StartCoroutine(FadeAndChangeClip(_menuMusic));
        }
        else if (sceneName == "Game")
        {
            if (musicAudioSource.clip == _gameMusic) return;
            if (fadeCorutine != null) StopCoroutine(fadeCorutine);
            fadeCorutine = StartCoroutine(FadeAndChangeClip(_gameMusic));
        }

        if (!musicAudioSource.isPlaying)
        {
            musicAudioSource.Play();
        }
    }

    public void SetGeneralVolume(float _volume)
    {
        generalVolume = _volume;
        PlayerPrefs.SetFloat("GeneralMusic", _volume);
        ApplyAllVolumes();
        SetMixerVolume("GeneralMusic", _volume);
        PlayerPrefs.Save();
    }

    public void SetMusicVolume(float _volume)
    {
        musicVolume = _volume;
        PlayerPrefs.SetFloat("Music", _volume);
        SetMixerVolume("Music", _volume);
        PlayerPrefs.Save();
    }


    public void SetEffectVolume(float _volume)
    {
        sfxVolume = _volume;
        PlayerPrefs.SetFloat("SFX", _volume);
        SetMixerVolume("SFX", _volume);
        PlayerPrefs.Save();
    }

    private IEnumerator FadeAndChangeClip(AudioClip clip)
    {
        //We will use the counter with half the time, since we have to fade in/out.
        float time = fadeTime / 2f;
        float counter = time;
        while (counter > 0)
        {
            musicAudioSource.volume = counter / time;
            counter -= Time.deltaTime;
            yield return null;
        }
        //Music clip change.
        musicAudioSource.clip = clip;
        //We start playback as it stops with the clip change
        musicAudioSource.Play();
        counter = time;
        //We turn up the volume.
        while (counter > 0)
        {
            musicAudioSource.volume = 1 - (counter / time);
            counter -= Time.deltaTime;
            yield return null;
        }
        musicAudioSource.volume = 1f;
    }

    [ContextMenu("Slow")]
    public void PitchSlow()
    {
        if (pitchCorutine != null)
        {
            StopCoroutine(pitchCorutine);
        }
        pitchCorutine = StartCoroutine(SlowMode(true));
    }

    [ContextMenu("Regular")]
    public void PitchRegular()
    {
        if (pitchCorutine != null)
        {
            StopCoroutine(pitchCorutine);
        }
        pitchCorutine = StartCoroutine(SlowMode(false));
    }

    /// <summary>
    /// To reduce the playback pitch.
    /// </summary>
    /// <param name="isSlow"></param>
    /// <returns></returns>
    private IEnumerator SlowMode(bool isSlow)
    {
        float target = isSlow ? pitchSlow : 1;
        float initial = musicAudioSource.pitch;
        float count = 0f;
        while (count < pitchTime)
        {
            musicAudioSource.pitch = Mathf.Lerp(initial, target, count / pitchTime);
            count += Time.deltaTime;
            yield return null;
        }
        musicAudioSource.pitch = target;
    }

    public float GetMusicVolume()
    {
        return musicVolume;
    }

    public float GetGeneralVolume()
    {
        return generalVolume;
    }

    public float GetEffectVolume()
    {
        return sfxVolume;
    }
}
