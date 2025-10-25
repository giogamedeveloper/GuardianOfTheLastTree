using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class TypewriterEffect : MonoBehaviour
{
    public TextMeshProUGUI messageText; // Texto a modificar
    public float typingSpeed = 0.05f; // Velocidad de escritura (segundos por letra)

    private Coroutine typingCoroutine;
    private bool isTyping = false;

    public bool IsTypingComplete()
    {
        return !isTyping;
    }

    public void StartTyping(string message)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(message));
    }

    private IEnumerator TypeText(string message)
    {
        messageText.text = "";
        foreach (char letter in message)
        {
            messageText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
