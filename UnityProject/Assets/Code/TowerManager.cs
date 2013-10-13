using UnityEngine;
using System.Collections;

public class TowerManager : MonoBehaviour {
	
	public float cellWidth = 1;
	public float cellHeight = 2;
	public float plateformThicknessFactor = 5;
	public float towerLevelWidth = 0;
	public float towerLevelHeight = 0;
	
	public static TowerManager Instance()
	{
		return s_Singleton;
	}
	
	void Awake() {
		Debug.Log ("TowerManager: Awake");
		s_Singleton = this;
	}
	
	private static TowerManager s_Singleton = null;
	
}
