using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using Utils;

namespace FaRUtils.FPSController
{
    [RequireComponent(typeof(CharacterController))]
    public class FaRCharacterController : MonoBehaviour
    {
        public static FaRCharacterController instance;

        private CharacterController _controller;
        
        [SerializeField] private Camera cam;
        [SerializeField] private float movementSpeed = 15f;

        [SerializeField] private float defaultWalkSpeed = 6f;
        [SerializeField] private float defaultMovementSpeed = 15f;
        [SerializeField] public float lookSensitivity = 0.05f;

        [SerializeField] Interactor interactor;
        [SerializeField] ThirdPersonCamera thirdPersonCamera;
        [SerializeField] private GameObject thirdPersonModel;
        private float _xRotation = 0f;

        [Header("Parámetros de movimiento")]
        private Vector3 _velocity;
        public float gravity = -9.81f;
        private bool _grounded;
        public float jumpSpeed;
        public bool doWalk;
        [SerializeField] private float acceleration = 30f;
        [SerializeField] private float airAcceleration = 8f;
        [SerializeField] private float jumpHeight = 1.2f;
        [SerializeField] private float airControl = 0.4f;
        [SerializeField] private float fallMultiplier = 2.5f;

        [Header("Parámetros de zoom")]
        public float zoomFOV = 35.0f;
        public float zoomSpeed = 9f;
        private float _targetFOV;
        private float _baseFOV;

        [Header("Parámetros de agacharse")]
        private float _initHeight;
        [SerializeField] private float crouchHeight;
        private bool _thirdPersonMode;

        [Header("Minigame Tools")] 
        [SerializeField] private GameObject spear;
        
        Locations currentLocation;
        private IMinigame currentMinigame; // TODO: Mover a GameManager u otro lado
        
        public Locations CurrentLocation => currentLocation;
        public ThirdPersonCamera ThirdPersonCam => thirdPersonCamera;
        public Camera FPSCamera => cam;
        
        public float MovementSpeed => movementSpeed;
        public float DefaultWalkSpeed => defaultWalkSpeed;
        public float DefaultRunSpeed => defaultMovementSpeed;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
            }
            else instance = this;
            
            _controller = GetComponent<CharacterController>();
            if (cam == null) cam = GetComponentInChildren<Camera>();
        }

        private void Start()
        {
            _initHeight = _controller.height;
            Cursor.lockState = CursorLockMode.Locked;
            SetBaseFOV(cam.fieldOfView);
        }


        private void OnEnable()
        {
            EnableInputActions();
            SceneManager.activeSceneChanged += SceneChangedHandler;
            MinigameManager.OnMinigameStarted += MinigameStartedHandler;
        }
        

        private void OnDisable()
        {
            SceneManager.activeSceneChanged -= SceneChangedHandler;
        }

        private void SceneChangedHandler(Scene arg0 = default, Scene arg1 = default)
        {
            Invoke(nameof(EnableInputActions), 0.1f); 
        }

        public void EnableInputActions()
        {
            GameInput.playerInputActions.Player.Enable();
        }

        private void Update()
        {
            if (_thirdPersonMode) return;
            DoMovement();
            DoLooking();
            DoWalk();
        }

        public void DoLooking()
        {
            if (_movementLockTimer > 0f) return;
            if (TelekinesisController.isRotatingObject) return;

            Vector2 looking = GetPlayerLook();
            float lookX = looking.x * lookSensitivity;
            float lookY = looking.y * lookSensitivity;

            _xRotation -= lookY;
            _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);
            
            cam.transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
            
            transform.Rotate(Vector3.up * lookX);
        }

        private float _movementLockTimer = 0f;

        public void LockMovementFor(float duration)
        {
            if (duration > _movementLockTimer)
            {
                _movementLockTimer = duration;
            }
        }

        private void DoMovement()
        {
            _grounded = _controller.isGrounded;

            if (_grounded && _velocity.y < 0f)
            {
                _velocity.y = -2f;
            }

            Vector2 input = Vector2.ClampMagnitude(GetPlayerMovement(), 1f);

            if (_movementLockTimer > 0f)
            {
                _movementLockTimer -= Time.deltaTime;
                input = Vector2.zero;
            }

            Vector3 moveDirection = transform.right * input.x + transform.forward * input.y;

            Vector3 targetVelocity = moveDirection * movementSpeed;

            if (_grounded)
            {
                _velocity.x = Mathf.MoveTowards(
                    _velocity.x,
                    targetVelocity.x,
                    acceleration * Time.deltaTime
                );

                _velocity.z = Mathf.MoveTowards(
                    _velocity.z,
                    targetVelocity.z,
                    acceleration * Time.deltaTime
                );

                if (GameInput.playerInputActions.Player.Jump.WasPressedThisFrame())
                {
                    _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                }
            }
            else
            {
                Vector3 airTargetVelocity = targetVelocity * airControl;

                _velocity.x = Mathf.MoveTowards(
                    _velocity.x,
                    airTargetVelocity.x,
                    airAcceleration * Time.deltaTime
                );

                _velocity.z = Mathf.MoveTowards(
                    _velocity.z,
                    airTargetVelocity.z,
                    airAcceleration * Time.deltaTime
                );
            }
            
            float currentGravity = _velocity.y < 0f
                ? gravity * fallMultiplier
                : gravity;

            _velocity.y += currentGravity * Time.deltaTime;

            _controller.Move(_velocity * Time.deltaTime);
        }

        private void DoWalk()
        {
            if (!doWalk) return;

            if (GameInput.playerInputActions.Player.Sprint.WasPressedThisFrame())
            {
                movementSpeed = defaultWalkSpeed;
            }
            else if (GameInput.playerInputActions.Player.Sprint.WasReleasedThisFrame())
            {
                movementSpeed = defaultMovementSpeed;
            }
        }

        public void SetBaseFOV(float fov)
        {
            _baseFOV = fov;
            cam.fieldOfView = _baseFOV;
        }
        
        public Vector2 GetPlayerMovement()
        {
            return GameInput.playerInputActions.Player.Movement.ReadValue<Vector2>();
        }

        public Vector2 GetPlayerLook()
        {
            return GameInput.playerInputActions.Player.Look.ReadValue<Vector2>();
        }

        public void Teleport(Transform newPosition)
        {
            _controller.enabled = false;
            transform.SetPositionAndRotation(newPosition.position, newPosition.rotation);
            _controller.enabled = true;
            
        }

        public void SetLocation(Locations location)
        {
            currentLocation = location;
        }

        public void EnableThirdPerson(bool enable, Transform camTarget = null)
        {
            _thirdPersonMode = enable;
            if(enable)
                thirdPersonCamera.ActivateCamera(camTarget);
            else 
                thirdPersonCamera.DeactivateCamera();
            
            
            thirdPersonCamera.SetCameraTarget(camTarget);
            thirdPersonModel.SetActive(enable);
            
            cam.enabled = !enable;
            interactor.enabled = !enable;
        }

        public void SetMinigame(IMinigame minigame)
        {
            currentMinigame = minigame;
            if (minigame == null) return;
            
            currentMinigame.OnMinigameFinished += HandleMinigameFinished;
        }

        private void HandleMinigameFinished()
        {
            DeactivateMinigameTools(currentMinigame);
            currentMinigame.OnMinigameFinished -= HandleMinigameFinished;
            if(_thirdPersonMode) EnableThirdPerson(false);
            SetMinigame(null);
        }

        public void DisableMinigameInput()
        {
            // TODO: Mover tema herramientas a PlayerInventoryHolder o clase MinigameToolHandler, clase abstracta para tools y switch reutilizable para conseguirla
            switch (currentMinigame.Tool)
            {
                case MinigameTools.Spear:
                    spear.GetComponentInChildren<Spear>()?.EnablePivot(false);
                    thirdPersonCamera.EnableCameraInteractor(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void MinigameStartedHandler(IMinigame minigame)
        {
            // TODO: Mover a otra clase tipo ToolController y manejar mejor logica de varias herramientas
            switch (minigame.Tool)
            {
                case MinigameTools.Spear:
                    spear.SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // TODO: Mover a otra clase tipo ToolController y manejar mejor logica de varias herramientas
        private void DeactivateMinigameTools(IMinigame minigame)
        {
            switch (minigame.Tool)
            {
                case MinigameTools.Spear:
                    spear.SetActive(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}