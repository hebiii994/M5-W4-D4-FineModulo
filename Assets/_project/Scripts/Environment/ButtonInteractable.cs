using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private string _prompt => "Premi E per usare il bottone";

    public UnityEvent OnInteracted;

    public string InteractionPrompt => _prompt;
    public void Interact()
    {
        OnInteracted?.Invoke();
        Debug.Log("Bottone premuto!");
        
    }
}