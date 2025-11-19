using UnityEngine;
using UnityEngine.InputSystem;
using static InputSystem_Actions;

public class UIActionsHandler : MonoBehaviour, IUIActions
{
    [SerializeField] private PauseMenu pauseMenu;
    
    public void OnNavigate(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 direction = context.ReadValue<Vector2>();
            Debug.Log("UI Navigate: " + context.ReadValue<Vector2>());
            // Aquí manejas la navegación del menú (flechas, WASD, gamepad)
        }
    }

    public void OnPauseClose(InputAction.CallbackContext context)
    {
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("UI Submit pressed");
            // Equivale a "Enter" o "A" en gamepad - para seleccionar opciones
        }
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("UI Cancel pressed");
            // Equivale a "Escape" o "B" en gamepad - para volver atrás
            if (pauseMenu != null)
            {
                // Si estás en settings, vuelve al menú de pausa
                // Si estás en el menú de pausa, despausa
                pauseMenu.SetPause(false);
            }
        }
    }

    public void OnPoint(InputAction.CallbackContext context)
    {
       
    }

    public void OnClick(InputAction.CallbackContext context)
    {
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
    }

    public void OnMiddleClick(InputAction.CallbackContext context)
    {
    }

    public void OnScrollWheel(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 scroll = context.ReadValue<Vector2>();
            Debug.Log("Scroll wheel: " + scroll);
        }
    }

    public void OnTrackedDevicePosition(InputAction.CallbackContext context)
    {
    }

    public void OnTrackedDeviceOrientation(InputAction.CallbackContext context)
    {
    }
    
}