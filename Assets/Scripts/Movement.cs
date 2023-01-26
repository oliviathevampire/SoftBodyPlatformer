using System;
using UnityEngine;

[Obsolete]
public class Movement : MonoBehaviour {
    private Rigidbody2D _rb;
    private Collider2D _bc;
    
    //Horizontal movement
    private float _moveX;
    
    public LayerMask groundLayer;
    
    [Header("Gravity Values")]
    public float fallGravity;
    public float defaultGravity;
    
    [Header("Speeds")]
    public float walkSpeed;
    public float runSpeed;
    public float jumpForce;
    
    [Header("Coyote")]
    public float coyoteTime;
    private float _currentCoyoteTime;

    [Header("Air Jumps")]
    public int airJumpAmount;
    private int _currentAirJumps;
    private float _fallTimer;

    private void Start() {
        _rb = GetComponent<Rigidbody2D>();
        _bc = GetComponent<Collider2D>();
        
        _currentAirJumps = airJumpAmount;
        _currentCoyoteTime = coyoteTime;
    }

    private void Update() {
        var velocity = _rb.velocity;
        
        //Jump
        if (Input.GetKeyDown(KeyCode.Space)) Jump();
        
        //Cancel jump
        if (Input.GetKeyUp(KeyCode.Space) && _rb.velocity.y > 0)
            _rb.velocity = new Vector2(_rb.velocity.x, velocity.y / 2);

        if (IsGrounded()) {
            _currentAirJumps = airJumpAmount;
            _currentCoyoteTime = coyoteTime;
            _rb.gravityScale = defaultGravity;
        } else if (_currentCoyoteTime > 0) {
            _currentCoyoteTime -= Time.deltaTime;
        }

        if (_rb.velocity.y < -0.1 && !IsGrounded()) {
            _rb.gravityScale = fallGravity;
        }
    }

    private void FixedUpdate() {
        var velocity = _rb.velocity;
        
        _moveX = Input.GetAxis("Horizontal");
        //Movement
        _rb.velocity = Input.GetKeyDown(KeyCode.LeftShift) 
            ? new Vector2(_moveX * runSpeed, velocity.y) 
            : new Vector2(_moveX * walkSpeed, velocity.y);
    }

    private void Jump() {
        var velocity = _rb.velocity;
        
        if (_currentAirJumps <= 0 && _currentCoyoteTime <= 0) return;
            
        if (IsGrounded() || _currentCoyoteTime > 0) _rb.velocity = new Vector2(velocity.x, jumpForce);
        else {
            if (_currentAirJumps > 0) {
                _currentAirJumps--;
                _rb.velocity = new Vector2(velocity.x, jumpForce);
            }
        }

        _currentCoyoteTime = 0;
    }

    private bool IsGrounded() {
        var bounds = _bc.bounds;
        return Physics2D.BoxCast(bounds.center, bounds.size, 0f, Vector2.down, 0.1f, groundLayer);
    }
}