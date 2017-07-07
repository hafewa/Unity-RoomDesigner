using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class XRayCamera : MonoBehaviour {

	public Transform target;
	public Camera camera;

	public Material sliceMaterial;
	public Dictionary<Renderer, Material[]> materialMap = new Dictionary<Renderer, Material[]> ();

	public GameObject box;

	public Collider[] colliders;
	private Plane[] planes;

	private Vector3 normal1;
	private Vector3 position1;
	private Vector3 normal2;
	private Vector3 position2;
	private Vector3 normal3;
	private Vector3 position3;
	private Vector3 normal4;
	private Vector3 position4;
	private Vector3 normal5;
	private Vector3 position5;
	private Vector3 normal6;
	private Vector3 position6;
	private float dot1;
	private float dot2;
	private float dot3;
	private float dot4;
	private float dot5;
	private float dot6;

	private bool runInEditor = false;

	private void UpdateShaderProperties(Material mat)
	{

		Vector3 exts = box.GetComponent<Collider> ().bounds.extents;

		position1 = box.transform.position + Vector3.left*exts.x;
		position2 = box.transform.position - Vector3.left*exts.x;
		position3 = box.transform.position + Vector3.up*exts.y;
		position4 = box.transform.position - Vector3.up*exts.y;
		position5 = box.transform.position + Vector3.forward*exts.z;
		position6 = box.transform.position - Vector3.forward*exts.z;

		normal1 = box.transform.TransformVector(Vector3.left);
		normal2 = box.transform.TransformVector(-Vector3.left);
		normal3 = box.transform.TransformVector(Vector3.up);
		normal4 = box.transform.TransformVector(Vector3.down);
		normal5 = box.transform.TransformVector(Vector3.forward);
		normal6 = box.transform.TransformVector(Vector3.back);

		mat.SetVector("_Plane1Normal", normal1);
		mat.SetVector("_Plane1Position", position1);
		mat.SetVector("_Plane2Normal", normal2);
		mat.SetVector("_Plane2Position", position2);
		mat.SetVector("_Plane3Normal", normal3);
		mat.SetVector("_Plane3Position", position3);
		mat.SetVector("_Plane4Normal", normal4);
		mat.SetVector("_Plane4Position", position4);
		mat.SetVector("_Plane5Normal", normal5);
		mat.SetVector("_Plane5Position", position5);
		mat.SetVector("_Plane6Normal", normal6);
		mat.SetVector("_Plane6Position", position6);

		dot1 = Vector3.Dot (position1, normal1);
		dot2 = Vector3.Dot (position2, normal2);
		dot3 = Vector3.Dot (position3, normal3);
		dot4 = Vector3.Dot (position4, normal4);
		dot5 = Vector3.Dot (position5, normal5);
		dot5 = Vector3.Dot (position6, normal6);
	}

	void Start() {
		if (!Application.isPlaying)
			return;
		
		Down ();
		Up ();
	}

	void OnDestroy(){
		Down ();
	}

	private void Up(){
		if (materialMap.Count > 0) {
			print ("Is already up");
			return;
		}
		materialMap.Clear ();

		colliders = target.GetComponentsInChildren<Collider> ();
		Renderer[] renderers = target.GetComponentsInChildren<Renderer> ();
		foreach (Renderer r in renderers) {
			Material[] mats = r.sharedMaterials;
			materialMap [r] = mats;
			mats = new Material[mats.Length];
			for (int i = 0; i < mats.Length; i++) {
				mats [i] = new Material(r.sharedMaterials[i]);
				mats [i].shader = sliceMaterial.shader;
				mats [i].SetColor ("_CrossColor", sliceMaterial.GetColor ("_CrossColor"));
			}
			r.sharedMaterials = mats;

		}
	}

	private void Down(){
		if (materialMap.Count <= 0)
			return;
		
		colliders = target.GetComponentsInChildren<Collider> ();
		Renderer[] renderers = target.GetComponentsInChildren<Renderer> ();
		foreach (Renderer r in renderers) {
			r.sharedMaterials = materialMap [r];
		}

		materialMap.Clear ();
	}

	void Update() {
		
		if (!Application.isPlaying &&!runInEditor)
			return;
		
		planes = GeometryUtility.CalculateFrustumPlanes(camera);

		foreach (Collider c in colliders) {
			if (GeometryUtility.TestPlanesAABB (planes, c.bounds)) {
				Renderer r = c.gameObject.GetComponent<Renderer> ();
				r.enabled = true;

				foreach(Material m in r.sharedMaterials){
					UpdateShaderProperties (m);
				}
			}else{
				c.gameObject.GetComponent<Renderer> ().enabled = false;
			}
		}
	}

	public void SetRunInEditor(bool b){
		if (b != runInEditor) {
			runInEditor = b;
			if (b) {
				Up ();
			}else{
				Down ();
			}
		}
	}
}