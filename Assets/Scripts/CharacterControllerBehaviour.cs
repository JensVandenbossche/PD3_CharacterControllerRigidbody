﻿using System.Collections;
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

    private CapsuleCollider _capsuleCollider;
    private Rigidbody _rigidbody;

    private bool _jump = false;
    private Vector3 _movement;

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
        _rigidbody.MovePosition(_rigidbody.position + _velocity * Time.deltaTime);
    }

    private void ApplyMovement()
    {
        if (IsGrounded())
        {
            Vector3 xzForward = Vector3.Scale(_absoluteTransform.forward, new Vector3(1, 0, 1));
            Quaternion relativeRotation = Quaternion.LookRotation(xzForward);
            Vector3 relativeMovement = relativeRotation * _movement;

            _velocity += relativeMovement * _acceleration * Time.fixedDeltaTime;
        }
    }

    private bool IsGrounded()
    {
        Vector3 rayCenter = _capsuleCollider.center;
        float rayLength = _capsuleCollider.bounds.extents.y + 0.1f;

        RaycastHit hitInfo;
        bool isGrounded = Physics.Raycast(rayCenter, Vector3.down, out hitInfo, rayLength);

        return isGrounded && Vector3.Dot(hitInfo.normal, Vector3.up) > 0.5; //check angle between ray and surface
    }
}