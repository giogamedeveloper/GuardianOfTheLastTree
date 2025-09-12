using System;
using TMPro;
using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    Rigidbody rb;
    public Transform player; // Referencia al transform del jugador
    public float speed = 5f;
    public float push = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = new Vector3(push * Time.deltaTime, 0f, 0f);
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("No se encontró ningún objeto con la etiqueta 'Player' en la escena.");
        }
    }

    void Update()
    {
        GoToPlayer();
    }

    public void GoToPlayer()
    {
        Debug.Log(player.position);
        transform.position = Vector3.Lerp(transform.position, player.position, speed * Time.deltaTime);
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
            GameManager.Instance.tankController.UpdateCoinsCollected();
        }
    }
}
