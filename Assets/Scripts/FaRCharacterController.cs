using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FaRUtils.FPSController
{
    [RequireComponent(typeof(CharacterController))]
    public class FaRCharacterController : MonoBehaviour
    {
        private CharacterController controller;
        
        [SerializeField] private Camera cam;
        [SerializeField] private float movementSpeed = 2.0f;
        [SerializeField] public float lookSensitivity = 1.0f;
        
        private float xRotation = 0f;

        [Header("Parámetros de movimiento")]
        private Vector3 velocity;
        public float gravity = -9.81f;
        private bool grounded;
        public float jumpSpeed;

        public bool doZoom;
        public bool doCrouch;

        [Header("Parámetros de zoom")]
        public float zoomFOV = 35.0f;
        public float zoomSpeed = 9f;
        private float targetFOV;
        private float baseFOV;

        [Header("Parámetros de agacharse")]
        private float initHeight;
        [SerializeField] private float crouchHeight;


        private void Start()
        {
            controller = GetComponent<CharacterController>();
            if (cam == null) cam = GetComponentInChildren<Camera>();
            initHeight = controller.height;
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
        }

        private void DoLooking()
        {
            Vector2 looking = GetPlayerLook();
            float lookX = looking.x * lookSensitivity * Time.deltaTime;
            float lookY = looking.y * lookSensitivity * Time.deltaTime;

            xRotation -= lookY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            
            cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            
            transform.Rotate(Vector3.up * lookX);
        }

        private void DoMovement()
        {
            grounded = controller.isGrounded;
            if (grounded)
            {
                velocity.y = -2f;

                if (GameInput.playerInputActions.Player.Jump.WasPressedThisFrame())
                {
                    velocity.y = jumpSpeed;
                }
            }
            else
            {
                velocity.y += gravity * Time.deltaTime;
            }

            Vector2 movement = GetPlayerMovement();
            Vector3 move = transform.right * movement.x + transform.forward * movement.y;
            controller.Move(move * movementSpeed * Time.deltaTime);

            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
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
                controller.height = crouchHeight;
            }
            else
            {
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.up), 2.0f, -1))
                {
                    controller.height = crouchHeight;
                }
                else
                {
                    controller.height = initHeight;
                }
            }
        }

        private void UpdateZoom()
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, zoomSpeed * Time.deltaTime);
        }

        public void SetBaseFOV(float fov)
        {
            baseFOV = fov;
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