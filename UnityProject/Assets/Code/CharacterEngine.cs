using UnityEngine;
using System.Collections;

public class ObjectEngine : MonoBehaviour {
	public float speed = 3f;
	public Vector3 dir = Vector3.right;
	public GameObject sprite;

	// Update is called once per frame
	void FixedUpdate () {
		if (_firstBounded) {
			if (_reverse) {
				_reverse = false;
				dir = -dir;
				transform.Rotate(Vector3.up, 180);
			}

			if (_grounded) {
				Vector3 v = dir * Time.deltaTime*speed;
				rigidbody.MovePosition(transform.position+v);
			}
		
		}
	}

	public virtual void OnCollisionEnter(Collision c)
	{
		if (c.gameObject.tag=="Floor") {
			Vector3 point = transform.InverseTransformPoint(c.contacts[0].point);
			if (point.y<=0.1) {
				//Debug.Log ("enter floor: " + _floorCount);
				_grounded = true;
				_firstBounded = true;
				sprite.SendMessage("Run");
			}
		}
	}
	
	protected bool _firstBounded = false;
	protected bool _grounded = false;
	protected bool _reverse = false;

}

public class CharacterEngine : ObjectEngine {
	
	public float jumpSpeedFactor = 0.75f;
	public Vector3 jumpForce = new Vector3(0,10,0);
	public float turnDuration = 1f;
	public float notMovingTimer = 0.5f;
	
	// Use this for initialization
	void Start ()
	{
		_turnDistance = (TowerManager.Instance().towerLevelWidth/2) * Mathf.Tan(Mathf.Deg2Rad * (90 / 2.0f));
		_turnDistance += TowerManager.Instance().cellWidth/2;
		_turnSqrDistance = _turnDistance * _turnDistance;
	}
	
	void Update()
	{
		if (_grounded && (Input.GetKeyDown(KeyCode.Space) || Input.GetAxis("CharacterJump")==1)) {
			_jump = true;
		}
		
		bool wasReverseBtn = _reverseBtn;
		
		if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetAxis("CharacterReverse")==1) {
			_reverseBtn = true;
		} else {
			_reverseBtn = false;
		}
		
		if (Input.GetMouseButtonDown(0)) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit = new RaycastHit();
			if (collider.Raycast(ray,out hit,100)) {
				_reverseBtn = true;
			} else {
				_jump = true;
			}
		}

		if (_grounded && !_reverse && !wasReverseBtn && _reverseBtn) {
			_reverse = true;
		}	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
		if (!_turning && transform.position==_previousPos) {
			_notMoving += Time.fixedDeltaTime;
			if (_notMoving>notMovingTimer) {
				_notMoving = 0;
				_jumpCollision = true;
				_reverse = true;
				_jumpCollisionForce = -dir;
			}
		} else {
			_notMoving = 0;
		}
		
		if (!_turning && !_turningPreviousFrame) {
			Vector3 faceCenter = Vector3.Cross(Vector3.up, dir);
			Vector3 fromCenter = transform.position;
			fromCenter.y = 0.0f;
			float dot = Vector3.Dot(faceCenter, fromCenter);
			if (dot < 0.0f) {
				faceCenter = -faceCenter;
			}	
			faceCenter *= TowerManager.Instance().towerLevelWidth/2 + TowerManager.Instance().cellWidth/2;
			float sqrMagnitude = (fromCenter - faceCenter).sqrMagnitude;
			if (sqrMagnitude >= _turnSqrDistance) {
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
			
		} else if (_firstBounded && !_turning) {
			if (_reverse) {
				_reverse = false;
				dir = -dir;
				transform.Rotate(Vector3.up, 180);
			}

			if (_grounded) {
				Vector3 v = dir * Time.deltaTime*speed;
				rigidbody.MovePosition(transform.position+v);
			} else if (_jumpCollision) {
				_jumpCollision = false;
				rigidbody.AddForce(_jumpCollisionForce,ForceMode.Impulse);
			} else {
				Vector3 v = dir * Time.deltaTime*speed*jumpSpeedFactor;
				rigidbody.MovePosition(transform.position+v);
			}
		
			if (_jump) {
				_grounded = false;
				_jumping = true;
				sprite.SendMessage("JumpBegin");
				_jump = false;
				rigidbody.AddForce(jumpForce,ForceMode.Impulse);
			}
		}
		
		_previousPos = transform.position;
	}
	
	public override void OnCollisionEnter(Collision c)
	{
		if (c.gameObject.tag=="Floor") {
			Vector3 point = transform.InverseTransformPoint(c.contacts[0].point);
			if (point.y<=0.1) {
				_floorCount++;
				//Debug.Log ("enter floor: " + _floorCount);
				_grounded = true;
				_firstBounded = true;
				if (_jumping) {
					_jumping = false;
					sprite.SendMessage("JumpEnd");
				} else {
					sprite.SendMessage("Run");
				}
			} else {
				float dot = Vector3.Dot(transform.up,c.contacts[0].normal);
				//Debug.Log ("dot="+dot);
				if (dot==0) {
					_jumpCollision = true;
					_reverse = true;
					_jumpCollisionForce = c.contacts[0].normal;
				} else {
				}
			}
		} else if (c.gameObject.tag=="Ennemi") {
			_jumpCollision = true;
			_reverse = true;
			_jumpCollisionForce = c.contacts[0].normal;
		}
	}
	
	void OnCollisionExit(Collision c)
	{
		if (c.gameObject.tag=="Floor") {
			if (_floorCount>0) {
				_floorCount--;
				//Debug.Log ("leave floor: " + _floorCount);
				if (_floorCount==0) {
					_grounded = false;
					sprite.SendMessage("Stop");
				}
			}
		}
	}
	
	void Turn(Vector3 faceCenter)
	{
		_turning = true;
		_turnElapsed = 0;
		
		// compute angle
		Vector3 cross = Vector3.Cross(transform.position, dir);
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
		rigidbody.MovePosition(rigidbody.position + dir*0.1f);
	
		// tell the camera to rotate
		TowerCamera.Instance().Rotate(sign);
	}

	private bool _jump = false;
	private bool _jumping = false;
	private bool _jumpCollision = false;
	private bool _reverseBtn = false;
	private int _floorCount = 0;
	private float _turnSqrDistance = 0;
	private float _turnDistance = 0;
	private bool _turning = false;
	private bool _turningPreviousFrame = false;
	private float _turnElapsed = 0;
	private float _turnAngle = 0;
	private Vector3 _jumpCollisionForce;
	private Vector3 _previousPos = Vector3.zero;
	private float _notMoving = 0;
	
}
