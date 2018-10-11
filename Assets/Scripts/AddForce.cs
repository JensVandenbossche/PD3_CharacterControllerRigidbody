using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Rigidbody))]

public class AddForce : MonoBehaviour {

    private Rigidbody _rigidbody;
    private bool _jump;

    [SerializeField]
    private float _acceleration;

	void Start ()
    {
        _rigidbody = GetComponent<Rigidbody>();

#if DEBUG
        Assert.IsNotNull(_rigidbody, "AddForce needs a Rigidbody Component in order to work");
#endif
    }
	
	void Update ()
    {
		if (Input.GetButtonDown("Jump"))
        {
            _jump = true;
        }
	}

    private void FixedUpdate()
    {
        if (_jump)
        {
            //Force             || houdt rekening met delta time en massa            
            //Acceleration      || houdt rekening met delta time maar niet met massa
            //Impulse           || houdt geen rekening met delta time maar wel met massa
            //VelocityChange    || houdt geen rekening met delta time en massa

            _rigidbody.AddForce(Vector3.forward * _rigidbody.mass * _acceleration, ForceMode.Impulse);
            _jump = false;
        }
    }
}
