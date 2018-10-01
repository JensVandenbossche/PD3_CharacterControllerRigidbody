using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]

public class CharacterControllerBehaviour : MonoBehaviour {

    [SerializeField]
    private Transform _absoluteTransform;

    [SerializeField]
    private float _acceleration = 3;

    [SerializeField]
    private float _jumpHeight = 1;

    private CapsuleCollider _capsuleCollider;
    private Rigidbody _rigidbody;

    private bool _jump;
    private Vector3 _movement;

    bool isGrounded = false;
    private Vector3 _velocity;

	void Start ()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _capsuleCollider = GetComponent<CapsuleCollider>();

#if DEBUG
        Assert.IsNotNull(_rigidbody, "CharacterControllerBehaviour needs a Rigidbody in order to work");
        Assert.IsNotNull(_capsuleCollider, "CharacterControllerBehaviour needs a CapsuleCollider in order to work");
#endif
    }
	
	void Update ()
    {
		if (Input.GetButtonDown("Jump"))
            _jump = true;

        _movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
	}

    private void FixedUpdate()
    {
        _isGroundedNeedsUpdate = true;
        ApplyGround();
        ApplyJump();
        ApplyMovement();
        _rigidbody.MovePosition(_rigidbody.position + _velocity * Time.deltaTime);
    }

    private void ApplyGround()
    {
        if (_isGrounded)
        {
            _velocity -= Vector3.Project(_velocity, Physics.gravity);
        }
    }

    private void ApplyMovement()
    {
        if (/*IsGrounded()*/ _isGrounded)
        {
            Vector3 xzForward = Vector3.Scale(_absoluteTransform.forward, new Vector3(1, 0, 1));
            Quaternion relativeRotation = Quaternion.LookRotation(xzForward);
            Vector3 relativeMovement = relativeRotation * _movement;

            _velocity += relativeMovement * _acceleration * Time.fixedDeltaTime;
        }
    }

    private void ApplyJump()
    {
        if (_isGrounded && _jump)
        {
            _velocity.y += Mathf.Sqrt(2 * Physics.gravity.magnitude * _jumpHeight);
            _jump = false;
        }
    }

    private HashSet<GameObject> _ground = new HashSet<GameObject>();

    private void OnCollisionEnter(Collision collision)
    {
        foreach(ContactPoint contact in collision.contacts)
        {
            if (Vector3.Dot(contact.normal, Vector3.up) > 0.5)
            {
                _isGrounded = true;
                _ground.Add(collision.gameObject);
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (_ground.Remove(collision.gameObject))
        {
            _isGrounded = (_ground.Count > 0);
        }
    }

    bool _isGrounded = false;
    bool _isGroundedNeedsUpdate = true;

    //private bool IsGrounded()
    //{
    //    //raycast // spherecast // collider

    //    if (_isGroundedNeedsUpdate)
    //    {
    //        Vector3 rayCenter = _capsuleCollider.center;
    //        float rayLength = _capsuleCollider.bounds.extents.y + 0.1f;
    //        float sphereRadius = _capsuleCollider.radius * 0.9f;

    //        RaycastHit hitInfo;
    //        bool isGrounded = Physics.SphereCast(rayCenter, sphereRadius, Vector3.down, out hitInfo, rayLength);

    //        _isGrounded = isGrounded && Vector3.Dot(hitInfo.normal, Vector3.up) > 0.5; //check angle between ray and surface
    //        _isGroundedNeedsUpdate = false;
    //    }
    //    return _isGrounded;
    //}
}
