using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PitchTectonics : MonoBehaviour {

	public Texture2D tectonicPowerMap;
	public Texture2D tectonicInputMap;
	public Texture2D tectonicDecalMap;
	
	public float levelOfDetail = 1.0f;
	public float length = 100.0f;
	private float width = 50.0f;
	public float aspectRatio = 0.5f;
	public int widthVerts = 3;
	public int lengthVerts = 6;
	public Mesh pitchSurface;

	public bool testing = false;
	public bool debugging = false;
	public float brushPower = 0.3f;
	public float tectonicPower = 5.0f;

	public static PitchTectonics instance
	{
		get;
		private set;
	}

	private float uPerPixel = 0.0f;
	private float vPerPixel = 0.0f;
	int halfDecalSize = 0;

	struct PixelPos
	{
		public int x;
		public int y;
	}

	struct PixelRect
	{
		public int x;
		public int y;
		public int width;
		public int height;
	}

	private PixelPos pixelInput;
	private PixelRect rectPos;
	private PixelRect decalPos;
	private Color[] inputColor;
	private Color[] decalColor;
	
	public Vector3[] verts;
	public Vector3[] normals;
	public Vector2[] uv;
	
	public float GetHeight(Vector2 position)
	{
		position.x /= halfLength;
		position.y /= halfWidth;
	
		int x = (int) ( (position.x + 1) * 0.5f * (lengthVerts - 1) );
		int y = (int) ( (position.y + 1) * 0.5f * (widthVerts - 1) );

		x = Mathf.Clamp(x , 0, lengthVerts - 1);
		y = Mathf.Clamp(y, 0, widthVerts - 1);
				
		return verts[ x + y * lengthVerts].y;	
	}
	
	public Vector3 GetNormal(Vector2 position)
	{
		position.x /= halfLength;
		position.y /= halfWidth;
		
		int x = (int) ( (position.x + 1) * 0.5f * (lengthVerts - 1) );
		int y = (int) ( (position.y + 1) * 0.5f * (widthVerts - 1) );

		x = Mathf.Clamp(x, 0, lengthVerts - 1);
		y = Mathf.Clamp(y, 0, widthVerts - 1);		
						
		return normals[ x + y * lengthVerts];	
	}
	
	float halfLength;
	float halfWidth;
	private void InitialiseVerts()
	{
		float incrementL = length / (lengthVerts - 1);
		float incrementW = width / (widthVerts - 1);
		float incrementU = 1.0f / (lengthVerts - 1);
		float incrementV = 1.0f / (widthVerts - 1);
		halfLength = length * 0.5f;
		halfWidth = halfLength * aspectRatio;
		
		verts = new Vector3[widthVerts * lengthVerts];
		normals = new Vector3[widthVerts * lengthVerts];
		uv = new Vector2[widthVerts * lengthVerts];
		
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
				//verts[index].y = 0.0f; 
				verts[index].z = ((w * incrementW) - halfWidth);
				
				//normals[index].x = 0.0f; 
				normals[index].y = 1.0f; 
				//normals[index].x = 0.0f;
				
				uv[index].x = (l * incrementU); uv[index].y = (w * incrementV);
			}
		}
		pitchSurface.vertices = verts;
		pitchSurface.normals = normals;
		pitchSurface.uv = uv;
		
		int[] indices = new int[(widthVerts - 1) * (lengthVerts - 1) * 6];
		

		int i = 0;
		int index1, index2, index3;
		for (int x = 0; x < lengthVerts -1; ++x) {
			for (int y = 0; y < widthVerts -1; ++y) {
				index1 = ((y * lengthVerts) + x);
				index2 = index1 + lengthVerts;
				index3 = index2 + 1;
				indices[i + 0] = index1;
				indices[i + 1] = index2;
				indices[i + 2] = index3;
				indices[i + 3] = index1;
				indices[i + 4] = index3;
				indices[i + 5] = index1 + 1;
				
				i += 6;
			}
		}
		
		Debug.Log (indices.Length);
		
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

	void Awake()
	{
		instance = this;
	}

	// Use this for initialization
	void Start () 
	{
		pitchSurface = new Mesh ();
		pitchSurface.MarkDynamic();
		
		InitialisePitch ();
		
		MeshCollider meshCollider = this.GetComponent ("MeshCollider") as MeshCollider;
		meshCollider.sharedMesh = pitchSurface;
		
		MeshFilter meshFilter = this.GetComponent ("MeshFilter") as MeshFilter;
		meshFilter.sharedMesh = pitchSurface;

		tectonicInputMap = new Texture2D ( lengthVerts, widthVerts , TextureFormat.ARGB32, false, false);
		
		int totalPixels = lengthVerts * widthVerts;
		Color[] colors = new Color[totalPixels];
		for (int i = 0; i <  totalPixels; ++i) { colors[i].g = colors[i].r = colors[i].b = 0.0f;  colors[i].a = 1.0f; }
		tectonicInputMap.SetPixels (0, 0, lengthVerts, widthVerts, colors);
		tectonicInputMap.Apply ();
		if(testing && debugging)
			renderer.material.mainTexture = tectonicInputMap;
		uPerPixel = 1.0f / tectonicInputMap.width;
		vPerPixel = 1.0f / tectonicInputMap.height;
		halfDecalSize = tectonicDecalMap.width / 2;
	}

	public void AddTectonics(Vector2 uvCoords)
	{
		pixelInput.x = (int)(tectonicInputMap.width * uvCoords.x);
		pixelInput.y = (int)(tectonicInputMap.height * uvCoords.y);
		if (pixelInput.x < 0) { pixelInput.x = 0; }
		if (pixelInput.y < 0) { pixelInput.y = 0; }
		if (pixelInput.x >= tectonicInputMap.width) { pixelInput.x = tectonicInputMap.width - 1; }
		if (pixelInput.y >= tectonicInputMap.height) { pixelInput.y = tectonicInputMap.height - 1; }

		int decalWidth = tectonicDecalMap.width;
		decalPos.x = decalPos.y = 0;
		if (pixelInput.x < halfDecalSize) {
			rectPos.width = decalPos.width = (decalWidth - (halfDecalSize - pixelInput.x));
			decalPos.x = decalWidth - decalPos.width;
		} else if ((tectonicInputMap.width - pixelInput.x) < halfDecalSize) {
			rectPos.width = decalPos.width = (decalWidth - (halfDecalSize - (tectonicInputMap.width - pixelInput.x)));
		} else
			rectPos.width = decalPos.width = decalWidth;
		
		rectPos.x = Mathf.Max (0, pixelInput.x - halfDecalSize);
		decalPos.x = Mathf.Max (0, halfDecalSize - pixelInput.x);
		
		if (pixelInput.y < halfDecalSize) {
			rectPos.height = decalPos.height = (decalWidth - (halfDecalSize - pixelInput.y));
			decalPos.y = decalWidth - decalPos.height;
		} else if ((tectonicInputMap.height - pixelInput.y) < halfDecalSize) {
			rectPos.height = decalPos.height = (decalWidth - (halfDecalSize - (tectonicInputMap.height - pixelInput.y)));
		} else
			rectPos.height = decalPos.height = tectonicDecalMap.width;
		
		rectPos.y = Mathf.Max (0, pixelInput.y - halfDecalSize);
		decalPos.y = Mathf.Max (0, halfDecalSize - pixelInput.y);

		inputColor = tectonicInputMap.GetPixels (rectPos.x, rectPos.y, rectPos.width, rectPos.height);
		decalColor = tectonicDecalMap.GetPixels (decalPos.x, decalPos.y, decalPos.width, decalPos.height);

		if (inputColor.Length != decalColor.Length)
			Debug.Log ("Pixels don't match...");
		else {
			for(int i = 0;i < decalColor.Length; ++i)
			{
				inputColor[i].r += decalColor[i].r * brushPower; // Mathf.Max(inputColor[i].r, decalColor[i].r);
				inputColor[i].g = inputColor[i].r;
				inputColor[i].b = inputColor[i].r;
			}
			tectonicInputMap.SetPixels (rectPos.x, rectPos.y, rectPos.width, rectPos.height, inputColor);
			tectonicInputMap.Apply();
		}
	}

	void SetPitchHeight()
	{

		int totalVerts = widthVerts * lengthVerts;
		int l = lengthVerts - 1;
		int w = widthVerts - 1;
		
		Color[] pixels = tectonicInputMap.GetPixels();
		for(int i = 0; i < totalVerts; ++i)
		{
			Vector2 uvssss = uv[i];
			int x = (int) (  uvssss.x * l) ;
			int y = (int) ( uvssss.y * w ) ; 
			verts[i].y = tectonicPower * pixels[x + y * lengthVerts].r ;// * tectonicPowerMap.GetPixelBilinear(uv[i].x, uv[i].y).r;
		}
		
		pitchSurface.vertices = verts;
		pitchSurface.RecalculateNormals();
		pitchSurface.RecalculateBounds();
		normals = pitchSurface.normals;		
	}

	public float dampening = 80f;
	void SettleTectonics()
	{ 
		Color[] pixels = tectonicInputMap.GetPixels();
	
		float actualDamping = dampening * Time.deltaTime;
		
		for (int i = 0; i < tectonicInputMap.width; ++i) 
		{
			for (int j = 0; j < tectonicInputMap.height; ++j) 
			{
				//if (texel.b > 0.0f)
				{
					pixels[i + j * tectonicInputMap.width].r -= actualDamping;
				}
			}
		}
		
		tectonicInputMap.SetPixels(pixels);
		tectonicInputMap.Apply ();
	}
	
	// Update is called once per frame
	void Update () {
		if (testing) {
			if (Input.GetMouseButtonDown (0)) {
				Vector3 mouse = Input.mousePosition;
				Debug.Log (Input.mousePosition);
				Vector2 coords = new Vector2 (mouse.x / Screen.width, mouse.y / Screen.height);

				AddTectonics (coords);
			}
		}

		Profiler.BeginSample( "set pitch height");
		SetPitchHeight ();
		Profiler.EndSample();

		Profiler.BeginSample("settle");
		SettleTectonics ();
		Profiler.EndSample();
		
	}
}
