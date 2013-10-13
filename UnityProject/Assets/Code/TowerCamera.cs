using UnityEngine;
using System.Collections;

public class TowerCamera : MonoBehaviour {

	public float rotationDuration = 0.1f;
	
	public static TowerCamera Instance()
	{
		return s_Singleton;
	}

	public void Rotate(float iSign)
	{
		camera.orthographic		= false; 
		_rotationAngle			= _rotating ? -_currentAngle : 90 * iSign;
		_realRotationDuration 	= _rotating ? _rotationElapsed : rotationDuration;
		_rotating 				= true;
		_currentAngle			= 0.0f;	
		_rotationElapsed		= 0.0f;
	}

	void Awake() {
		s_Singleton = this;
	}
	
	void Start()
	{
		_rotating = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		float dt = Time.deltaTime;
		
		//Vector3 camPos = transform.position;
		//Vector3 characPos = character.position;
		
		/////////////////////////////////////////////////
		// Check if camera is too far from character
		/*if (Mathf.Abs(characPos.y - camPos.y) > maxDistanceFromCharacter) {
			wantedHeight = characPos.y + maxDistanceFromCharacter;	
		}*/
		
		/////////////////////////////////////////////////
		// Height
		/*float stretch = camPos.y - wantedHeight;
		float acceleration = -heightStiffness * stretch - heightDamping * heightSpeed; 
		heightSpeed += acceleration * dt;
		camPos.y += heightSpeed * dt;
		
		transform.position = camPos;*/
		
		/////////////////////////////////////////////////
		// Rotation
		if (_rotating) {
			_rotationElapsed += dt;
			if (_rotationElapsed > _realRotationDuration) {
				dt -= _rotationElapsed - _realRotationDuration;
				_rotating = false;
				camera.orthographic	= true; 
			}	
			float angle = _rotationAngle * dt / _realRotationDuration;
			_currentAngle += angle;
			transform.RotateAround(Vector3.zero, Vector3.up, angle);
		}
	}

	private static TowerCamera s_Singleton = null;
	
	private bool _rotating = false;
	private float _currentAngle;
	private float _rotationElapsed;
	private float _realRotationDuration;
	private float _rotationAngle;

}
