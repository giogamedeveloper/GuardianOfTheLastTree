using TMPro;
using UnityEngine;

public class TranslateText : MonoBehaviour
{
    [SerializeField]
    private string _text;

    [SerializeField] TextMeshProUGUI _textMesh;

    [SerializeField]
    private TMP_Dropdown _dropdown;

    void OnEnable()
    {
        TranslateManager.OnLanguageChanged += () => ChangeText();
    }

    void OnDisable()
    {
        TranslateManager.OnLanguageChanged -= () => ChangeText();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_text == null && _dropdown != null)
            _textMesh.text = TranslateManager.Instance.GetText(_dropdown.options[_dropdown.value].text);
        _textMesh.text = TranslateManager.Instance.GetText(_text);
    }

    public void ChangeText()
    {
        _textMesh.text = TranslateManager.Instance.GetText(_text);
    }
}
