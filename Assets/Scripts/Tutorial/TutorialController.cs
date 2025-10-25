using System.Collections;
using TMPro;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    public TextMeshProUGUI messageText;
    public GameObject messagePanel;
    public GameObject arrowIndicator;
    public TypewriterEffect typewriter;

    private int currentStep = 0;
    public bool isTutorialOn = false;
    private static TutorialController _instance;
    public static TutorialController Instance => _instance;
    [SerializeField]
    private GameObject objetivoMoverse;
    [SerializeField]
    private GameObject objetivoEliminar;
    [SerializeField]
    private GameObject objetivoMisil;
    [SerializeField]
    private GameObject objetivoMina;
    bool hasLanzadoMisil;
    bool hasPuestoMina;

    void Start()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);

        isTutorialOn = true;
        messagePanel.SetActive(false);
        arrowIndicator.SetActive(false);
        StartTutorial();
    }
    void StartTutorial()
    {
        ShowTypingMessage("Muévete hacia la zona marcada.", true);
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
                    AdvanceStep("¡Bien hecho! Ahora dispara para eliminar un objetivo.");
                }
                break;
            case 2:
                // Detectar si el objetivo fue destruido
                if (!objetivoEliminar.GetComponent<Collider>().enabled)
                {
                    AdvanceStep("¡Perfecto! Ahora dispara un misil al objetivo.");
                }
                break;
            case 3:
                // Detectar si el misil fue disparado
                // Supongamos que cuando el misil impacta, alguien llama a PlayerLanzaMisil
                if (hasLanzadoMisil)
                {
                    AdvanceStep("¡Buen trabajo! Ahora coloca una mina en la zona marcada.");
                }
                break;
            case 4:
                // Detectar si la mina fue colocada
                if (objetivoMina != null && IsCloseToTarget(objetivoMina) && hasPuestoMina)
                {
                    EndTutorial();
                }
                break;
        }
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
        if (currentStep + 1 <= 4)
        {
            switch (currentStep + 1)
            {
                case 2:
                    objetivoEliminar.SetActive(true);
                    ShowArrowAtTarget(objetivoEliminar);
                    break;
                case 3:
                    objetivoMisil.SetActive(true);
                    ShowArrowAtTarget(objetivoMisil);
                    break;
                case 4:
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
        ShowTypingMessage("¡Has completado el tutorial!", false);
        arrowIndicator.SetActive(false);
    }

    private void ShowTypingMessage(string message, bool showPanel)
    {
        StartCoroutine(ShowMessageAndAutoHide(message, showPanel, 3f)); // 3 segundos, por ejemplo
    }

    private IEnumerator ShowMessageAndAutoHide(string message, bool showPanel, float delayAfterComplete)
    {
        messagePanel.SetActive(true);
        typewriter.StartTyping(message);

        // Esperar a que termine la escritura
        yield return new WaitUntil(() => typewriter.IsTypingComplete());

        if (showPanel)
        {
            // Mantener visible durante un tiempo
            yield return new WaitForSeconds(delayAfterComplete);
            messagePanel.SetActive(false);
        }
    }

    private void ShowArrowAtTarget(GameObject target)
    {
        Debug.Log("Mostrando flecha en: " + target.name);
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

    // Métodos que llamarán cuando el jugador realice la acción correspondiente:
    public void PlayerLanzaMisil()
    {
        hasLanzadoMisil = true;
    }
    
    public void PlayerPoneMina()
    {
        hasPuestoMina = true;
    }
}
