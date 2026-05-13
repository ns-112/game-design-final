using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class NameInput : MonoBehaviour
{
    public string playerName;
    public TMP_InputField inputField;
    public TMP_Text displayText;

    public TMP_Text label3;

    public bool nameFinalized = false;
    public bool inputActive = false;

    void OnEnable()
    {
        inputField.text = "";

        inputField.onValueChanged.AddListener(UpdateDisplay);
        inputField.onSubmit.AddListener(OnSubmit);
    }

    void OnDisable()
    {
        inputField.onValueChanged.RemoveListener(UpdateDisplay);
        inputField.onSubmit.RemoveListener(OnSubmit);
    }

    public void FocusInput()
    {
        if (!inputActive) return;

        EventSystem.current.SetSelectedGameObject(inputField.gameObject);
        inputField.ActivateInputField();
        inputField.Select();
    }

    void UpdateDisplay(string text)
    {
        displayText.text = $"[{text}]";
    }

    void OnSubmit(string text)
    {
        playerName = text;
        nameFinalized = true;

        label3.text += $" {text}.";

        Debug.Log("Submitted: " + text);

        inputField.DeactivateInputField();
        inputField.ReleaseSelection();
    }

}