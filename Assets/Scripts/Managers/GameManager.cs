using System.Linq;
using UnityEngine;
using static InputSystem_Actions;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    public TankController tankController;

    public ExpPlayer expPlayer;
    public DataManager dataManager;
    public SceneController sceneController;
    public GameController gameController;
    public AttackEnemyMelee attackMutant;
    public AttackVampire attackVampire;
    Enemy _enemy;
    bool _showPanel = true;
    public bool bossDead = false;
    public int level;

    [Header("PlayerStats")]
    public PlayerHealth playerHealth;

    public Projectile projectile;

    [Header("EnemyStats")]
    public EnemyHealth enemyHealth;

    public Projectile enemyProjectile;
    public PauseMenu pauseMenu;

    [SerializeField]
    private CanvasGroup _statCanvasGroup;

    private static GameManager _instance;
    public static GameManager Instance => _instance;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        Time.timeScale = 1;
        bossDead = false;
        if (attackVampire == null)
            attackVampire = gameObject.GetComponent<AttackVampire>();
        if (attackMutant == null)
            attackMutant = gameObject.GetComponent<AttackEnemyMelee>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ShowStats()
    {
        _showPanel = !_showPanel;
        Time.timeScale = _showPanel ? 1 : 0;
        _statCanvasGroup.alpha = _showPanel ? 0 : 1;
        Debug.Log("muestra stats");
    }

    public void SetHp(float hp)
    {
        playerHealth.UpdateLifeBar(hp);
    }

    public void SetUpDamage(float damage)
    {
        projectile.projectileDamage = damage;
    }

    public void UpdateStatsEnemies(int hP, int damage)
    {
        GameData.Instance.enemyMutantStats.Hp += hP;
        GameData.Instance.enemyMutantStats.damage += damage;
        GameData.Instance.enemyRobotStats.Hp += hP;
        GameData.Instance.enemyRobotStats.damage += damage;
        GameData.Instance.enemyVampireStats.Hp += hP;
        GameData.Instance.enemyVampireStats.damage += damage;
    }

    public void pause(bool isPause)
    {
        pauseMenu.SetPause(isPause);
    }

    public void Portal(Vector3 playerTransform)
    {
        gameController.level++;
        if (gameController.level == 3)
        {
            GameWin();
        }
        else
        {
            expPlayer.UpdateStatsPlayer(gameController.level, playerTransform);
            // sceneController.LoadSceneAndSetActive(gameController.level);
        }
    }

    public void GameWin()
    {
        gameController.hudGO.SetActive(false);
        gameController.winPanelGO.SetActive(true);
        gameController.pauseMenuGO.SetActive(false);
        gameController.level = 0;

    }
}
