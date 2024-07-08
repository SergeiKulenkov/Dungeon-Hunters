using UnityEngine;
using UnityEngine.UI;
using System;

public class StartUIController : MonoBehaviour
{
    ///////////////////////////////////////////
    // Fields
    // events
    public event Action OnStartPressed;
    public event Action OnLoadPressed;
    public event Action OnOptionsPressed;
    public event Action OnQuitPressed;

    // const
    private const string BUTTONS = "Buttons";
    private const string LOAD_BUTTON_PATH = BUTTONS + "/LoadButton";
    
    ///////////////////////////////////////////
    // Methods

    public void OnStartButtonPressed() => OnStartPressed?.Invoke();
    public void OnLoadButtonPressed() => OnLoadPressed?.Invoke();
    public void OnOptionsButtonPressed() => OnOptionsPressed?.Invoke();
    public void OnExitButtonPressed() => OnQuitPressed?.Invoke();

    public void DisableLoadButton()
    {
        Button loadButton = transform.Find(LOAD_BUTTON_PATH).GetComponent<Button>();
        loadButton.interactable = false;
    }
}
