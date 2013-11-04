using UnityEngine;
using System.Collections;

public class CharacterSprite : MonoBehaviour 
{
	public Vector2 spriteSize;
	public Vector2 frameSize;
	public Material mat;
	
	public int runStartFrame;
	public int runEndFrame;
	public float runHeight;
	public float runDuration;
	
	public int jumpFrame;
	
	private Mesh mesh;
	private Texture texture;
	private Vector2 uvSize;
	
	private int currentFrame;
	
	private bool run;
	private int runInc = 1;
	private float runFrameDuration;
	private float runElapsed;
	
	public Vector2 GetSpriteSize()
	{
		return spriteSize;	
	}
	
	// Use this for initialization
	void Awake() 
	{
		texture = mat.mainTexture;
		
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		if (meshFilter==null) {
			meshFilter = (MeshFilter)gameObject.AddComponent(typeof(MeshFilter));
		}
		if (GetComponent<MeshRenderer>()==null) {
	        gameObject.AddComponent(typeof(MeshRenderer));
		}
 
		uvSize = new Vector2((float)(((float)frameSize.x) / ((float)texture.width)), frameSize.y / (float)texture.height); 
		
		// create the mesh
		mesh = new Mesh();
		
		int vertexCount 	= 8;
		Vector3[] vertices 	= new Vector3[vertexCount];
		Vector3[] normals	= new Vector3[vertexCount];
		Vector2[] uvs		= new Vector2[vertexCount];
		int[] triangles		= new int[(vertexCount / 2) * 3];
		
		// Front Face
		vertices[0] 		= new Vector3(0.0f, 0.0f, 0.0f);
		vertices[1] 		= new Vector3(spriteSize.x, 0.0f, 0.0f);
		vertices[2] 		= new Vector3(spriteSize.x, spriteSize.y, 0.0f);
		vertices[3] 		= new Vector3(0.0f, spriteSize.y, 0.0f);
		
		normals[0] = normals[1] = normals[2] = normals[3] = -Vector3.forward;
		
		uvs[0]				= new Vector2(0.0f, 0.0f);
		uvs[1]				= new Vector2(1.0f, 0.0f);
		uvs[2]				= new Vector2(1.0f, uvSize.y);
		uvs[3]				= new Vector2(0.0f, uvSize.y);
		
		triangles[0] = 0; triangles[1] = 3; triangles[2] = 2;
		triangles[3] = 0; triangles[4] = 2;	triangles[5] = 1;
		
		// Back Face
		float deltaZ		= 0.01f;
		vertices[4] 		= new Vector3(vertices[1].x, vertices[1].y, deltaZ);
		vertices[5] 		= new Vector3(vertices[0].x, vertices[0].y, deltaZ);
		vertices[6] 		= new Vector3(vertices[3].x, vertices[3].y, deltaZ);
		vertices[7] 		= new Vector3(vertices[2].x, vertices[2].y, deltaZ);
		
		normals[4] = normals[5] = normals[6] = normals[7] = Vector3.forward;
		
		uvs[4]				= uvs[1];
		uvs[5]				= uvs[0];
		uvs[6]				= uvs[3];
		uvs[7]				= uvs[2];
		
		triangles[6] = 4; triangles[7] = 7; triangles[8] = 6;
		triangles[9] = 4; triangles[10] = 6; triangles[11] = 5;
		
		mesh.vertices 		= vertices;
		mesh.normals		= normals;
		mesh.uv				= uvs;
		mesh.triangles		= triangles;
		
		meshFilter.sharedMesh = mesh;
        mesh.RecalculateBounds();
		renderer.material = mat;
		
		transform.localPosition = new Vector3(-spriteSize.x / 2, 0.0f, 0.0f);
		//transform.parent.SendMessage("SetSprite", this);
		
		currentFrame = 0;
		SetFrame(currentFrame);		
	}
	
	public void Stop()
	{
		run = false;
	}
	
	public void Run()
	{
		run = true;
		runInc = 1;
		runElapsed = 0.0f;
		SetFrame(runStartFrame);
	}

	public void JumpBegin()
	{
		SetFrame(jumpFrame);
		Stop();
	}
	
	public void JumpEnd()
	{
		SetFrame(runStartFrame);
		Run();
	}
	
	void SetFrame(int iFrame)
	{
		currentFrame = iFrame;
			
		Vector2[]uvs = mesh.uv;
		
		uvs[0].x = uvSize.x * iFrame;
		uvs[1].x = uvs[0].x + uvSize.x;
		uvs[2].x = uvs[1].x;
		uvs[3].x = uvs[0].x;
		uvs[4].x = uvs[1].x;
		uvs[5].x = uvs[0].x;
		uvs[6].x = uvs[3].x;
		uvs[7].x = uvs[2].x;
		
		mesh.uv = uvs;
		
		/*if (run) {
			Vector3 pos = transform.localPosition;
			pos.y = runHeight * (iFrame - runStartFrame);
			transform.localPosition = pos;
		}*/
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (run) {
			float dt = Time.deltaTime;
			runElapsed += dt;
			
			runFrameDuration = runDuration / (2 * (runEndFrame - runStartFrame) + 1);
			if (runElapsed > runFrameDuration) {
				runElapsed -= runFrameDuration;				

				int nextFrame = currentFrame + runInc;
				if ((nextFrame == runStartFrame) || (nextFrame == runEndFrame)) {
					runInc = -runInc;	
				}
				SetFrame(nextFrame);
			}
		}
	}
}



	