using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControllerOld : MonoBehaviour
{
    public void ChangeScene(string name)
    {
        if (name == "Level 0")
        {
            if (GameData.Instance != null)
            {
                SceneManager.LoadScene("Level " + GameData.Instance.playerStats.level);
            }
            else
            {
                SceneManager.LoadScene(name);
            }
            Debug.Log("Level " + GameData.Instance.playerStats.level);
        }
        else
        {
            SceneManager.LoadScene(name);
        }
    }

    public void Restart()
    {
        //We reset the time scale.
        Time.timeScale = 1;
        //We reload the current scene.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ResetData(string name)
    {
        GameData.Instance.InitialStats();
        SceneManager.LoadScene(name);
    }
}
