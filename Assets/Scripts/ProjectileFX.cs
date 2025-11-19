using System;
using UnityEngine;
using UnityEngine.Events;

public class ProjectileFX : MonoBehaviour
{
    //Creamos un evento de unity que recibirá un vector 3 como parámetro
    public UnityEvent<Vector3> OnImpact;

    // Evento de unity que se lanzará cuando el proyectil sea inicializado 
    public UnityEvent OnInitialize;

    //Referencia al proyectil al que suscribiremos los eventos
    public Projectile projectile;

    void OnEnable()
    {
        //Nos suscribimos con un metodo que carece de parametros
        projectile.OnInitialize += OnInitialize.Invoke;
        //Nos suscribimos al action de impacto con el invoke del unity event
        //Indicar que ambos coinciden con el parámetro requerido (vector 3) de no ser así, esto no sería posible
        projectile.OnImpact += OnImpact.Invoke;
    }

    void OnDestroy()
    {
        //Nos desuscribimos para evitar que se traten de ejecutar metodos en objetos ya eliminados.
        projectile.OnInitialize -= OnInitialize.Invoke;
        projectile.OnImpact -= OnImpact.Invoke;
    }

    
 }
