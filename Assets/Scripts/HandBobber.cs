using UnityEngine;

namespace FaRUtils.FPSController
{
    public class HandBobber : MonoBehaviour
    {
        [Header("Bobbing Settings - Walk")]
        [SerializeField] private float walkBobbingSpeed = 3f;
        [SerializeField] private float walkBobbingAmountX = 0.05f;
        [SerializeField] private float walkBobbingAmountY = 0.05f;

        [Header("Bobbing Settings - Run")]
        [SerializeField] private float runBobbingSpeed = 10f;
        [SerializeField] private float runBobbingAmountX = 0.1f;
        [SerializeField] private float runBobbingAmountY = 0.12f;

        [Header("Global Settings")]
        [SerializeField] private float smoothAmount = 10f;

        [Header("References")]
        [SerializeField] private FaRCharacterController controller;

        private float _timer = 0f;
        private Vector3 _initialPosition;

        private void Start()
        {
            _initialPosition = transform.localPosition;
            if (controller == null)
            {
                controller = GetComponentInParent<FaRCharacterController>();
            }
        }

        private void Update()
        {
            float horizontal = controller.GetPlayerMovement().x;
            float vertical = controller.GetPlayerMovement().y;

            Vector3 targetPosition;

            if (Mathf.Abs(horizontal) == 0 && Mathf.Abs(vertical) == 0)
            {
                _timer = Mathf.Lerp(_timer, 0, Time.deltaTime * smoothAmount);
                targetPosition = _initialPosition;
            }
            else
            {
                float currentSpeed = controller.MovementSpeed;
                float walkSpeed = controller.DefaultWalkSpeed;
                float runSpeed = controller.DefaultRunSpeed;
                
                float t = (runSpeed - walkSpeed) > 0 ? Mathf.Clamp01((currentSpeed - walkSpeed) / (runSpeed - walkSpeed)) : 0;

                float currentBobSpeed = Mathf.Lerp(walkBobbingSpeed, runBobbingSpeed, t);
                float currentAmountX = Mathf.Lerp(walkBobbingAmountX, runBobbingAmountX, t);
                float currentAmountY = Mathf.Lerp(walkBobbingAmountY, runBobbingAmountY, t);

                _timer += currentBobSpeed * Time.deltaTime;

                if (_timer > Mathf.PI * 2)
                {
                    _timer -= Mathf.PI * 2;
                }
                
                float xOffset = Mathf.Cos(_timer) * currentAmountX;
                float yOffset = Mathf.Sin(_timer) * Mathf.Cos(_timer) * currentAmountY;

                float totalAxes = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
                
                targetPosition = _initialPosition + new Vector3(xOffset * totalAxes, yOffset * totalAxes, 0);
            }

            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * smoothAmount);
        }
    }
}
