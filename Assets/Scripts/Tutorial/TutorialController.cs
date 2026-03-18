using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    // public TextMeshProUGUI messageText;
    public GameObject messagePanel;
    public GameObject arrowIndicator;
    public TypewriterEffect typewriter;
    bool _isActiveStats = false;
    private int activateStats = 0;
    private int currentStep = 0;
    public bool isTutorialOn = false;
    public static TutorialController Instance { get; private set; }

    [SerializeField]
    private GameObject objetivoMoverse;

    [SerializeField]
    private GameObject objetivoEliminar;

    [SerializeField]
    MenuManager _menuManager;

    [SerializeField]
    private GameObject objetivoMisil;

    [SerializeField]
    private GameObject objetivoMina;

    bool hasLanzadoMisil;
    bool hasPuestoMina;

    void Awake()
    {
        if (Instance != this && Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {

        if (_menuManager == null)
            _menuManager = GetComponent<MenuManager>();
        isTutorialOn = true;
        if (messagePanel != null && arrowIndicator != null)
        {
            messagePanel.SetActive(false);
            arrowIndicator.SetActive(false);
            StartTutorial();
        }
    }

    void StartTutorial()
    {
        ShowTypingMessage("text_Movimiento", true);
        ShowArrowAtTarget(objetivoMoverse);

        currentStep = 1; // Empezamos en paso 1
    }

    void Update()
    {
        switch (currentStep)
        {
            case 1:
                // Ver si el jugador se movió a la posición
                if (objetivoMoverse != null && IsCloseToTarget(objetivoMoverse))
                {
                    AdvanceStep("text_Shoot");
                }
                break;
            case 2:
                if (!objetivoEliminar.GetComponent<Collider>().enabled)
                {
                    AdvanceStep("text_Stats");
                }
                break;
            case 3:
                // Detectar si el objetivo fue destruido
                if (!_isActiveStats && activateStats > 1)
                {
                    AdvanceStep("text_Missile");
                }
                break;
            case 4:
                // Detectar si el misil fue disparado
                if (hasLanzadoMisil && !objetivoMisil.GetComponent<Collider>().enabled)
                {
                    AdvanceStep("text_Mine");
                }
                break;
            case 5:
                // Detectar si la mina fue colocada
                if (objetivoMina != null && IsCloseToTarget(objetivoMina) && hasPuestoMina)
                {
                    EndTutorial();
                }
                break;
        }
    }

    public bool ToggleStats()
    {
        _isActiveStats = !_isActiveStats;
        activateStats++;
        return _isActiveStats;
    }

    private bool IsCloseToTarget(GameObject target, float threshold = 1f)
    {
        if (target == null) return false;
        // Distancia entre jugador y target
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador == null) return false;
        return Vector3.Distance(jugador.transform.position, target.transform.position) < threshold;
    }

    void AdvanceStep(string message)
    {
        ShowTypingMessage(message, true);
        // Actualizar el indicador a la nueva posición
        if (currentStep + 1 <= 5)
        {
            switch (currentStep + 1)
            {
                case 2:
                    HidePreviewsTarget(objetivoMoverse);
                    objetivoEliminar.SetActive(true);
                    ShowArrowAtTarget(objetivoEliminar);
                    break;
                case 3:
                    ShowTypingMessage("text_ShowStats", true);
                    HidePreviewsTarget(objetivoEliminar);
                    break;
                case 4:
                    objetivoMisil.SetActive(true);
                    ShowArrowAtTarget(objetivoMisil);
                    break;
                case 5:
                    HidePreviewsTarget(objetivoMisil);
                    objetivoMina.SetActive(true);
                    ShowArrowAtTarget(objetivoMina);
                    break;
                default:
                    break;
            }
        }
        currentStep++;
    }

    void EndTutorial()
    {
        HidePreviewsTarget(objetivoMina);
        arrowIndicator.SetActive(false);
        activateStats = 0;
        ShowTypingMessage("text_FinishTuto", true);

    }

    private void ShowTypingMessage(string message, bool showPanel)
    {
        StartCoroutine(ShowMessageAndAutoHide(message, showPanel, 4f)); // 3 segundos, por ejemplo
    }

    private IEnumerator ShowMessageAndAutoHide(string message, bool showPanel, float delayAfterComplete)
    {
        messagePanel.SetActive(true);
        typewriter.StartTyping(message);

        // Esperar a que termine la escritura
        yield return new WaitUntil(() => typewriter.IsTypingComplete());
        _menuManager.changeToMainMenu();
        if (showPanel)
        {
            // Mantener visible durante un tiempo
            yield return new WaitForSeconds(delayAfterComplete);
            messagePanel.SetActive(false);
        }
    }

    private void ShowArrowAtTarget(GameObject target)
    {
        if (target != null)
        {
            arrowIndicator.SetActive(true);
            Vector3 positionAboveTarget = target.transform.position + new Vector3(0, 2f, 0);
            arrowIndicator.transform.position = positionAboveTarget;
            Vector3 direction = target.transform.position - arrowIndicator.transform.position;
            if (direction != Vector3.zero)
                arrowIndicator.transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    private void HidePreviewsTarget(GameObject target)
    {
        if (target != null)
        {
            target.SetActive(false);
        }
    }

    // Métodos que llamarán cuando el jugador realice la acción correspondiente:
    public void PlayerLanzaMisil()
    {
        hasLanzadoMisil = true;
    }

    public void PlayerPoneMina()
    {
        Debug.Log("puso mina");
        hasPuestoMina = true;
    }
}
