using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputActionAsset actionAssets;

    [SerializeField]
    private CanvasGroup pauseCanvasGroup;

    [SerializeField]
    private CanvasGroup hudCanvasGroup;

    [SerializeField]
    private CanvasGroup settingsCanvasGroup;

    public bool _isActive;

    // Update is called once per frame
    public void SetPause(bool isActive)
    {
        _isActive = isActive;
        ToggleInputs(!_isActive);
        Time.timeScale = _isActive ? 0 : 1;
        // Actualiza el parametro de verificacion
        pauseCanvasGroup.alpha = _isActive ? 1 : 0;
        pauseCanvasGroup.interactable = _isActive;

    }

    /// <summary>
    /// Alterna entre Player inputs y UI imputs
    /// </summary>
    private void ToggleInputs(bool playerInput)
    {
        if (playerInput)
        {
            actionAssets.FindActionMap("Player").Enable();
            actionAssets.FindActionMap("UI").Disable();
        }
        else
        {
            actionAssets.FindActionMap("Player").Disable();
            actionAssets.FindActionMap("UI").Enable();
        }
    }

    public void Settings()
    {
        settingsCanvasGroup.alpha = _isActive ? 1 : 0;
        settingsCanvasGroup.interactable = _isActive;
        settingsCanvasGroup.blocksRaycasts = _isActive;
        pauseCanvasGroup.alpha = _isActive ? 0 : 1;
        pauseCanvasGroup.interactable = !_isActive;
        pauseCanvasGroup.blocksRaycasts = !_isActive;
        hudCanvasGroup.interactable = !_isActive;
        hudCanvasGroup.blocksRaycasts = !_isActive;
        hudCanvasGroup.alpha = _isActive ? 0 : 1;
        _isActive = !_isActive;
        ToggleInputs(!_isActive);
    }
}
