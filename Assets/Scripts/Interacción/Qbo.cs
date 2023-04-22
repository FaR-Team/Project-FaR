using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Qbo : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject _prompt;

    public GameObject InteractionPrompt => _prompt;

    public GameObject qboUI;
    public AudioSource qboSong;

    private void Awake() {
        _prompt = GameObject.FindGameObjectWithTag("HouseInteraction");
    }

    public void Interact(Interactor interactor, out bool interactSuccessful)
    {
        qboUI.SetActive(true);
        qboSong.Play();
        Debug.Log("Interactuando con qbo");
        interactSuccessful = true;
    }

    private void Update() 
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            qboUI.SetActive(false);
            qboSong.Stop();
        }
    }

    public void EndInteraction()
    {
        Debug.Log("Terminando interacción con qbo");
    }
}