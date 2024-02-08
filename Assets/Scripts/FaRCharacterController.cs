using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FaRUtils.FPSController
{
    [RequireComponent(typeof(CharacterController))]
    public class FaRCharacterController : MonoBehaviour
    {
        public static FaRCharacterController instance;

        private CharacterController _controller;
        
        [SerializeField] private Camera cam;
        [SerializeField] private float movementSpeed = 20f;
        [SerializeField] public float lookSensitivity = 1.0f;
        
        private float _xRotation = 0f;

        [Header("Parámetros de movimiento")]
        private Vector3 _velocity;
        public float gravity = -9.81f;
        private bool _grounded;
        public float jumpSpeed;

        public bool doZoom;
        public bool doCrouch;
        public bool doWalk;

        [Header("Parámetros de zoom")]
        public float zoomFOV = 35.0f;
        public float zoomSpeed = 9f;
        private float _targetFOV;
        private float _baseFOV;

        [Header("Parámetros de agacharse")]
        private float _initHeight;
        [SerializeField] private float crouchHeight;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            _controller = GetComponent<CharacterController>();
            if (cam == null) cam = GetComponentInChildren<Camera>();
            _initHeight = _controller.height;
            Cursor.lockState = CursorLockMode.Locked;
            //Cursor.visible = false;
            SetBaseFOV(cam.fieldOfView);
        }

        private void OnEnable()
        {
            GameInput.playerInputActions.Enable();
        }

        

        private void Update()
        {
            DoMovement();
            DoLooking();
            DoZoom();
            DoCrouch();
            DoWalk();
        }

        public void DoLooking()
        {
            Vector2 looking = GetPlayerLook();
            float lookX = looking.x * lookSensitivity * Time.deltaTime;
            float lookY = looking.y * lookSensitivity * Time.deltaTime;

            _xRotation -= lookY;
            _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);
            
            cam.transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
            
            transform.Rotate(Vector3.up * lookX);
        }

        private void DoMovement()
        {
            _grounded = _controller.isGrounded;
            if (_grounded)
            {
                _velocity.y = -2f;

                if (GameInput.playerInputActions.Player.Jump.WasPressedThisFrame())
                {
                    _velocity.y = jumpSpeed;
                }
            }
            else
            {
                _velocity.y += gravity * Time.deltaTime;
            }

            Vector2 movement = GetPlayerMovement();
            Vector3 move = transform.right * movement.x + transform.forward * movement.y;
            _controller.Move(move * movementSpeed * Time.deltaTime);

            _velocity.y += gravity * Time.deltaTime;
            _controller.Move(_velocity * Time.deltaTime);
        }

        private void DoZoom()
        {
            if (!doZoom) return;
            
            /*
            if (GameInput.playerInputActions.Player.Zoom.ReadValue<float>() > 0)
            {
                targetFOV = zoomFOV;
            }
            else
            {
                targetFOV = baseFOV;
            }

            UpdateZoom();
            */
        }

        private void DoCrouch()
        {
            if (!doCrouch) return;
        
            if (GameInput.playerInputActions.Player.Crouch.ReadValue<float>() > 0)
            {
                _controller.height = crouchHeight;
            }
            else
            {
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.up), 2.0f, -1))
                {
                    _controller.height = crouchHeight;
                }
                else
                {
                    _controller.height = _initHeight;
                }
            }
        }

        private void DoWalk()
        {
            if (!doWalk) return;

            if (GameInput.playerInputActions.Player.Sprint.WasPressedThisFrame())
            {
                movementSpeed = 6.0f;
            }
            else if (GameInput.playerInputActions.Player.Sprint.WasReleasedThisFrame())
            {
                movementSpeed = 20f;
            }
        }

        private void UpdateZoom()
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, _targetFOV, zoomSpeed * Time.deltaTime);
        }

        public void SetBaseFOV(float fov)
        {
            _baseFOV = fov;
        }


        public Vector2 GetPlayerMovement()
        {
            return GameInput.playerInputActions.Player.Movement.ReadValue<Vector2>();
        }

        public Vector2 GetPlayerLook()
        {
            return GameInput.playerInputActions.Player.Look.ReadValue<Vector2>();
        }
    }
}