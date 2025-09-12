using System;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    #region Variables

    [Header("HUD & UI")]
    // animator del HUD de las oleadas
    public Animator waveUIAnimator;

    public ParticleSystem waveVortexParticles;

    public TextMeshProUGUI remainingEnemiesText;

    public TextMeshProUGUI coinsCollectedText;

    //Referencia al GamObject del HUD
    public GameObject hudGO;
    public GameObject pauseMenuGO;

    //Referencia al GamObject del UI
    public GameObject endPanelGO;

    public GameObject winPanelGO;

    //Texto del numero de oleadas
    public TextMeshProUGUI waveNumberText;

    [Header("Enemies")]
    //Identificadores de las pools de los enemigos
    public string[] enemiesIds;

    public string[] bossIds;

    int _waveEnemies = 15;

    //El nivel en el que está 
    public int level;

    //numero de puntos de spawn de enemigos
    public Transform[] spawnPoints;

    [Header("Waves")]
    //retardo de generacion de enbemigos
    public float spawnDelay = .2f;

    //numeros de enemigos a multiplicar por oleadas
    public int waveEnemyNumberMultiplier = 1; //12

    //Oleada a partir de la cual saldrá un boss
    public int waveBoss = 2; //5

    //Enemigos en la oleada actual
    [SerializeField]
    private int _remainingEnemies;

    //numero de oleada actual
    public int currenWave = 0;

    //Temporizador para la generacion de enemigos
    private float _spawnTimer = 0f;

    // Límite máximo enemigo de una encena
    public int maxEnemiesOnScene = 5;

    //Contador de enemigo en escena 
    private int _enemiesOnScene;

    #endregion

    #region Unity Events

    void OnEnable()
    {
        EnemyHealth.OnEnemyDead += EnemyDead;
        PlayerHealth.OnPlayerDead += GameOver;

    }

    void OnDisable()
    {
        EnemyHealth.OnEnemyDead -= EnemyDead;
        PlayerHealth.OnPlayerDead -= GameOver;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hudGO.SetActive(true);
        endPanelGO.SetActive(false);
        //Iniciamos la primera oleada
        StartWave();
    }

    // Update is called once per frame
    void Update()
    {
        //Solo contamos si el temporizador no ha llegado al final
        if (_spawnTimer <= spawnDelay)
            _spawnTimer += Time.deltaTime;
        //Si quedan enemigos por generar y ha pasado el tiempo de retardo entre enemigos 
        if (_waveEnemies > 0 && _spawnTimer > spawnDelay && _enemiesOnScene < maxEnemiesOnScene + currenWave)
        {
            //Generamos nuevo enemigo
            GenerateEnemy(enemiesIds);
            //Reseteamos el timer
            _spawnTimer = 0f;
        }
        //Si han muerto todos los enemigos
        if (_remainingEnemies <= 0)
            StartWave();
    }

    #endregion

    #region Methods

    private void GenerateEnemy(string[] enemiesIds)
    {
        if (spawnPoints.Length == 0 || enemiesIds.Length == 0)
        {
            Debug.LogError("la oleada debe contener al menos 1 spawn points y un enemigo");
        }

        int randomSpawnIndex = Random.Range(0, spawnPoints.Length);
        //Obtenemos indices aleatorios de los arrays
        int randomEnemyIndex = Random.Range(0, enemiesIds.Length);
        PoolManager.instance.Pull(enemiesIds[randomEnemyIndex], spawnPoints[randomSpawnIndex].position, quaternion.identity);
        //Tras generar un nuevo enemigo, decremento el contador del enemigo a generar
        _waveEnemies--;
        //Incremento de los enemigos en escena 
        _enemiesOnScene++;
    }

    private void EnemyDead()
    {
        //Reducimos el numero de enemigos restantes en 1
        _remainingEnemies--;
        remainingEnemiesText.text = _remainingEnemies.ToString();
        //Decremento de los enemigos en escena 
        _enemiesOnScene--;
    }

    private void StartWave()
    {
        if (currenWave > 0)
        {
            Debug.Log("actualizamos las estadis");
            //Actualizamos las estadísticas del enemigo
            UpdateStatsEnemy();
        }
        //Incrementamos el numnero de oleadas 
        currenWave++;
        //Asignamos el valor del numero de oleada
        waveNumberText.text = currenWave.ToString();
        //Iniciamos la animacion del HUD de oleadas
        waveUIAnimator.SetTrigger("Show");
        waveVortexParticles.Play();
        //calculamos el numero de enemigos que serán generados en esta oleada
        _waveEnemies = waveEnemyNumberMultiplier + (currenWave * 2);
        remainingEnemiesText.text = _waveEnemies.ToString();
        //El numero de enemigos restantes se inicializara con el numero total de enemigos a generar en la oleada
        _remainingEnemies = _waveEnemies;
        //Inicializamos los enemigos la escena 
        _enemiesOnScene = 0;
        if (currenWave == waveBoss)
        {
            GenerateEnemy(bossIds);
        }
    }

    void UpdateStatsEnemy()
    {
        int hp = currenWave * 10;
        int dam = currenWave * 5;
        GameManager.Instance.UpdateStatsEnemies(hp, dam);
    }


    private void GameOver()
    {
        hudGO.SetActive(false);
        endPanelGO.SetActive(true);
        pauseMenuGO.SetActive(false);
    }

    #endregion
}
