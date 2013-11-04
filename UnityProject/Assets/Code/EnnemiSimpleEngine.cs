using UnityEngine;
using System.Collections;

public class EnnemiSimpleEngine : ObjectEngine {
	
	void Update()
	{
		if (_floor!=null) {
			Ray ray = new Ray(
				new Vector3(
					transform.position.x + dir.x*collider.bounds.extents.x,
					transform.position.y + collider.bounds.extents.y,
					transform.position.z),
				-transform.up
			);

			RaycastHit hit = new RaycastHit();
			if (!_floor.Raycast(ray,out hit,collider.bounds.extents.y*2)) {
				_reverse = true;
			}
		}
	}
	
	public override void OnCollisionEnter(Collision c)
	{
		base.OnCollisionEnter(c);
		if (c.gameObject.tag=="Floor") {
			_floor = c.collider;
		}
	}
	
	void OnCollisionExit(Collision c)
	{
		if (c.gameObject.tag=="Floor") {
			_reverse = true;
		}
	}
	
	protected Collider _floor = null;
}
