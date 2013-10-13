using UnityEngine;
using UnityEditor;
using System.Collections;
 
 
public class CreateTowerLevel : ScriptableWizard
{
	public float towerLevelWidth = 24;
	public float towerLevelHeight = 48;
    public string optionalName;
 
    [MenuItem("GameObject/TowerWorld/Tower Level")]
    static void CreateWizard()
    {
		ScriptableWizard.DisplayWizard("Create Tower Level",typeof(CreateTowerLevel));
    }
 
 	void OnWizardUpdate ()
	{
		if (!ComputeTowerManager()) {
			return;
		}
		
		if (tManager.towerLevelWidth!=0) {
			towerLevelWidth = tManager.towerLevelWidth;
		}
		
		if (tManager.towerLevelHeight!=0) {
			towerLevelHeight = tManager.towerLevelHeight;
		}
	}
	
    void OnWizardCreate()
    {
		if (!ComputeTowerManager()) {
			return;
		}
		
        GameObject tlevel = new GameObject();
 
        if (!string.IsNullOrEmpty(optionalName))
            tlevel.name = optionalName;
        else
            tlevel.name = "TowerLevel";
 
        tlevel.transform.position = Vector3.zero;
 
        MeshFilter meshFilter = (MeshFilter)tlevel.AddComponent(typeof(MeshFilter));
        tlevel.AddComponent(typeof(MeshRenderer));
 
        string tlevelAssetName = tlevel.name + towerLevelWidth + "_" + towerLevelHeight + "_" + tManager.cellWidth + "_" + tManager.cellHeight + ".asset";
        Mesh m = (Mesh)AssetDatabase.LoadAssetAtPath("Assets/Editor/" + tlevelAssetName,typeof(Mesh));
 
		float width = towerLevelWidth;
		float length = towerLevelHeight;
		int widthSegments = Mathf.FloorToInt(towerLevelWidth/tManager.cellWidth);
		int lengthSegments = Mathf.FloorToInt(towerLevelHeight/tManager.cellHeight);
		
        if (m == null)
        {
			m = TowerEditorTools.BuildCubeMesh(widthSegments,lengthSegments,widthSegments,width,length,width);
            m.name = tlevel.name;
            AssetDatabase.CreateAsset(m, "Assets/Editor/" + tlevelAssetName);
            AssetDatabase.SaveAssets();
        }
 
        meshFilter.sharedMesh = m;
        m.RecalculateBounds();

		if (tManager.towerLevelWidth==0) {
			tManager.towerLevelWidth = towerLevelWidth;
		}
		
		if (tManager.towerLevelHeight==0) {
			tManager.towerLevelHeight = towerLevelHeight;
		}
	}
	
	private bool ComputeTowerManager()
	{
		if (tManager==null) {
			if (TowerManager.Instance()==null) {
				GameObject towerManager = GameObject.FindGameObjectWithTag("TowerManager");
				if (towerManager!=null) {
					tManager = towerManager.GetComponent<TowerManager>();
				}
			} else {
				tManager = TowerManager.Instance();
			}
		}
		
		if (tManager==null) {
			Debug.LogError("cannot find TowerManger");
			return false;
		}
		
		return true;
	}
	
	private TowerManager tManager = null;
	
}
