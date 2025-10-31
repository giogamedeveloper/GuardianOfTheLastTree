using System;
using UnityEngine;

public class ObstaclesDamage : MonoBehaviour
{
    public static Action OnTutorialUp;

    [SerializeField]
    private Collider _collider;

    [SerializeField]
    private Light _light;

    private int count;
    public ParticleSystem explosionParticles;

    void OnTriggerEnter(Collider other)
    {
        count++;
        if (count > 3 && _collider.isTrigger)
        {
            if (_light != null)
                _light.enabled = true;
            explosionParticles.Play();
            GiveExperience(100);
        }
    }

    public void GiveExperience(int value)
    {
        if (_collider != null)
            _collider.isTrigger = false;
        GameManager.Instance.tankController.GetComponent<ExpPlayer>().LevelExp(value);
    }
}
