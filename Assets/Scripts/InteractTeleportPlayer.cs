using System.Collections;
using System.Collections.Generic;
using FaRUtils.FPSController;
using UnityEngine;

public class InteractTeleportPlayer : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform newPosition;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject InteractionPrompt { get; }
    public void Interact(Interactor interactor, out bool interactSuccessful)
    {
        interactor.gameObject.GetComponent<FaRCharacterController>().Teleport(newPosition);
        interactSuccessful = true;
    }

    public void InteractOut()
    {
        throw new System.NotImplementedException();
    }

    public void EndInteraction()
    {
        throw new System.NotImplementedException();
    }
}
