using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class Interactor : MonoBehaviour
{
    [SerializeField] private Transform _interactionPoint;
    [SerializeField] private Transform _interactionPoint2;
    [SerializeField] private float _interactionPointRadius = 0.7f;
    [SerializeField] private LayerMask _interactableMask;	
    [SerializeField] private LayerMask _dirtMask, _sellMask;	
    [SerializeField] private InteractionPromptUI _interactionPromptUI;
    public bool IsInteracting { get; private set; }

    private readonly Collider[] _colliders = new Collider[3];
    [SerializeField] private int _numFound;
    [SerializeField] private int _numDirtFound;


    public IInteractable _interactable;

    public InputAction InteractionKey;

    [SerializeField] private Camera _camera;
    public bool _LookingAtDirt = false;
    public bool IsLookingAtStore = false;

    //private float distance = 10f;
    private Vector3 _oneVector3 = Vector3.one;

    public bool HasInteractable => _interactable != null;
    private void OnEnable()
    {
        InteractionKey = GameInput.playerInputActions.Player.Interaction;
        InteractionKey.Enable();
    }

    private void OnDisable()
    {
        InteractionKey.Disable();
    }
    private void Update()
    {
        _numFound = Physics.OverlapCapsuleNonAlloc(_interactionPoint.position, _interactionPoint2.position, _interactionPointRadius, _colliders, _interactableMask);

        if (_numFound > 0)
        {
            _interactable = _colliders[0].GetComponent<IInteractable>(); 
            if (_interactable == null) return;
        
            if (!_interactionPromptUI.IsDisplayed)
            {
                _interactionPromptUI.SetUp();
            }
            if (GameInput.playerInputActions.Player.Interaction.WasPressedThisFrame())
            {
                _interactable.Interact(this, out bool interactSuccessful);
            }
        }
        else
        {
            if (_interactable != null)
            {
                _interactable.EndInteraction();
                _interactable = null;
            }

            if (_interactionPromptUI.IsDisplayed) 
            {
                _interactionPromptUI.Close();
            }
        }

        _LookingAtDirt = Physics.Raycast(_interactionPoint.position, _interactionPoint2.position - _interactionPoint.position, 10f, _dirtMask);

        _numDirtFound = Physics.OverlapCapsuleNonAlloc(_interactionPoint.position, _interactionPoint2.position, _interactionPointRadius, _colliders, _sellMask);

        IsLookingAtStore = _numDirtFound > 0 && _colliders[0] != null && _colliders[0].CompareTag("Sell");
    }

    public void ForceEndInteraction()
    {
        if (_interactable != null)
        {
            _interactable.EndInteraction();
            _interactable = null;
        }

        if (_interactionPromptUI.IsDisplayed) 
        {
            _interactionPromptUI.Close();
        }
    }

    private Ray CenterRay()
    {
        return _camera.ViewportPointToRay(_oneVector3 * 0.5f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_interactionPoint.position, _interactionPointRadius);
        Gizmos.DrawWireSphere(_interactionPoint2.position, _interactionPointRadius);
    }
}
