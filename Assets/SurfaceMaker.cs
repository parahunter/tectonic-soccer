using UnityEngine;
using System.Collections;

public class SurfaceMaker : MonoBehaviour {

	public float levelOfDetail = 1.0f;
	public float length = 100.0f;
	private float width = 50.0f;
	public float aspectRatio = 0.5f;
	private int widthVerts = 3;
	public int lengthVerts = 6;
	Mesh pitchSurface;

	private void InitialiseVerts()
	{
		float incrementL = length / (lengthVerts - 1);
		float incrementW = width / (widthVerts - 1);
		float incrementU = 1.0f / (lengthVerts - 1);
		float incrementV = 1.0f / (widthVerts - 1);
		float halfLength = length * 0.5f;
		float halfWidth = halfLength * aspectRatio;

		Vector3[] verts = new Vector3[widthVerts * lengthVerts];
		Vector3[] normals = new Vector3[widthVerts * lengthVerts];
		Vector2[] uv = new Vector2[widthVerts * lengthVerts];

		int index = 0;
		for (int l = 0; l < lengthVerts; ++l) {
			for (int w = 0; w < widthVerts; ++w) {
				index = (w * lengthVerts) + l;
				if(index >= (widthVerts * lengthVerts))
				{
					Debug.Log(l);
					Debug.Log(w);
				}
				verts[index].x = ((l * incrementL) - halfLength);
				verts[index].y = 0.0f; verts[index].z = ((w * incrementW) - halfWidth);
				normals[index].x = 0.0f; normals[index].y = 1.0f; normals[index].x = 0.0f;
				uv[index].x = (l * incrementU); uv[index].y = (w * incrementV);
			}
		}
		pitchSurface.vertices = verts;
		pitchSurface.normals = normals;
		pitchSurface.uv = uv;

		int[] indices = new int[(widthVerts - 1) * (lengthVerts - 1) * 6];
		IList indicesList = new ArrayList ();
		int index1, index2, index3;
		for (int x = 0; x < lengthVerts -1; ++x) {
			for (int y = 0; y < widthVerts -1; ++y) {
				index1 = ((y * lengthVerts) + x);
				index2 = index1 + lengthVerts;
				index3 = index2 + 1;
				indicesList.Add(index1);
				indicesList.Add(index2);
				indicesList.Add(index3);
				indicesList.Add(index1);
				indicesList.Add(index3);
				indicesList.Add(index1 + 1);
			}
		}
		Debug.Log (indices.Length);
		Debug.Log (indicesList.Count);
		for (int i = 0; i < indices.Length; ++i)
			indices [i] = (int)indicesList [i];

		pitchSurface.triangles = indices;
	}

	private void InitialisePitch()
	{
		if (lengthVerts < 1) { lengthVerts = 1; }
		widthVerts = (int)(lengthVerts * aspectRatio);

		if(widthVerts%2 == 1) { widthVerts+=1; }
		if(lengthVerts%2 == 1) { lengthVerts+=1; }

		if (length < 1.0f) { length = 1.0f; }
		width = length * aspectRatio; 

		pitchSurface.vertices = new Vector3[widthVerts * lengthVerts];
		pitchSurface.normals = new Vector3[widthVerts * lengthVerts];
		pitchSurface.uv = new Vector2[widthVerts * lengthVerts];

		InitialiseVerts ();
	}

	// Use this for initialization
	void Start () {
		pitchSurface = new Mesh ();
		InitialisePitch ();

		MeshCollider meshCollider = this.GetComponent ("MeshCollider") as MeshCollider;
		meshCollider.sharedMesh = pitchSurface;

		MeshFilter meshFilter = this.GetComponent ("MeshFilter") as MeshFilter;
		meshFilter.sharedMesh = pitchSurface;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
