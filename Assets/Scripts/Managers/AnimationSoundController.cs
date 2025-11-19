using UnityEngine;
using UnityEngine.Audio;

public class AnimationSoundController : MonoBehaviour
{
    [Header("Audio Mixer Groups")]
    public AudioMixerGroup effectsMixerGroup; // Asignar el grupo de efectos del AudioMixer

    public AudioMixerGroup voiceMixerGroup;
    public AudioMixerGroup movementMixerGroup;

    [Header("Audio Sources")]
    public AudioSource movementAudioSource;

    public AudioSource attackAudioSource;
    public AudioSource specialAudioSource;
    public AudioSource voiceAudioSource;

    [Header("Footstep Sounds")]
    public AudioClip[] footstepSounds;

    [Range(0f, 1f)]
    public float footstepVolume = 0.5f;

    public float footstepPitchMin = 0.8f;
    public float footstepPitchMax = 1.2f;

    [Header("Attack Sounds")]
    public AudioClip basicAttackSound;

    [Range(0f, 1f)]
    public float attackVolume = 0.7f;

    [Header("Voice Sounds")]
    public AudioClip deathSound;
    public AudioClip attackUltimateSound;

    [Range(0f, 1f)]
    public float voiceVolume = 1f;

    [Range(0f, 1f)]
    public float effectsVolume = 0.6f;

    private void Start()
    {
        InitializeAudioSources();
    }

    private void InitializeAudioSources()
    {
        // Crear AudioSources si no están asignados
        if (movementAudioSource == null)
            movementAudioSource = CreateAudioSource("MovementAudio", 1f, 5f, 100f);

        if (attackAudioSource == null)
            attackAudioSource = CreateAudioSource("AttackAudio", 1f, 8f, 150f);

        if (specialAudioSource == null)
            specialAudioSource = CreateAudioSource("SpecialAudio", 1f, 10f, 200f);

        if (voiceAudioSource == null)
            voiceAudioSource = CreateAudioSource("VoiceAudio", 1.5f, 15f, 250f);
        if (effectsMixerGroup != null)
        {
            attackAudioSource.outputAudioMixerGroup = effectsMixerGroup;
            specialAudioSource.outputAudioMixerGroup = effectsMixerGroup;
            movementAudioSource.outputAudioMixerGroup = effectsMixerGroup;
        }

        if (voiceMixerGroup != null)
        {
            voiceAudioSource.outputAudioMixerGroup = voiceMixerGroup;
        }

        if (movementMixerGroup != null)
        {
            movementAudioSource.outputAudioMixerGroup = movementMixerGroup;
        }
    }

    private AudioSource CreateAudioSource(string name,
        float minDistance = 1f,
        float maxDistance = 10f,
        float spatialBlend = 1f)
    {
        GameObject audioObject = new GameObject(name);
        audioObject.transform.SetParent(transform);
        audioObject.transform.localPosition = Vector3.zero;

        AudioSource newSource = audioObject.AddComponent<AudioSource>();
        newSource.spatialBlend = spatialBlend;
        newSource.minDistance = minDistance;
        newSource.maxDistance = maxDistance;
        newSource.rolloffMode = AudioRolloffMode.Logarithmic;

        return newSource;
    }

    // ========== MÉTODOS PARA ANIMATION EVENTS ==========

    // 🦎 MOVIMIENTO
    public void PlayFootstep()
    {
        if (footstepSounds.Length > 0 && movementAudioSource != null)
        {
            AudioClip randomFootstep = footstepSounds[Random.Range(0, footstepSounds.Length)];
            movementAudioSource.pitch = Random.Range(footstepPitchMin, footstepPitchMax);
            movementAudioSource.PlayOneShot(randomFootstep, footstepVolume);
        }
    }

    // 🗡️ ATAQUE
    

    // 🔥 ATAQUES DE FUEGO
    public void PlayBasicAttack()
    {
        PlaySound(basicAttackSound, attackAudioSource, attackVolume, "ataque de fuego");
    }
    public void PlayUltimate()
    {
        PlaySound(attackUltimateSound, voiceAudioSource, voiceVolume, "gruñido de ataque");
    }

    public void PlayDeathSound()
    {
        PlaySound(deathSound, voiceAudioSource, voiceVolume, "sonido de muerte");
    }
    // 🔧 MÉTODO GENÉRICO
    private void PlaySound(AudioClip clip, AudioSource source, float volume, string soundName = "sonido")
    {
        if (clip != null && source != null)
        {
            source.PlayOneShot(clip, volume);
        }
        else
        {
            Debug.LogWarning($"❌ No se pudo reproducir {soundName}: Clip o AudioSource nulo");
        }
    }

    // 🎚️ MÉTODOS DE CONTROL
    public void SetMasterVolume(float volume)
    {
        AudioListener.volume = Mathf.Clamp01(volume);
    }

    public void StopAllSounds()
    {
        if (movementAudioSource != null) movementAudioSource.Stop();
        if (attackAudioSource != null) attackAudioSource.Stop();
        if (specialAudioSource != null) specialAudioSource.Stop();
        if (voiceAudioSource != null) voiceAudioSource.Stop();
    }

    public void PauseAllSounds()
    {
        if (movementAudioSource != null) movementAudioSource.Pause();
        if (attackAudioSource != null) attackAudioSource.Pause();
        if (specialAudioSource != null) specialAudioSource.Pause();
        if (voiceAudioSource != null) voiceAudioSource.Pause();
    }

    public void ResumeAllSounds()
    {
        if (movementAudioSource != null) movementAudioSource.UnPause();
        if (attackAudioSource != null) attackAudioSource.UnPause();
        if (specialAudioSource != null) specialAudioSource.UnPause();
        if (voiceAudioSource != null) voiceAudioSource.UnPause();
    }
}
