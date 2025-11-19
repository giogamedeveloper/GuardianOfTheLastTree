using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup pauseCanvasGroup;

    [SerializeField]
    private CanvasGroup hudCanvasGroup;

    [SerializeField]
    private CanvasGroup settingsCanvasGroup;

    [SerializeField]
    private static PlayerInput _playerInput;

    [SerializeField] InputController _inputController;
    public bool _isActive;
    public static Action OnPause;

    void OnEnable()
    {
        OnPause += TogglePause;
    }

    void OnDisable()
    {
        OnPause -= TogglePause;
    }

    public void TogglePause()
    {
        _isActive = !_isActive;
        SetPause(_isActive);
    }

    void SetPause()
    {
        SetPause(_isActive);
    }

    // Update is called once per frame
    public void SetPause(bool isActive)
    {
        _isActive = isActive;
        // SwitchUIPlayer(!_isActive);
        Time.timeScale = _isActive ? 0 : 1;

        if (pauseCanvasGroup != null)
        {
            pauseCanvasGroup.alpha = _isActive ? 1 : 0;
            pauseCanvasGroup.interactable = _isActive;
            pauseCanvasGroup.blocksRaycasts = _isActive;
        }

        if (hudCanvasGroup != null)
        {
            hudCanvasGroup.interactable = !_isActive;
            hudCanvasGroup.blocksRaycasts = !_isActive;
            hudCanvasGroup.alpha = _isActive ? 0 : 1;
        }
    }

    public void Settings(bool isSettings)
    {
        Debug.Log(isSettings);

        if (settingsCanvasGroup != null)
        {
            settingsCanvasGroup.alpha = isSettings ? 1 : 0;
            settingsCanvasGroup.interactable = isSettings;
            settingsCanvasGroup.blocksRaycasts = isSettings;
        }

        if (pauseCanvasGroup != null)
        {
            pauseCanvasGroup.alpha = isSettings ? 0 : 1;
            pauseCanvasGroup.interactable = !isSettings;
            pauseCanvasGroup.blocksRaycasts = !isSettings;
        }

    }
    private void OnDestroy()
    {
        // Asegurar que el tiempo se reanude y el input se resetee
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
            Debug.Log("⏰ TimeScale reseteado al destruir PauseMenu");
        }
    }
   
}
