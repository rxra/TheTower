using UnityEngine;
using UnityEditor;
using System.Collections;
 
public enum TowerSide 
{
	TS_FRONT,
	TS_BACK,
	TS_LEFT,
	TS_RIGHT
}

public enum PlateformType
{
	PT_WIDTH,
	PT_WIDTH1,
	PT_WIDTH2,
	PT_CELL2,
	PT_CELL4,
	PT_CELL6,
	PT_CELL8
}

public class CreateTowerPlateform : ScriptableWizard
{
	public float plateformThicknessFactor = 5;
	public TowerSide towerSide = TowerSide.TS_FRONT;
	public PlateformType type = PlateformType.PT_WIDTH;
    public string optionalName;
 
    [MenuItem("GameObject/TowerWorld/Tower Plateform")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard("Create Tower Plateform",typeof(CreateTowerPlateform));
    }
 
 
    void OnWizardUpdate()
    {
		if (!ComputeTowerManager()) {
			return;
		}

		if (tManager.plateformThicknessFactor!=0) {
			plateformThicknessFactor = tManager.plateformThicknessFactor;
		}
	}
 
 
    void OnWizardCreate()
    {
		if (!ComputeTowerManager()) {
			return;
		}

		GameObject tplateform = new GameObject();
 
        if (!string.IsNullOrEmpty(optionalName))
            tplateform.name = optionalName;
        else
            tplateform.name = "TowerPlateform";
 
		switch(towerSide) {
		case TowerSide.TS_FRONT:
	        tplateform.transform.position = new Vector3(0,0,-(tManager.towerLevelWidth/2+tManager.cellWidth/2));
			break;
		case TowerSide.TS_BACK:
	        tplateform.transform.position = new Vector3(0,0,(tManager.towerLevelWidth/2+tManager.cellWidth/2));
			break;
		case TowerSide.TS_LEFT:
			tplateform.transform.rotation = Quaternion.Euler(0,90,0);
	        tplateform.transform.position = new Vector3(-(tManager.towerLevelWidth/2+tManager.cellWidth/2),0,0);
			break;
		case TowerSide.TS_RIGHT:
			tplateform.transform.rotation = Quaternion.Euler(0,90,0);
	        tplateform.transform.position = new Vector3((tManager.towerLevelWidth/2+tManager.cellWidth/2),0,0);
			break;
		}
			
        MeshFilter meshFilter = (MeshFilter)tplateform.AddComponent(typeof(MeshFilter));
        tplateform.AddComponent(typeof(MeshRenderer));
 
        string tplateformAssetName = tplateform.name + tManager.towerLevelWidth + "_" + tManager.towerLevelHeight + "_" + tManager.cellWidth + "_" + tManager.cellHeight + "_" + plateformThicknessFactor + "_" + type + ".asset";
        Mesh m = (Mesh)AssetDatabase.LoadAssetAtPath("Assets/Editor/" + tplateformAssetName,typeof(Mesh));
 
		float width = tManager.towerLevelWidth;
		int cellWidth = Mathf.FloorToInt(tManager.towerLevelWidth/tManager.cellWidth);
		switch(type) {
		case PlateformType.PT_WIDTH1:
			width += tManager.cellWidth;
			cellWidth += 1;
			break;

		case PlateformType.PT_WIDTH2:
			width += tManager.cellWidth*2;
			cellWidth += 2;
			break;

		case PlateformType.PT_CELL2:
			width = 2*tManager.cellWidth;
			cellWidth = 2;
			break;

		case PlateformType.PT_CELL4:
			width = 4*tManager.cellWidth;
			cellWidth = 4;
			break;

		case PlateformType.PT_CELL6:
			width = 6*tManager.cellWidth;
			cellWidth = 6;
			break;

		case PlateformType.PT_CELL8:
			width = 8*tManager.cellWidth;
			cellWidth = 8;
			break;
		}
		
        if (m == null)
        {
			m = TowerEditorTools.BuildCubeMesh(
				cellWidth,1,1,
				width,tManager.cellHeight/10,tManager.cellWidth);
            m.name = tplateform.name;
            AssetDatabase.CreateAsset(m, "Assets/Editor/" + tplateformAssetName);
            AssetDatabase.SaveAssets();
        }
 
        meshFilter.sharedMesh = m;
        m.RecalculateBounds();

		if (tManager.plateformThicknessFactor==0) {
			tManager.plateformThicknessFactor = plateformThicknessFactor;
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
