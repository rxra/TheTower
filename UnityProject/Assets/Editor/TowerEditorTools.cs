using UnityEngine;
using UnityEditor;
using System.Collections;

public class TowerEditorTools
{
	
	public static Mesh BuildCubeMesh (int nbCellX, int nbCellY, int nbCellZ, float sizeX, float sizeY, float sizeZ)
	{
		Mesh m = new Mesh ();
 
		int xCount2 = nbCellX + 1;
		int yCount2 = nbCellY + 1;
		int zCount2 = nbCellZ + 1;
		int numTriangles = nbCellX * nbCellZ * 6 * 2 + nbCellX * nbCellY * 6 * 2 + nbCellZ * nbCellY * 6 * 2;
		int numVertices = (2 * xCount2 * zCount2) + (2 * xCount2 * yCount2) + (2 * yCount2 * zCount2);
 
		Vector3[] vertices = new Vector3[numVertices];
		Vector2[] uvs = new Vector2[numVertices];
		int[] triangles = new int[numTriangles];
 
		float uvFactorX = 1.0f / nbCellX;
		float uvFactorY = 1.0f / nbCellY;
		float uvFactorZ = 1.0f / nbCellZ;
		float scaleX = sizeX / nbCellX;
		float scaleY = sizeY / nbCellY;
		float scaleZ = sizeZ / nbCellZ;
			
		// bottom
		int vindex = 0;
		for (float z = 0.0f; z < zCount2; z++) {
			for (float x = 0.0f; x < xCount2; x++) {
				vertices [vindex] = new Vector3 (x * scaleX - sizeX / 2f, 0.0F, z * scaleZ - sizeZ / 2f);
				uvs [vindex++] = new Vector2 (x * uvFactorX, z * uvFactorZ);
			}
		}
 
		int tindex = 0;
		for (int z = 0; z < nbCellZ; z++) {
			for (int x = 0; x < nbCellX; x++) {
				triangles [tindex++] = (z * xCount2) + x;
				triangles [tindex++] = (z * xCount2) + x + 1;
				triangles [tindex++] = ((z + 1) * xCount2) + x;
 
				triangles [tindex++] = ((z + 1) * xCount2) + x;
				triangles [tindex++] = (z * xCount2) + x + 1;
				triangles [tindex++] = ((z + 1) * xCount2) + x + 1;
			}
		}
		int bottomVIndex = vindex;
 
		// top
		for (float z = 0.0f; z < zCount2; z++) {
			for (float x = 0.0f; x < xCount2; x++) {
				vertices [vindex] = new Vector3 (x * scaleX - sizeX / 2f, sizeY, z * scaleZ - sizeZ / 2f);
				uvs [vindex++] = new Vector2 (x * uvFactorX, z * uvFactorZ);
			}
		}
 
		for (int z = 0; z < nbCellZ; z++) {
			for (int x = 0; x < nbCellX; x++) {
				triangles [tindex++] = bottomVIndex + (z * xCount2) + x;
				triangles [tindex++] = bottomVIndex + ((z + 1) * xCount2) + x;
				triangles [tindex++] = bottomVIndex + (z * xCount2) + x + 1;
 
				triangles [tindex++] = bottomVIndex + ((z + 1) * xCount2) + x;
				triangles [tindex++] = bottomVIndex + ((z + 1) * xCount2) + x + 1;
				triangles [tindex++] = bottomVIndex + (z * xCount2) + x + 1;
			}
		}
		int topVIndex = vindex;
 
		// front
		for (float y = 0.0f; y < yCount2; y++) {
			for (float x = 0.0f; x < xCount2; x++) {
				vertices [vindex] = new Vector3 (x * scaleX - sizeX / 2f, y * scaleY, -sizeZ / 2f);
				uvs [vindex++] = new Vector2 (x * uvFactorX, y * uvFactorY);
			}
		}
 
		for (int y = 0; y < nbCellY; y++) {
			for (int x = 0; x < nbCellX; x++) {
				triangles [tindex++] = topVIndex + (y * xCount2) + x;
				triangles [tindex++] = topVIndex + ((y + 1) * xCount2) + x;
				triangles [tindex++] = topVIndex + (y * xCount2) + x + 1;
 
				triangles [tindex++] = topVIndex + ((y + 1) * xCount2) + x;
				triangles [tindex++] = topVIndex + ((y + 1) * xCount2) + x + 1;
				triangles [tindex++] = topVIndex + (y * xCount2) + x + 1;
			}
		}
		int frontVIndex = vindex;
			
		// back
		for (float y = 0.0f; y < yCount2; y++) {
			for (float x = 0.0f; x < xCount2; x++) {
				vertices [vindex] = new Vector3 (x * scaleX - sizeX / 2f, y * scaleY, sizeZ / 2f);
				uvs [vindex++] = new Vector2 (x * uvFactorX, y * uvFactorY);
			}
		}
 
		for (int y = 0; y < nbCellY; y++) {
			for (int x = 0; x < nbCellX; x++) {
				triangles [tindex++] = frontVIndex + (y * xCount2) + x;
				triangles [tindex++] = frontVIndex + (y * xCount2) + x + 1;
				triangles [tindex++] = frontVIndex + ((y + 1) * xCount2) + x;
 
				triangles [tindex++] = frontVIndex + ((y + 1) * xCount2) + x;
				triangles [tindex++] = frontVIndex + (y * xCount2) + x + 1;
				triangles [tindex++] = frontVIndex + ((y + 1) * xCount2) + x + 1;
			}
		}
		int backVIndex = vindex;
			
		// left
		for (float y = 0.0f; y < yCount2; y++) {
			for (float z = 0.0f; z < zCount2; z++) {
				vertices [vindex] = new Vector3 (-sizeX / 2f, y * scaleY, z * scaleZ - sizeZ / 2f);
				uvs [vindex++] = new Vector2 (z * uvFactorZ, y * uvFactorY);
			}
		}
 
		for (int y = 0; y < nbCellY; y++) {
			for (int z = 0; z < nbCellZ; z++) {
				triangles [tindex++] = backVIndex + (y * zCount2) + z;
				triangles [tindex++] = backVIndex + (y * zCount2) + z + 1;
				triangles [tindex++] = backVIndex + ((y + 1) * zCount2) + z;
 
				triangles [tindex++] = backVIndex + ((y + 1) * zCount2) + z;
				triangles [tindex++] = backVIndex + (y * zCount2) + z + 1;
				triangles [tindex++] = backVIndex + ((y + 1) * zCount2) + z + 1;
			}
		}
		int leftVIndex = vindex;
			
		// right
		for (float y = 0.0f; y < yCount2; y++) {
			for (float z = 0.0f; z < zCount2; z++) {
				vertices [vindex] = new Vector3 (sizeX / 2f, y * scaleY, z * scaleZ - sizeZ / 2f);
				uvs [vindex++] = new Vector2 (z * uvFactorZ, y * uvFactorY);
			}
		}
 
		for (int y = 0; y < nbCellY; y++) {
			for (int z = 0; z < nbCellZ; z++) {
				triangles [tindex++] = leftVIndex + (y * zCount2) + z;
				triangles [tindex++] = leftVIndex + ((y + 1) * zCount2) + z;
				triangles [tindex++] = leftVIndex + (y * zCount2) + z + 1;
 
				triangles [tindex++] = leftVIndex + ((y + 1) * zCount2) + z;
				triangles [tindex++] = leftVIndex + ((y + 1) * zCount2) + z + 1;
				triangles [tindex++] = leftVIndex + (y * zCount2) + z + 1;
			}
		}
			
		m.vertices = vertices;
		m.uv = uvs;
		m.triangles = triangles;
		m.RecalculateNormals ();
		
		return m;
	}
	
}
