using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TankMovement : MonoBehaviour
{
    [Header("Movement")]
    public float movementSpeed = 8f;

    public float rotationSpeed = 14f;
    public float acceleration = 30f;

    private float _runTimer = 0f;

    public float NormalizedSpeed
    {
        get
        {
            float speed = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z).magnitude;

            if (speed < 0.1f) return 0f; // Idle
            return Mathf.Lerp(0.4f, 1f, speed / movementSpeed);
        }
    }

    [Header("Jump")]
    public float jumpForce = 10f;

    [Header("Dash")]
    public float dashForce = 10f;

    public ParticleSystem dashParticlesL;
    public ParticleSystem dashParticlesR;

    [Header("Ground Check")]
    public LayerMask groundLayer;

    public Transform groundCheck;
    public Vector3 groundCheckSize;

    [Header("Collision Pre Detection")]
    public LayerMask checkLayer;

    public Transform checkPoint;
    public float checkSize = .3f;
    [Range(0, 3)] public float checkDistance = 2f;

    public Rigidbody rb;
    public bool isGrounded { get; private set; }
    public bool isWalled { get; private set; }
    public bool isWalk { get; private set; }

    public UnityEvent OnJumpSound;

    private float _horizontal;
    private float _vertical;
    private Vector3 _direction;
    private Vector3 _desiredVelocity;

    public void SetInput(float horizontal, float vertical)
    {
        _horizontal = horizontal;
        _vertical = vertical;
    }

    public void Tick()
    {
        GroundCheck();
        CollisionPreDetection();
        Movement();
    }

    private void GroundCheck()
    {
        Collider[] buffer = new Collider[1];
        Physics.OverlapBoxNonAlloc(groundCheck.position, groundCheckSize / 2f,
            buffer, transform.rotation, groundLayer);
        isGrounded = buffer[0] != null;
    }

    private void CollisionPreDetection()
    {
        Collider[] buffer = new Collider[1];
        Physics.OverlapSphereNonAlloc(checkPoint.position, checkSize, buffer, checkLayer);
        isWalled = buffer[0] != null;
    }

    private void Movement()
    {
        _direction.Set(_horizontal, 0f, _vertical);
        _direction.Normalize();
        _desiredVelocity = _direction * movementSpeed;

        Vector3 temp = transform.position + _direction * checkDistance;
        temp.y = checkPoint.position.y;
        checkPoint.position = temp;

        if ((_horizontal != 0 || _vertical != 0) && !isWalled && isGrounded)
        {
            isWalk = true;
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(_desiredVelocity), rotationSpeed * Time.deltaTime);
        }
        if ((_horizontal == 0 && _vertical == 0) || isWalled)
        {
            _desiredVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            isWalk = false;
        }
        if (isGrounded)
            rb.linearVelocity = Vector3.MoveTowards(rb.linearVelocity,
                _desiredVelocity, acceleration * Time.deltaTime);
    }

    public void Jump()
    {
        if (!isGrounded) return;
        StartCoroutine(JumpRoutine());
    }

    private IEnumerator JumpRoutine()
    {
        yield return new WaitForSeconds(0.1f);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        OnJumpSound?.Invoke();
    }

    public void Dash()
    {
        dashParticlesL.Play();
        dashParticlesR.Play();
        StartCoroutine(DashRoutine());
    }

    private IEnumerator DashRoutine()
    {
        yield return new WaitForSeconds(0.8f);
        rb.AddForce(transform.forward * dashForce, ForceMode.Impulse);
    }
}
