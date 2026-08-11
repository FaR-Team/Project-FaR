using UnityEngine;
using FaRUtils.FPSController;
using UnityEngine.Events;
using Utils;

[RequireComponent(typeof(UniqueID))]
public class Cofre : Container, IInteractable
{
    [SerializeField] private InteractionPromptUI _prompt;
    [SerializeField] private Animator _animator;
    private static readonly int IsOpen = Animator.StringToHash("IsOpen");
    private UniqueID _uniqueID;
    public AudioClip ChestOpenClip, ChestCloseClip;
    private bool _isOpen;
    
    public InteractionPromptUI InteractionPrompt => _prompt;
    public Transform InteractionTarget => transform;
    
    
    public string ID => _uniqueID.ID;

    protected void Awake()
    {
        _uniqueID = GetComponent<UniqueID>();
    }

    public void Interact(InteractorBase interactor, out bool interactSuccessful)
    {
        if (!_isOpen)
        {
            _isOpen = true;
            if (_animator != null)
            { 
                _animator.SetBool(IsOpen, true);
                MusicManager.Instance.PlaySFX(ChestOpenClip);
            }
        }
        OnDynamicInventoryDisplayRequested?.Invoke(inventorySystem, 0);
        interactSuccessful = true;
    }

    public void InteractOut()
    {
        this.Log("InteractOut");
    }

    public void EndInteraction()
    {
        if (_isOpen)
        {
            _isOpen = false;
            if (_animator != null)
            { 
                _animator.SetBool(IsOpen, false);
                MusicManager.Instance.PlaySFX(ChestCloseClip);
            }
        }

        if (_prompt != null)
        {
            _prompt.Close();
        }
    }

    public void LoadData(ChestData data)
    {
        inventorySystem = new InventorySystem(data.inventorySystem);
        //transform.position = data.position; Comentado por si queremos alguna vez poder mover cofres
    }
}