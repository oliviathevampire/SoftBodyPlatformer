using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace SoftBody {
    public class BlobMovement : MonoBehaviour {
        [SerializeField] private float walkSpeed;
        [SerializeField] private float sprintSpeed;
        [SerializeField] private float jumpForce;
        [SerializeField] private float gravity;
        [Header("GroundCheck")] 
        [SerializeField] private float groundCheckDistance;
        [SerializeField] private LayerMask groundLayer;

        private PlayerInputs _playerInputs;
        private Rigidbody2D[] _nodesRigidbodyArray;

        private void Awake() {
            _playerInputs = new PlayerInputs();
        }

        private void OnEnable() {
            _playerInputs.Movement.Enable();
            _playerInputs.Movement.Jump.performed += JumpOnPerformed;
        }

        private void OnDisable() {
            _playerInputs.Movement.Disable();
            _playerInputs.Movement.Jump.performed -= JumpOnPerformed;
            
        }

        private void Start() {
            _nodesRigidbodyArray = GetComponentsInChildren<Rigidbody2D>();
        }

        private void Update() {
            float2 direction = new float2(_playerInputs.Movement.LeftRight.ReadValue<float>(), 0);
            float currentSpeed = _playerInputs.Movement.Sprint.IsPressed() ? sprintSpeed : walkSpeed;
            float2 movementVector = direction * currentSpeed * Time.deltaTime;

            AddForceToBlob(movementVector, ForceMode2D.Force);
        }

        private void FixedUpdate() {
            foreach (Rigidbody2D rb in _nodesRigidbodyArray) {
                rb.gravityScale = gravity;
            }
        }

        private void JumpOnPerformed(InputAction.CallbackContext obj) {
            if (!IsGrounded()) return;
            
            AddForceToBlob(new float2(0, 1) * jumpForce, ForceMode2D.Impulse);
        }
        
        private void AddForceToBlob(float2 movementVector, ForceMode2D forceMode) {
            foreach (Rigidbody2D rb in _nodesRigidbodyArray) {
                rb.AddForce(movementVector, forceMode);
            }
        }

        private bool IsGrounded() {
            return Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
        }
    }
}
