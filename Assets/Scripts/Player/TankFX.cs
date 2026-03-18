using UnityEngine;

public class TankFX : MonoBehaviour
{
    [Header("Engine Sound")]
    public AudioSource tankEngineSound;
    public float tankBasePitch = .4f;
    public float tankMaxPitch = 3f;

    [Header("Effects")]
    public ParticleSystem[] dustParticles;

    private Rigidbody _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void UpdateFX(bool grounded)
    {
        tankEngineSound.pitch = Mathf.Clamp(
            tankBasePitch + _rb.linearVelocity.magnitude,
            tankBasePitch,
            tankMaxPitch);

        foreach (ParticleSystem ps in dustParticles)
        {
            if (grounded && !ps.isPlaying) ps.Play();
            else if (!grounded) ps.Stop();
        }
    }
}
