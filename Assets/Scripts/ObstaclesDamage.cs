using System;
using UnityEngine;

public class ObstaclesDamage : MonoBehaviour
{
    public static Action OnTutorialUp;

    private int count;
    public ParticleSystem explosionParticles;

    void OnTriggerEnter(Collider other)
    {
        count++;
        if (count > 3)
        {
            explosionParticles.Play();
            GiveExperience(100);
        }
        
        // gameObject.transform.position.y = transform.position.y + 0.2f;

    }
    public void GiveExperience(int value)
    {
        GameManager.Instance.tankController.GetComponent<ExpPlayer>().LevelExp(value);
    }
}
