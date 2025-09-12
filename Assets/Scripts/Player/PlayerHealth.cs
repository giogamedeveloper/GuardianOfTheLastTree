using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;


public class PlayerHealth : MonoBehaviour,
    IDamageable<float>
{
    //Vida máxima del jugador
    public float maxHealth = 100f;

    float _previewHealth;

    //Vida actual del jugador
    public float currentHealth;

    [Header("HUD")]
    //Referencia a la imagen de la barra de vida
    [SerializeField]
    private Image _healthBar;

    //Referencia a la imagen de daño
    [SerializeField]
    private Image _damageImage;

    //Velocidad a la que se realizará el flash de la imagen de daño
    [SerializeField]
    float _flashSpeed = 5f;

    //Color de flash de la pantalla
    [SerializeField]
    private Color _flashColor =
        new Color(1f, 1f, 1f, .5f);

    [Header("Events")]
    //Accion y evento a realizar cuando el jugador muera
    public UnityEvent OnDead;

    public Animator anim;

    public ParticleSystem particle;
    public GameObject deathmech;

    //Accion (evento) estatico para informar a los observadores
    public static Action OnPlayerDead;

    //Referencias
    //Referencia al componente de control para desactivarlo llegado el momento
    [SerializeField]
    private TankController _tankController;

    //Para controlar cuando el jugador ha sufrido daño
    private bool _isDamaged;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        FlashDamage();
    }

    [ContextMenu("Set Damage Test")]
    public void SetDamageTest()
    {
        TakeDamage(25);
    }

    #region IDamageable

    /// <summary>
    ///Devuelve true si el jugador se queda sin salud 
    /// </summary>
    /// <returns></returns>
    public bool isDead()
    {
        return currentHealth <= 0f;
    }

    /// <summary>
    /// Ejecuta las acciones necesarias al sufir daño
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="impactPosition"></param>
    public void TakeDamage(float damage,
        Vector3 impactPosition = default(Vector3))
    {
        if (isDead()) return;
        //Indicamos que el jugador acaba de recibir daño
        _isDamaged = true;
        //Aplicamos el daño recibido
        currentHealth -= damage;
        //Actualizamos el estado de la barra de vida
        _healthBar.fillAmount = currentHealth / maxHealth;
        // UpdateLifeBar();
        //Revisar bien que la pantalla se ponga en rojo
        if (currentHealth <= maxHealth / 4)
        {
            _damageImage.color = new Color(_damageImage.color.r, _damageImage.color.g,
                _damageImage.color.b, .5f);
        }
        //Si el jugador ha muerto
        if (isDead())
        {
            Death();
        }
    }

    public void UpdateLifeBar(float value)
    {
        maxHealth = value;
        _previewHealth = currentHealth;
        currentHealth += 10;
        float targetHealth = currentHealth / maxHealth;
        _previewHealth = _previewHealth / maxHealth;
        StartCoroutine(HealthBarAnimation(targetHealth, _previewHealth));
    }

    IEnumerator HealthBarAnimation(float targetHealth, float previewHealth)
    {
        float transitionTime = 1f, elapsedTime = 0f;
        while (elapsedTime < transitionTime)
        {
            elapsedTime += Time.deltaTime;
            _healthBar.fillAmount = Mathf.Lerp(previewHealth, targetHealth, elapsedTime / transitionTime);
            yield return null;
        }
        _healthBar.fillAmount = targetHealth;
    }

    #endregion

    /// <summary>
    /// Método que se encargará de realizar las acciones necesarias para la muerte del jugador.
    /// </summary>
    private void Death()
    {
        //LLamamos al evento estatic para observadores.
        OnPlayerDead?.Invoke();
        //Llamamos al unity Event propio para gestionar los efectos a realizar.
        OnDead?.Invoke();
        //Desactivamos los scripts de control de jugador para que no pueda moverse ni disparara. (Esto se podría gestionar con el propio evento)
        _tankController.enabled = false;
        anim.SetTrigger("Dead");
    }

    void OnParticleCollision(GameObject other)
    {
        TakeDamage(25);
    }

    /// <summary>
    /// Gestiona el flash de la imgane de daño
    /// </summary>
    private void FlashDamage()
    {
        //Si ha recibido daño
        if (_isDamaged)
        {
            //aplicamos elcolor de daño a la imgaen
            _damageImage.color = _flashColor;
            //Reiniciamos la variable de daño
            _isDamaged = false;
        }
        else
        {
            //Si no ha sido dañado nos encargamos de que la imagen vuelva a su estado original (negro transparente)
            _damageImage.color = Color.Lerp(
                _damageImage.color, Color.clear,
                _flashSpeed * Time.deltaTime);
        }

    }
}
