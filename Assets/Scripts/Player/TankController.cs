using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static InputSystem_Actions;


public class TankController : MonoBehaviour, IPlayerActions
{
    #region Variables

    [Header("References")]
    [SerializeField]
    private TankFX _tankFX;

    [SerializeField]
    private TankAiming _tankAiming;

    [SerializeField]
    private TankMovement _tankMovement;

    [SerializeField]
    private TankShooting _tankShooting;

    private bool isCheats = false;
    public CanvasGroup cheatsCanvasGroup;
    public float coins;
    private Vector2 _mousePosition;
    private Vector3 _aimingDirection;
    bool _directionalAiming;

    bool _isEnemyDetected;
    private Transform modelTransform;
    private float horizontal = 0f;
    private float vertical = 0f;
    bool _isShooting;

    //Dirección de movimiento
    private Vector3 direction;
    private Vector3 desiredVelocity;
    private bool _inputBlocked = false;
    private float _inputBlockTime = 0.5f;
    [Header("Animator")] public Animator animator;
    public static Action<float> OnShootMissile;
    public static Action<float> OnPutMine;
    public TextMeshProUGUI coinText;

    #endregion

    #region Events

    public static Action OnActiveMissile;
    public static Action OnActiveMine;

    [SerializeField]
    private PauseMenu pauseMenu;

    bool isPause;

    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (coinText != null)
            coinText.text = coins.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (this == null) return;
        _tankMovement.Tick();
        _tankFX.UpdateFX(_tankMovement.isGrounded);
        if (_isShooting) _tankShooting.Shoot();
        if (_directionalAiming) _tankAiming.DirectionalAimingBehaviour(_aimingDirection);
        else _tankAiming.AimingBehaviour(_mousePosition);
        _tankAiming.RotationBehaviour(_tankMovement.isWalled, _tankMovement.isGrounded,
            hasInput: horizontal != 0 || vertical != 0 || _directionalAiming);
        AnimationFeed();
    }

    #region Methods

    private void AnimationFeed()
    {
        animator.SetFloat("Speed", _tankMovement.NormalizedSpeed, 0.1f, Time.deltaTime);
        animator.SetBool("IsGrounded", _tankMovement.isGrounded);
    }

    public void UpdateCoinsCollected()
    {
        coins += 30;
        //Actualizamos las monedas que tengamos en el player
        coinText.text = coins.ToString();
    }

    #endregion


    #region IPlayerActions

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 input = context.ReadValue<Vector2>();
            _tankMovement.SetInput(input.x, input.y);
        }
        else if (context.canceled)
            _tankMovement.SetInput(0, 0);
    }


    public void OnPause(InputAction.CallbackContext context)
    {
        if (_inputBlocked) return;

        if (context.performed)
        {
            // PauseMenu pauseMenu = FindObjectOfType<PauseMenu>();
            if (pauseMenu != null)
            {
                StartCoroutine(BlockInputTemporarily());
                pauseMenu.TogglePause();
            }
        }
    }

    private IEnumerator BlockInputTemporarily()
    {
        _inputBlocked = true;
        yield return new WaitForSecondsRealtime(_inputBlockTime); // Usar tiempo real aunque esté en pausa
        _inputBlocked = false;
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _directionalAiming = true;
            Vector2 input = context.ReadValue<Vector2>();
            if (input.magnitude <= 0.3f) return;
            _aimingDirection = new Vector3(input.x, 0f, input.y);
        }
    }

    public void OnPointer(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _directionalAiming = false;
            _mousePosition = context.ReadValue<Vector2>();
        }
    }


    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
            _isShooting = true;
        else if (context.canceled)
            _isShooting = false;
    }

    public void OnSpecialAttack(InputAction.CallbackContext context)
    {
        if (context.performed && (GameManager.Instance.level > 0) || TutorialController.Instance.isTutorialOn)
            _tankShooting.ShootMissile();
    }

    public void OnSpecialAttack2(InputAction.CallbackContext context)
    {

        if (context.performed && (GameManager.Instance.level > 1) || TutorialController.Instance.isTutorialOn)
            _tankShooting.PutMine();
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed) _tankMovement.Dash();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed) _tankMovement.Jump();
    }

    public void OnCheatsMenu(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isCheats = !isCheats;
            Time.timeScale = isCheats ? 0 : 1;
            // Actualiza el parametro de verificacion
            cheatsCanvasGroup.alpha = isCheats ? 1 : 0;
            cheatsCanvasGroup.interactable = isCheats;
        }
    }

    public void OnShowStats(InputAction.CallbackContext context)
    {
        if (_inputBlocked) return;
        if (context.performed)
        {
            StartCoroutine(BlockInputTemporarily());
            GameManager.Instance.ShowStats();
            if (GameManager.Instance._isTutorialOn)
                TutorialController.Instance.ToggleStats();
        }
    }

    private void OnDestroy()
    {
        // Limpiar todas las corrutinas activas
        StopAllCoroutines();

        // Desregistrar los eventos de input si es necesario
        // Esto depende de cómo esté configurado tu Input System
        _inputBlocked = true;

        // Limpiar referencias
        if (cheatsCanvasGroup != null)
        {
            cheatsCanvasGroup.alpha = 0;
            cheatsCanvasGroup.interactable = false;
        }
    }

    #endregion
}
