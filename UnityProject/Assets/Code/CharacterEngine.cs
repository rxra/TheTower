using UnityEngine;
using System.Collections;

public class CharacterEngine : MonoBehaviour {
	
	public float speed = 2.5f;
	public Vector3 dir = Vector3.right;
	public float turnDuration = 1f;
	
	// Use this for initialization
	void Start ()
	{
		_turnDistance = (TowerManager.Instance().towerLevelWidth/2) * Mathf.Tan(Mathf.Deg2Rad * (90 / 2.0f));
		_turnDistance += TowerManager.Instance().cellWidth/2;
		_turnSqrDistance = _turnDistance * _turnDistance;
				
		renderer.material.color = Color.blue;
		Debug.Log ("distance: " + _turnDistance + " " + _turnSqrDistance);
	}
	
	void Update()
	{
		Debug.DrawRay(transform.position,transform.right);
		Debug.DrawRay(transform.position,transform.forward,Color.red);
		
		if (_grounded && (Input.GetKeyDown(KeyCode.Space) || Input.GetAxis("CharacterJump")==1)) {
			_jump = true;
		}
		
		bool wasReverseBtn = _reverseBtn;
		
		if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetAxis("CharacterReverse")==1) {
			_reverseBtn = true;
		} else {
			_reverseBtn = false;
		}
		if (_grounded && !_reverse && !wasReverseBtn && _reverseBtn) {
			_reverse = true;
		}	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
		if (!_turning && !_turningPreviousFrame) {
			Vector3 faceCenter = Vector3.Cross(Vector3.up, dir);
			Vector3 fromCenter = transform.position;
			fromCenter.y = 0.0f;
			if (Vector3.Dot(faceCenter, fromCenter) < 0.0f) {
				faceCenter = -faceCenter;
			}	
			faceCenter *= TowerManager.Instance().towerLevelWidth/2 + TowerManager.Instance().cellWidth/2;
			if ((fromCenter - faceCenter).sqrMagnitude >= _turnSqrDistance) {
				Turn(faceCenter);	
			}	
		}
		
		if (_turningPreviousFrame) {
			_turningPreviousFrame = false;
		}
		
		if (_turning) {
			float dt = Time.deltaTime;	
			_turnElapsed += dt;
			if (_turnElapsed > turnDuration) {
				dt -= (_turnElapsed - turnDuration);
				_turning = false;
				_turningPreviousFrame = true;
			}
			transform.Rotate(Vector3.up, _turnAngle * dt / turnDuration);
			
			return;
		}

		if (_firstBounded && !_turning) {
			if (_reverse) {
				_reverse = false;
				speed = -speed;
			}

			if (_grounded) {
				Vector3 v = dir * Time.deltaTime*speed;
				rigidbody.MovePosition(transform.position+v);
			} else if (_jumpCollision) {
				_jumpCollision = false;
				rigidbody.AddForce(_jumpCollisionForce,ForceMode.Impulse);
			} else {
				Vector3 v = dir * Time.deltaTime*speed*0.75f;
				rigidbody.MovePosition(transform.position+v);
			}
		
			if (_jump) {
				_grounded = false;
				renderer.material.color = Color.blue;
				_jump = false;
				rigidbody.AddForce(0,10f,0,ForceMode.Impulse);
			}
		}
	}
	
	void OnCollisionEnter(Collision c)
	{
		if (c.gameObject.tag=="Floor") {
			Vector3 point = transform.InverseTransformPoint(c.contacts[0].point);
			if (point.y<-0.4f) {
				_floorCount++;
				Debug.Log ("enter floor: " + _floorCount);
				_grounded = true;
				renderer.material.color = Color.red;
				_firstBounded = true;
			} else {
				_jumpCollision = true;
				speed = -speed;
				_jumpCollisionForce = c.contacts[0].normal;
			}
		}
	}
	
	void OnCollisionExit(Collision c)
	{
		if (c.gameObject.tag=="Floor") {
			if (_floorCount>0) {
				_floorCount--;
				Debug.Log ("leave floor: " + _floorCount);
				if (_floorCount==0) {
					_grounded = false;
					renderer.material.color = Color.blue;
				}
			}
		}
	}
	
	void Turn(Vector3 faceCenter)
	{
		_turning = true;
		_turnElapsed = 0;
		
		// compute angle
		Vector3 cross = Vector3.Cross(transform.position, transform.forward);
		float sign = Mathf.Sign(cross.y);
		_turnAngle = 90 * sign;	
			
		// adjust character position
		/*Vector3 pos = faceCenter + dir * _turnDistance;
		pos.y = transform.position.y;
		rigidbody.position = pos;*/
		 
		// rotate dir and velocity
		Quaternion fullQuat = Quaternion.AngleAxis(_turnAngle, Vector3.up);
		dir = fullQuat * dir;
		rigidbody.velocity = fullQuat * rigidbody.velocity;
		rigidbody.MovePosition(rigidbody.position + transform.forward*0.1f);
	
		// tell the camera to rotate
		TowerCamera.Instance().Rotate(sign);
	}

	//private bool _dir = true;
	private bool _jump = false;
	private bool _jumpCollision = false;
	private bool _reverse = false;
	private bool _reverseBtn = false;
	private bool _grounded = false;
	private int _floorCount = 0;
	private bool _firstBounded = false;
	private float _turnSqrDistance = 0;
	private float _turnDistance = 0;
	private bool _turning = false;
	private bool _turningPreviousFrame = false;
	private float _turnElapsed = 0;
	private float _turnAngle = 0;
	private Vector3 _jumpCollisionForce;
}
