using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Math = System.Math;

public class ExpPlayer : MonoBehaviour
{
    #region Variables

    public float hP;
    public float maxHp;
    public int levelPlayer;
    public int level;
    public float exp;
    private float _previosExp;
    public float maxExp;
    public float damage;
    public int stats;

    [Header("HUD")]
    [SerializeField]
    private Image _expBar;

    private GameObject gO;

    public ParticleSystem levelUp;
    public ParticleSystem circlePS;

    [SerializeField]
    private Image _expBarFill;

    [SerializeField]
    private TankController _tankController;

    [Header("HudStats")]
    public TextMeshProUGUI hpMaxText;

    public TextMeshProUGUI levelText;
    public TextMeshProUGUI levelHUDText;
    public TextMeshProUGUI expText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI statsText;

    #endregion

    public void TextInterface()
    {
        if (hpMaxText.text != null)
            hpMaxText.text = maxHp.ToString();
        if (levelText.text != null)
            levelText.text = levelPlayer.ToString();
        if (levelHUDText.text != null)
            levelHUDText.text = levelPlayer.ToString();
        if (expText.text != null)
            expText.text = exp.ToString();
        if (hpText.text != null)
            hpText.text = hP.ToString();
        if (damageText.text != null)
            damageText.text = damage.ToString();
        if (statsText.text != null)
            statsText.text = stats.ToString();

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetStatsUpdated();
    }

    // Update is called once per frame
    void Update()
    {
        TextInterface();
    }

    #region Methods

    public void LevelExp(int value)
    {
        _previosExp = exp;
        exp += value;
        Debug.Log(exp);
        Debug.Log(maxExp);
        if (exp > maxExp)
        {
            levelPlayer++;
            maxExp = (int)Math.Round(exp * 1.3f);
            stats += 5;
            exp = 1;
            FXPlayerLevelUp();
        }
        if (exp != 0)
        {
            UpdateExpBar(_previosExp);
        }
    }

    public void FXPlayerLevelUp()
    {
        circlePS.Play();
        levelUp.Play();
    }

    void UpdateExpBar(float previusExp)
    {
        float targetExp = exp / maxExp;
        previusExp = previusExp / exp;
        StartCoroutine(ExpBarAnimation(targetExp, previusExp));
    }

    IEnumerator ExpBarAnimation(float targetExp, float previusExp)
    {
        float transitionTime = 1f, elapsedTime = 0f;
        while (elapsedTime < transitionTime)
        {
            elapsedTime += Time.deltaTime;
            _expBar.fillAmount = Mathf.Lerp(previusExp, targetExp, elapsedTime / transitionTime);
            yield return null;
        }
        _expBar.fillAmount = targetExp;
    }

    public void UpDamage()
    {
        if (stats > 0)
        {
            SetDamage(damage);
            damage += 5;
            stats--;
        }
    }

    public void UpHp()
    {
        if (stats > 1)
        {
            SetHP(maxHp);
            maxHp += 10;
            stats -= 2;
        }
    }

    public void UpdateStatsPlayer(int _level, Vector3 _transform)
    {
        GameData.Instance.UpdateStats(_level, _transform, levelPlayer, _level, maxHp, damage, exp, maxExp, stats);

    }

    public void GetStatsUpdated()
    {
        hP = GameData.Instance.playerStats.hpMax;
        maxHp = GameData.Instance.playerStats.hpMax;
        damage = GameData.Instance.playerStats.damage;
        levelPlayer = GameData.Instance.playerStats.levelPlayer;
        exp = GameData.Instance.playerStats.exp;
        maxExp = GameData.Instance.playerStats.exp;
        level = GameData.Instance.playerStats.level;
        stats = GameData.Instance.playerStats.stats;
    }

    public void MaxHP()
    {
        maxHp = 10000f;
        hP = maxHp;
        GameManager.Instance.playerHealth.currentHealth = 10000f;
        SetHP(hP);
    }

    public void MaxDamage()
    {
        damage = 1000f;
        SetDamage(damage);
    }

    public void SkipLevel(int value)
    {
        level = value;
        SceneManager.LoadScene("Level " + level);
    }

    public void SetHP(float value)
    {

        GameManager.Instance.SetHp(value);
    }

    public void SetDamage(float value)
    {
        GameManager.Instance.SetUpDamage(value);
    }

    #endregion
}
