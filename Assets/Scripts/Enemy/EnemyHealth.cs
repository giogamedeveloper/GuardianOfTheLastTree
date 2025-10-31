using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour, IDamageable<float>
{
    //Vida inicial del enemigo
    public float startingHealth;
    private string _robotName = "EnemyRobot(Clone)";
    private string _vampireName = "VampireEnemy(Clone)";
    private string _mutantName = "MutantEnemy(Clone)";

    //Vida actual del enemigo
    [SerializeField]
    private float _currentHealth;

    public float maxHealth;

    [Header("References")]
    //Referencia al animator
    [SerializeField]
    private Animator _animator;

    [Header("Events")]
    public static Action OnEnemyDead;

    public static Action OnBossDead;
    public UnityEvent OnDead;

    [Header("Dissolve")]
    //Referencia al renderer que será disuelto
    public Renderer rend;

    //Para no generar instancias de materiales al modificarlos
    private MaterialPropertyBlock _dissolvePropertyBlock;

    //Tiempo de duracion de la disolucion
    public float dissolveTime = 5f;

    //Alturas para definir el rango del efecto de disolución
    public float dissolveMaxHeight = 2f;
    public float dissolveMinHeight = -2f;

    //contador de tiempo interno
    public GameObject portal;
    private float _dissolveTimer;

    //Contador dealtura interno
    float _dissolveCurrentHeight;
    //Corutina para realizar la disolución

    private Coroutine _dissolveCoroutine;
    //Evento de unity para cuando se haya disuelto completamente el enemigo

    [FormerlySerializedAs("healthBar")] [Header("UI")]
    public Image healthImage;


    public CanvasGroup healthBarCG;
    public float healthBarHideTime = 3f;
    private float _healthBarHideTimer;
    DroppingItems _droppingItems;

    public UnityEvent OnDissolve;
    public UnityEvent OnSolve;


    void Awake()
    {
        _dissolvePropertyBlock = new MaterialPropertyBlock();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _droppingItems = GetComponent<DroppingItems>();
        Revive();
        CheckEnemyAndLevel();

    }

    public void CheckEnemyAndLevel()
    {
        if (gameObject.name == _robotName)
        {

            if (GameManager.Instance.gameController.level == 0)
            {
                startingHealth = 110f;
                GameManager.Instance.enemyProjectile.projectileDamage = 10;
            }
            else if (GameManager.Instance.gameController.level == 1)
            {
                startingHealth = 200f;
                GameManager.Instance.enemyProjectile.projectileDamage = 20;

            }
            else if (GameManager.Instance.gameController.level == 2)
            {
                startingHealth = 300f;
                GameManager.Instance.enemyProjectile.projectileDamage = 30;
            }
            startingHealth += GameData.Instance.enemyRobotStats.Hp;
        }
        else if (gameObject.name == _mutantName)
        {

            if (GameManager.Instance.gameController.level == 0)
            {
                startingHealth = 70f;
            }
            else if (GameManager.Instance.gameController.level == 1)
            {
                startingHealth = 100f;

            }
            else if (GameManager.Instance.gameController.level == 2)
            {
                startingHealth = 130f;
            }
            startingHealth += GameData.Instance.enemyMutantStats.Hp;
        }
        else if (gameObject.name == _vampireName)
        {
            if (GameManager.Instance.gameController.level == 0)
            {
                startingHealth = 150f;
            }
            else if (GameManager.Instance.gameController.level == 1)
            {
                startingHealth = 250f;

            }
            else if (GameManager.Instance.gameController.level == 2)
            {
                startingHealth = 400f;
            }
            startingHealth += GameData.Instance.enemyVampireStats.Hp;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float t = 1 - Mathf.Clamp01(_healthBarHideTimer / healthBarHideTime);
        healthBarCG.alpha = Mathf.Lerp(1f, 0f, t);
        if (_healthBarHideTimer > 0f)
            _healthBarHideTimer -= Time.deltaTime;
    }

    public bool isDead()
    {
        return _currentHealth <= 0;
    }

    public void TakeDamage(float damage, Vector3 impactPosition = default(Vector3))
    {
        if (isDead()) return;
        _currentHealth -= damage;
        healthImage.fillAmount = _currentHealth / startingHealth;
        _healthBarHideTimer = healthBarHideTime;
        if (isDead())
        {
            Death();
        }
    }

    void Death()
    {
        _animator.SetTrigger("Dead");
        OnEnemyDead?.Invoke();
        OnDead?.Invoke();
        if (gameObject.name == "EnemyRobotBoss(Clone)" || gameObject.name == "MutantEnemyBoss")
        {
            GiveExperience(100);
            Instantiate(portal, transform.position, Quaternion.identity);
            OnBossDead?.Invoke();
            GameManager.Instance.bossDead = true;
        }
        else if (gameObject.name == "VampireEnemyBoss")
            GameManager.Instance.GameWin();
        if (gameObject.name == _robotName || gameObject.name == _mutantName || gameObject.name == _vampireName)
        {

            GiveExperience(10);
            _droppingItems.ItemDropped();

        }
        //Si es el enemigho hago un portal donde murió y así puedo pasar de nivel
        if (_dissolveCoroutine != null) StopCoroutine(_dissolveCoroutine);
        _dissolveCoroutine = StartCoroutine(DissolveCoroutine());
    }

    public void Revive()
    {
        _currentHealth = startingHealth;
        healthImage.fillAmount = _currentHealth / startingHealth;
        _healthBarHideTimer = 0f;
        // Paramos la corrutina si esta en marcha
        if (_dissolveCoroutine != null) StopCoroutine(_dissolveCoroutine);
        _dissolveCoroutine = StartCoroutine(SolveCoroutine());
        //Reseteamos el trigger de muerte para evitar que se repita
        _animator.ResetTrigger("Dead");
        //Reiniciamos el animator
        //Reiniciamos el animator
        _animator.Play("Forward");
        //Obtenenmos el property block del renderer
        rend.GetPropertyBlock(_dissolvePropertyBlock);
        //Modificamos el valor usando la cariable expuesta del material
        _dissolvePropertyBlock.SetFloat("_CutoffHeight", dissolveMaxHeight);
        rend.SetPropertyBlock(_dissolvePropertyBlock);
    }

    /// <summary>
    ///Corrutina que ejecuta la disolución 
    /// </summary>
    /// <returns></returns>
    private IEnumerator DissolveCoroutine()
    {
        _dissolveTimer = dissolveTime;
        while (_dissolveTimer >= 0f)
        {
            //Obtenenmos el property block del renderer
            rend.GetPropertyBlock(_dissolvePropertyBlock);
            //Modificamos el valor usando la variable del material
            _dissolvePropertyBlock.SetFloat("_CutoffHeight",
                Mathf.Lerp(dissolveMaxHeight, dissolveMinHeight, 1 - (_dissolveTimer / dissolveTime)));
            //Devolvemos el property block con las modificaciones
            rend.SetPropertyBlock(_dissolvePropertyBlock);
            //Actualizamos el contador de tiempo
            _dissolveTimer -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        //al finalizar ejecutamos el evento
        OnDissolve?.Invoke();
    }

    private IEnumerator SolveCoroutine()
    {
        _dissolveTimer = dissolveTime;
        while (_dissolveTimer >= 0f)
        {
            //Obtenenmos el property block del renderer
            rend.GetPropertyBlock(_dissolvePropertyBlock);
            //Modificamos el valor usando la variable del material
            _dissolvePropertyBlock.SetFloat("_CutoffHeight",
                Mathf.Lerp(dissolveMaxHeight, dissolveMinHeight, _dissolveTimer / dissolveTime));
            //Devolvemos el property block con las modificaciones
            rend.SetPropertyBlock(_dissolvePropertyBlock);
            //Actualizamos el contador de tiempo
            _dissolveTimer -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        //al finalizar ejecutamos el evento
        OnSolve?.Invoke();
    }

    public void GiveExperience(int value)
    {
        GameManager.Instance.tankController.GetComponent<ExpPlayer>().LevelExp(value);
    }
}
