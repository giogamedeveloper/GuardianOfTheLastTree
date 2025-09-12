using UnityEngine;

public class PoisonPool : MonoBehaviour
{
    public float duration = 1f; // Duración del charco en segundos
    public int damage = 20; // Daño que causa por contacto
    private float timer;

    void Start()
    {
        timer = duration;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Asumiendo que el jugador tiene un método TakeDamage()
            other.GetComponent<PlayerHealth>()?.TakeDamage(damage);
        }
    }
}
