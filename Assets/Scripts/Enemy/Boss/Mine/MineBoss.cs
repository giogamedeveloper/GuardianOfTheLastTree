using System;
using UnityEngine;

public class MineBoss : MonoBehaviour
{
    public Animator animator; // referencia al Animator
    public ParticleSystem explosionEffect; // asigna en inspector las partículas de explosión
    public int damage = 20;

    void Start()
    {
        if (animator == null)
            // Cargar la referencia al Animator para evitar GetComponentCost
            animator = GetComponent<Animator>();
    }

    // Este método inicializa la mina (puedes agregar más lógica)
    public void Initialize()
    {
        // Por ejemplo, si quieres cargar referencias manualmente, hacerlo aquí
        // animator = GetComponent<Animator>();
        // explosionEffect = ... (si aún no la asignaste desde inspector)
    }

    // Activar explosión después de x segundos
    public void DetonateAfter(float time)
    {
        Invoke("Explode", time);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Asumiendo que el jugador tiene un método TakeDamage()
            other.GetComponent<PlayerHealth>()?.TakeDamage(damage);
        }
    }

    private void Explode()
    {
        // Activar la animación de explosión
        if (animator != null)
        {
            animator.SetTrigger("ExplodeTrigger");
        }
        // O activar las partículas, si las tienes
        if (explosionEffect != null)
        {
            explosionEffect.Play();
        }

        // Destruir la mina después de la animación o efecto
        Destroy(gameObject, 1f); // Ajusta según la duración de la animación
    }
}
