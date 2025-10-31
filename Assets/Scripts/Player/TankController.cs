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

    public bool isPaused = true;
    private bool isCheats = false;
    public CanvasGroup cheatsCanvasGroup;
    public ParticleSystem dashParticlesL;
    public ParticleSystem dashParticlesR;
    public bool showGizmos = false;
    [Header("Player Movement")] public float movementSpeed = 8f;
    public float rotationSpeed = 14f;
    public float acceleration = 30f;
    [Header("Jump")] public float jumpForce = 10f;
    [Header("Dash")] public float dashForce = 10f;
    [Header("Aiming")] public float camRayLength;
    public LayerMask pointerLayer;
    public float coins;
    public Transform aimingPivot;

    private Vector2 _mousePosition;
    private Vector3 _aimingDirection;
    bool _directionalAiming;

    //Para almacenar la referencia de la camara principal
    private Camera mainCamera;

    [Header("Shooting")]
    public float shootDelay = .5f;

    private float shootTime = 0;
    bool leftCanon = true;
    private bool rightCanon = true;
    public Transform leftCanonPoint;
    public Transform rightCanonPoint;
    public string bulletType = "RegularBullets";

    [Header("Missiles")]
    public Transform[] missileShootingPoints;

    public float missileDelay = .5f;
    public float missileTime = 0f;
    public float angleDetection = 45f;
    public float rangeDetection = 50f;

    public string missilePoolId = "Missile";

    //Offset conforme al punto de impacto, para asegurar que golpee el suelo
    public Vector3 missileAimOffset = new Vector3(0f, -2f, 0f);


    [Header("Mine")]
    public Transform minePoint;

    public float mineTime = 5f;
    public float mineDelay = .5f;
    public string minePoolId = "Mine";

    [Header("Physics")] public Rigidbody rb;
    public LayerMask groundLayer;
    public Transform groundCheck;
    [SerializeField] private bool grounded;
    public Vector3 groundCheckSize;
    public bool isWalk;
    [Header("Collisions pre detection")] public LayerMask checkLayer;
    public Transform checkPoint;
    public float checkSize = .3f;
    [Range(0, 3)] public float checkDistance = 2f;
    [SerializeField] private bool isWalled;

    [Header("Enemies pre detection")]
    public LayerMask enmiesLayer;

    public float checkSizeToEnemies = 1f;
    bool _isEnemyDetected;

    [Header("Engine Sound")] public AudioSource tankEngineSound;
    public float tankBasePitch = .4f;
    public float tankMaxPitch = 3f;

    [Header("Effects")] public ParticleSystem[] dustParticles;
    private Transform modelTransform;
    private float horizontal = 0f;

    private float vertical = 0f;
    bool _isShooting;

    //Dirección de movimiento
    private Vector3 direction;
    private Vector3 desiredVelocity;

    [Header("Animator")] public Animator animator;
    public static Action<float> OnShootMissile;
    public static Action<float> OnPutMine;
    public TextMeshProUGUI coinText;

    #endregion

    #region Events

    public UnityEvent OnJumpSound;

    public static Action OnActiveMissile;
    public static Action OnActiveMine;


    [ContextMenu("Initilize components")]
    void InitializeComponents()
    {
        tankEngineSound = GetComponentInChildren<AudioSource>();
        rb = GetComponent<Rigidbody>();
    }

    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (coinText != null)
            coinText.text = coins.ToString();
        //Recuperamos la referencia a la camara principal
        mainCamera = Camera.main;
        // if (!TutorialController.Instance.isTutorialOn)
        //     transform.position = GameData.Instance.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        GroundCheck();
        CollisionPreDetection();
        MovementFX();
        Movement();
        if (_isShooting) Shoot();
        if (_directionalAiming) DirectionalAimingBehaviour(_aimingDirection);
        else AimingBehaviour(_mousePosition);
        RotationBehaviour();
        AnimationFeed();

    }

    private void OnDrawGizmos()
    {
        //Si no está activo el mostrado de los gizmos
        if (!showGizmos) return;
        // Mostramos el gizmos del check de colisión frontal
        Gizmos.DrawWireSphere(checkPoint.position, checkSize);
        //Cambiamos el color del proximo gizmos 
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(checkPoint.position, checkSizeToEnemies);
        Gizmos.color = Color.green;
        //Mostramos el gizmos de un cubo para visualizar el control del suelo
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
        Gizmos.color = Color.yellow;

        // Direcciones de los extremos del cono
        Vector3 rightDirection = Quaternion.Euler(0, angleDetection, 0) * checkPoint.forward;
        Vector3 leftDirection = Quaternion.Euler(0, -angleDetection, 0) * checkPoint.forward;

        // Dibujar líneas desde el punto de deteccion hasta los extremos del cono
        Gizmos.DrawRay(checkPoint.position, rightDirection * rangeDetection);
        Gizmos.DrawRay(checkPoint.position, leftDirection * rangeDetection);

    }

    #region Methods

    private void GroundCheck()
    {
        //Solo vamos a comprobar si es mayor que 0 asi que no necesitamos más capacidad en el buffer
        Collider[] coliderBuffer = new Collider[1];
        //Comprobamos si hay contacto con el suelo
        //Lo hacewmos mediante un NonAlloc para no consumir más recursos de los necesarios ya que esta comprobación se hará de forma continua
        Physics.OverlapBoxNonAlloc(
            groundCheck.position,
            groundCheckSize / 2f,
            coliderBuffer,
            transform.rotation,
            groundLayer);
        grounded = coliderBuffer[0] != null;
    }

    /// <summary>
    /// Detecta si hay un collider del layer indicado en conctacto con el checker de colision de desplazamiento
    /// </summary>
    private void CollisionPreDetection()
    {
        Collider[] coliderBuffer = new Collider[1];
        Physics.OverlapSphereNonAlloc(
            checkPoint.position,
            checkSize,
            coliderBuffer,
            checkLayer);
        isWalled = coliderBuffer[0] != null;
    }

    /// <summary>
    /// Realiza el movimiento si se cumplen las condiciones
    /// </summary>
    private void Movement()
    {
        //Componemos el vector de direccion deseado a partir del input
        direction.Set(horizontal, 0f, vertical);
        //Para asegurarnos de que las diagoanles no tiene una magnitud superior a 1 normalizamos el vector
        direction.Normalize();
        //Calculamos la celocidad deseada en base a la velocidad y dirección
        desiredVelocity = direction * movementSpeed;
        //Sitúa el checker de colisión en la direccion que deseamos movernos haciendo uso de un vector 3 temporal para respetar la altura configurada en el objeto vacio 
        Vector3 temp = transform.position + direction * checkDistance;
        temp.y = checkPoint.position.y;
        checkPoint.position = temp;
        //Solo aplicamos y rotamos en caso de cumplir las siguientes condiciones:
        //1- Que haya un input
        //2- Que no haya una detección de colisión frontal
        //3- Que estemos tocando el suelo
        if ((horizontal != 0 || vertical != 0) && !isWalled && grounded)
        {
            isWalk = true;
            Debug.DrawRay(transform.position, rb.linearVelocity);
            //Rotamos el tanque para que mire hacia la dirección a la que apunta la velocidad de movimiento
            // transform.rotation = Quaternion.Slerp(
            //     transform.rotation,
            //     Quaternion.LookRotation(desiredVelocity),
            //     rotationSpeed * Time.deltaTime);
        }
        if ((horizontal == 0 && vertical == 0) || isWalled)
        {
            //Frenamos movimiento si detecta colisión con el checkeo para evitar cabeceo contra paredes o elevaciones con mucha pendiente
            desiredVelocity = Vector3.zero;
            //Para evitar rotaciones no deseadas del rigibody
            rb.angularVelocity = Vector3.zero;
            isWalk = false;
        }
        //aplicamos la velocidad deseada al rigibody
        if (grounded)
            rb.linearVelocity = Vector3.MoveTowards(
                rb.linearVelocity,
                desiredVelocity,
                acceleration * Time.deltaTime);
    }

    /// <summary>
    /// Ejecuta el salto
    /// </summary>
    private void Jump()
    {
        if (grounded)
        {
            StartCoroutine(WaitForEffectJump(.1f));
        }
    }

    IEnumerator WaitForEffectJump(float delay)
    {
        yield return new WaitForSeconds(delay);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        OnJumpSound?.Invoke();
    }

    /// <summary>
    /// Ejecuta el dash
    /// </summary>
    private void Dash()
    {
        dashParticlesL.Play();
        dashParticlesR.Play();
        StartCoroutine(WaitForEffect(.8f));

    }

    IEnumerator WaitForEffect(float delay)
    {
        yield return new WaitForSeconds(delay);
        rb.AddForce(transform.forward * dashForce, ForceMode.Impulse);
    }

    private void RotationBehaviour()
    {
        // Siempre rotar hacia donde apunta el mouse, independientemente del movimiento
        Vector3 mouseDirection = aimingPivot.position - transform.position;
        mouseDirection.y = 0; // Mantener en plano horizontal

        if (mouseDirection.magnitude > 0.1f && !isWalled && grounded)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(mouseDirection),
                rotationSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Apuntado de la torreta
    /// </summary>
    private void AimingBehaviour(Vector2 mousePosition)
    {
        //Preparamos un rayo que va desde la posiscion del puntero del mouse en pantalla hacia adelante (en direccion de la proyección de la cámara)
        Vector3 temp = new Vector3();
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);
        RaycastHit groundHit = new RaycastHit();
        if (Physics.Raycast(ray, out groundHit, camRayLength, pointerLayer))
        {
            Vector3 tempPoint = new Vector3(groundHit.point.x, aimingPivot.position.y, groundHit.point.z);
            Vector3 tempPoint2 = new Vector3(transform.position.x, aimingPivot.position.y, transform.position.z);
            Vector3 dirVector = tempPoint - tempPoint2;
            if (dirVector.magnitude < 2f)
            {
                dirVector.Normalize();
                tempPoint = transform.position + dirVector * 2f;
            }
            tempPoint.y = aimingPivot.position.y;
            aimingPivot.position = tempPoint;
        }
    }

    /// <summary>
    /// Apuntado de la torreta a traves de una dirección
    /// </summary>
    private void DirectionalAimingBehaviour(Vector3 direction)
    {
        if (direction.magnitude == 0) return;
        Vector3 nextPivotPosition = aimingPivot.position + transform.position + direction.normalized;
        nextPivotPosition.y = aimingPivot.position.y;
        aimingPivot.position = nextPivotPosition;
        Debug.Log(aimingPivot.position);

    }

    /// <summary>
    /// Dispara el cañon 1 ó 2 según toque
    /// </summary>
    private void Shoot()
    {
        if (Time.time < shootTime) return;
        if (leftCanon)
        {
            animator.SetTrigger("Shoot Left");
            //pedimos a la poll q extraiga y stue un entity del tipo indicado en la posición y relación de leftCanonPoint
            PoolManager.instance.Pull(bulletType, leftCanonPoint.position,
                Quaternion.LookRotation(leftCanonPoint.forward));
        }
        else
        {
            animator.SetTrigger("Shoot Right");
            //pedimos a la poll q extraiga y situe un entity del tipo indicado en la posición y relación de leftCanonPoint
            PoolManager.instance.Pull(bulletType, rightCanonPoint.position,
                Quaternion.LookRotation(rightCanonPoint.forward));
        }
        //Modificamos la velocidad de disparo en función de shoot delay
        animator.SetFloat("ShootSpeed", 1 / shootDelay);
        //calculamos cuando será posible volver a disparar
        shootTime = Time.time + shootDelay;
        //Cambiamos de cañon para el siguiente disparo
        leftCanon = !leftCanon;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    /// <summary>
    /// Dispara un misil por cada punto de disparo de misil
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    void ShootMissile()
    {
        if (Time.time < missileTime) return;
        Collider[] targets = Physics.OverlapSphere(
            transform.position,
            checkSizeToEnemies,
            enmiesLayer);
        if (targets == null || targets.Length == 0) return;
        OnActiveMissile?.Invoke();
        Collider nearEnemyCollider = targets[0];
        float nearDistanceToEnemy = Vector3.Distance(transform.position, nearEnemyCollider.transform.position);
        for (int i = 0; i < targets.Length; i++)
        {
            float temp = Vector3.Distance(transform.position, targets[i].transform.position);
            if (temp < nearDistanceToEnemy)
            {
                nearDistanceToEnemy = temp;
                nearEnemyCollider = targets[i];
            }
        }
        // float angle = Vector3.Angle(nearEnemyCollider.transform.position, transform.forward);
        // if (angle < angleDetection && 5f < distance && distance < rangeDetection)
        foreach (Transform point in missileShootingPoints)
        {
            Missile temp = PoolManager.instance
                .Pull(missilePoolId, point.position, Quaternion.LookRotation(point.forward))
                .GetComponent<Missile>();
            temp.startPosition = point.position;
            temp.targetPosition = nearEnemyCollider.transform.position + missileAimOffset;
            temp.shooterPosition = transform.position;

        }
        missileTime = Time.time + missileDelay;
        OnShootMissile?.Invoke(missileDelay);
        if (TutorialController.Instance.isTutorialOn)
            TutorialController.Instance.PlayerLanzaMisil();
    }

    public void PutMine()
    {
        OnActiveMine?.Invoke();
        if (Time.time < mineTime) return;
        Transform point = minePoint.transform;
        Mine temp = PoolManager.instance
            .Pull(minePoolId, point.position, Quaternion.LookRotation(point.forward))
            .GetComponent<Mine>();
        temp.startPosition = point.position;
        mineTime = Time.time + mineDelay;
        OnPutMine?.Invoke(mineDelay);
        if (TutorialController.Instance.isTutorialOn)
            TutorialController.Instance.PlayerPoneMina();
    }


    /// <summary>
    /// Muestra los efectos especiaqles derivcados del movimiento
    /// </summary>
    private void MovementFX()
    {
        //Modificamos el pitch del motro de forma dinámica segín la velocidad 
        tankEngineSound.pitch = Math.Clamp(tankBasePitch + rb.linearVelocity.magnitude,
            tankBasePitch, tankMaxPitch);
        //Mostramos la emisión de partículas en función de si está o no tocando el suelo
        if (grounded)
        {
            foreach (ParticleSystem ps in dustParticles)
            {
                if (!ps.isPlaying) ps.Play();
            }
        }
        else
        {
            foreach (ParticleSystem ps in dustParticles)
            {
                ps.Stop();
            }
        }
    }

    /// <summary>
    /// Alimenta la informacion para los animators
    /// </summary>
    private void AnimationFeed()
    {
        animator.SetBool("Is Grounded", grounded);
        animator.SetBool("Start Walk", isWalk);
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
            Vector2 inputDir = context.ReadValue<Vector2>();
            horizontal = inputDir.x;
            vertical = inputDir.y;
        }
        else if (context.canceled)
        {
            horizontal = 0;
            vertical = 0;
        }
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
        if (context.performed) ShootMissile();
    }

    public void OnSpecialAttack2(InputAction.CallbackContext context)
    {

        if (context.performed && (GameManager.Instance.level > 0) || TutorialController.Instance.isTutorialOn) PutMine();
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed) Dash();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed) Jump();
    }

    public void OnPause(InputAction.CallbackContext context)
    {

        if (context.started)
        {
            isPaused = GameManager.Instance.pauseMenu._isActive;
            GameManager.Instance.pause(!isPaused);
        }
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
        GameManager.Instance.ShowStats();
    }

    #endregion
}
