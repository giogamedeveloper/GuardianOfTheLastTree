using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    PauseMenu _pauseMenu;

    public void changeToGame()
    {
        Time.timeScale = 1;
        SceneController.Instance.FadeAndLoadScene("Level 0");
    }


    public void ResetScene()
    {
        Time.timeScale = 1;
        SceneController.Instance.FadeAndLoadScene(SceneManager.GetActiveScene().name);
    }

    public void changeToMainMenu()
    {
        if (_pauseMenu != null)
            _pauseMenu.SetPause(false);
        StartCoroutine(LoadMainMenuDelayed());
    }

    IEnumerator LoadMainMenuDelayed()
    {
        yield return new WaitForEndOfFrame(); // Esperar un frame
        Time.timeScale = 1;
        SceneController.Instance.FadeAndLoadScene("MainMenu");
    }


    public void changeToAchievements()
    {
        Time.timeScale = 1;
        SceneController.Instance.FadeAndLoadScene("Achievements");

    }

    public void changeToTutorial()
    {
        Time.timeScale = 1;
        SceneController.Instance.FadeAndLoadScene("Tutorial");
    }

    public void QuitGame()
    {
#if UNITY_STANDALONE
        Application.Quit();
#endif
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
