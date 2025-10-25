using UnityEngine;

public class MissileBoss : MonoBehaviour
{
    public float damage = 20f;
    public float impactRadius = 3f;
    public float speed = 10f; // velocidad en unidades por segundo
    public float parabolaHeight = 3f;
    public ParticleSystem particle;
    private Vector3 startPos;
    private Vector3 targetPos;
    private float totalDistance;
    private float distanceTravelled = 0f;
    private Vector3 direction;
    private bool hasImpacted = false;

    public void Initialize(Vector3 start, Vector3 target, float parabolaHeight)
    {
        startPos = start;
        targetPos = target;
        this.parabolaHeight = parabolaHeight;

        direction = (targetPos - startPos).normalized;
        totalDistance = Vector3.Distance(startPos, targetPos);

        transform.position = startPos;

        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction);
    }

    void Update()
    {
        // Mover en línea recta en la dirección
        float step = speed * Time.deltaTime;
        float oldDistance = distanceTravelled;
        distanceTravelled += step;

        // Limitar a la distancia total
        if (distanceTravelled > totalDistance)
            distanceTravelled = totalDistance;

        float t = distanceTravelled / totalDistance;
        Vector3 currentPos = Vector3.Lerp(startPos, targetPos, t);

        // Añadir la parábola en altura
        float parabolaOffset = 4 * parabolaHeight * t * (1 - t);
        currentPos.y += parabolaOffset;

        transform.position = currentPos;

        // Destruir si llega a la distancia
        if (distanceTravelled >= totalDistance)
        {
            particle.Play();
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasImpacted) return;

        if (other.CompareTag("Player"))
        {
            var health = other.GetComponent<PlayerHealth>();
            if (health != null)
                health.TakeDamage(damage);
            particle.Play();
            hasImpacted = true;
            Destroy(gameObject);
        }
    }
}
