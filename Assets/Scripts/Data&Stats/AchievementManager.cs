using System;
using System.Linq;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    public static Action<string, string> OnAchievementUnlock;

    void OnEnable()
    {
        //suscripcion al metodo de EnemyHealth
        EnemyHealth.OnEnemyDead += () => IncreaseStatAndCheckAchievement("Kill", 1);
        EnemyHealth.OnBossDead += () => IncreaseStatAndCheckAchievement("Level", 1);
        //suscripcion al metodo de PlayerHealth
        PlayerHealth.OnPlayerDead += () => IncreaseStatAndCheckAchievement("Fall", 1);
        TankController.OnActiveMissile += () => IncreaseStatAndCheckAchievement("Skill", 1);
        TankController.OnActiveMine += () => IncreaseStatAndCheckAchievement("Skill", 1);
        //El tutorial
        ObstaclesDamage.OnTutorialUp += () => IncreaseStatAndCheckAchievement("Tutorial", 1);
    }

    /// <summary>
    /// Incrementa una estadistica y veridica sus logros asociados
    /// </summary>
    /// <param name="code"></param>
    /// <param name="amount"></param>
    public void IncreaseStatAndCheckAchievement(string code, int amount)
    {
        // Recuperamos la estadistica solicitada del DataManager mediante el uso de LinQ haciendo una consulta para recuperar aquella que coincida con el codigo indicado
        Stat stat = DataManager.Instance.data.statistics.FirstOrDefault(s => s.code == code);
        //Si no exsite no hacemos nada
        if (stat == null) return;
        //Incrementamos el valor de la estadistica
        stat.value += amount;
        //Recorremos todos los logros qye dependen de la estadistica que no hayan sido desbloqueadas
        Achievement[] achievements =
            DataManager.Instance.data.achievements.Where(a => a.statCode == code && !a.unlocked).ToArray();
        bool needSave = false;
        foreach (Achievement achievement in achievements)
        {
            //Si el valor actual desbloquea un logro, lo marcamos como completado
            if (stat.value >= achievement.targetAmount)
            {
                achievement.unlocked = true;
                OnAchievementUnlock?.Invoke(achievement.name, achievement.imageName);
                //Additionally, we ask the data manager to save the process if it needs to be saved.
                needSave = true;
            }
        }
        if (needSave) DataManager.Instance.Save();
    }
}
