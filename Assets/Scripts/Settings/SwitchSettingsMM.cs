using System;
using UnityEngine;

public class SwitchSettingsMM : MonoBehaviour
{
    [SerializeField]
    CanvasGroup mainCanvasGroup;

    [SerializeField]
    CanvasGroup pauseCanvasGroup;

    [SerializeField]
    CanvasGroup settingsCanvasGroup;


    [SerializeField]
    GameObject scene;

    [SerializeField]
    private bool _isActive;

    void Awake()
    {
    }

    public void SwitchSettings()
    {
        if (settingsCanvasGroup != null || mainCanvasGroup != null || scene != null)
            _isActive = !_isActive;
        settingsCanvasGroup.gameObject.SetActive(_isActive);
        settingsCanvasGroup.alpha = _isActive ? 1 : 0;
        settingsCanvasGroup.interactable = _isActive;
        settingsCanvasGroup.blocksRaycasts = _isActive;
        mainCanvasGroup.gameObject.SetActive(!_isActive);
        mainCanvasGroup.interactable = !_isActive;
        mainCanvasGroup.blocksRaycasts = !_isActive;
        mainCanvasGroup.alpha = _isActive ? 0 : 1;
       
        scene.SetActive(!_isActive);
    }

    
    public void ExitGame()
    {
        Application.Quit();
    }
}
