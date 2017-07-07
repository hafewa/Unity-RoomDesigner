using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class FillCubeEditorExtention : MonoBehaviour {

	private static GameObject CreateSphere(Vector3 pos){
		GameObject go = GameObject.CreatePrimitive (PrimitiveType.Sphere);
		go.transform.position = pos;
		go.transform.localScale = Vector3.one * 1f;
		return go;
	}

	private static bool IsLined(Vector2 v1, Vector2 v2, Vector2 v3){
		float ang = 175f;

		float a1 = Vector2.Angle (v2 - v1, v3 - v1);
		float a2 = Vector2.Angle (v3 - v2, v1 - v2);
		float a3 = Vector2.Angle (v2 - v3, v1 - v3);
		if (a1 > ang || a2 > ang || a3 > ang) {
			return true;
		}

		return false;
	}
	private static bool IsPerpendicular(Vector2 v1, Vector2 v2, Vector2 v3){
		
		float a1 = Vector2.Angle (v2 - v1, v3 - v1);
		float a2 = Vector2.Angle (v3 - v2, v1 - v2);
		float a3 = Vector2.Angle (v2 - v3, v1 - v3);
		if (Mathf.Abs(90-a1) < 5 || Mathf.Abs(90-a2) < 5 || Mathf.Abs(90-a3) < 5) {
			return true;
		}

		return false;
	}

	[MenuItem("Tools/FloorMesh/CreateAtSelection")]
	static void CreateFillCube ()
	{
		GameObject g = Selection.activeGameObject;
		if (g == null)
			return;

		GameObject go = new GameObject(g.name + "-FloorMesh");
		go.AddComponent<FloorMesh> ();
		go.transform.position = g.transform.position;
		go.transform.localScale = g.transform.localScale;
		go.transform.rotation = g.transform.rotation;
		
		Vector3 cen = g.transform.position;
		List<Vector2> list = new List<Vector2> ();
		for(float i=0; i<360f; i+=1f){	
			Vector3 dir = Quaternion.Euler(new Vector3 (0, i, 0)) * Vector3.forward;
			RaycastHit hit = new RaycastHit ();
			bool b= Physics.Raycast (cen, dir, out hit);
			if (b) {
				list.Add (new Vector2(hit.point.x, hit.point.z));
//				CreateSphere (hit.point);
			}
		}

		int idx = 0;
		int counter = 0;
		List<Vector2> list2 = new List<Vector2> (list);

		List<HashSet<Vector2>> lines = new List<HashSet<Vector2>>();
		while(list2.Count > 0 && counter < list2.Count){
			counter++;

			if (list2.Count <= idx+2)
				break;

			Vector2 v1 = list2 [idx];
			Vector2 v2 = list2 [idx + 1];
			Vector2 v3 = list2 [idx + 2];

			if (IsLined (v1, v2, v3)) {
				Color c = new Color (Random.Range (0.5f, 1f), Random.Range (0.5f, 1f), Random.Range (0.5f, 1f));
				Material m = new Material (Shader.Find("Standard"));
				m.color = c;

				HashSet<Vector2> s = new HashSet<Vector2> ();
				s.Add (v1);
				s.Add (v2);
				s.Add (v3);
				lines.Add (s);

				idx = (idx + 3) % list2.Count;
				int startIdx = idx;
				while (startIdx != (idx + 1) % list2.Count) {
					v3 = list2 [idx];
					if (IsLined (v1, v2, v3)) {
						v1 = v2;
						v2 = v3;
						s.Add (v3);
					}
					idx = (idx + 1) % list2.Count;
				}
				foreach (Vector2 v in s) {
					list2.Remove (v);

//					if (s.Count >= 20) {
//						ggg = CreateSphere (v);
//						ggg.GetComponent<Renderer> ().material = m;
//					} 
				}
				if (s.Count < 20) {
					lines.Remove (s);
				}

				counter = 0;
				idx = 0;
			} else {
				idx++;
			}
		}
		print (list2.Count);
		print (counter);

		lines.Sort ((x, y) => {
			return y.Count - x.Count;
		});

		List<HashSet<Vector2>> res = new List<HashSet<Vector2>> ();

		int cnt = 0;
		foreach (HashSet<Vector2> s in lines) {
			float dist = 0;
			Vector2[] vv = new Vector2[2];
			Vector2[] vecs = s.ToArray ();
			for (int i = 0; i < vecs.Length - 1; i++) {
				for (int j = 0; j < vecs.Length; j++) {
					float d = Vector2.Distance (vecs [i], vecs [j]);
					if (d > dist) {
						dist = d;
						vv [0] = vecs [i];
						vv [1] = vecs [j];
					}
				}
			}

			print (""+s.Count);

			s.Clear ();
			s.Add (vv [0]);s.Add (vv [1]);
			res.Add(s);

			if(++cnt >= 3)
				break;
		}
		lines = res;

		HashSet<Vector2> points = new HashSet<Vector2> ();

		foreach (HashSet<Vector2> s in lines) {
			Vector2[] vv = s.ToArray ();
			print (""+s.Count+":"+Vector3.Distance(vv[0], vv[1]));
			foreach (Vector2 v in s) {
				print("  "+list.IndexOf(v));
				points.Add (v);
			}
		}

		Vector2[] ps = points.ToArray ();
		System.Array.Sort (ps, (x, y) => {
			return list.IndexOf(x) - list.IndexOf(y);
		});

		list.Clear ();
		list.AddRange (ps);

		float maxDist = 0;
		int[] farests = new int[3];
		for (int i = 0; i < list.Count - 2; i++) {
			for (int j = 1; j < list.Count - 1; j++) {
				for (int k = 2; k < list.Count; k++) {
					if (IsPerpendicular (list [i], list [j], list [k])) {

						float dist = Vector2.Distance (list [i], list [j])
						           + Vector2.Distance (list [j], list [k])
						           + Vector2.Distance (list [k], list [i]);
						if (maxDist < dist) {
							maxDist = dist;
							farests [0] = i;
							farests [1] = j;
							farests [2] = k;
						}
					}
				}
			}
		}

		Vector2[] lastList = new Vector2[4];

		Vector2 vv1 = list[farests[0]];
		Vector2 vv2 = list[farests[1]];
		Vector2 vv3 = list[farests[2]];
		lastList [0] = vv1;
		lastList [1] = vv2;
		lastList [2] = vv3;

		float d1 = Vector3.Distance(vv1, vv2);
		float d2 = Vector3.Distance(vv2, vv3);
		float d3 = Vector3.Distance(vv3, vv1);

		if (d1 > d2 && d1 > d3) {
			lastList [3] = (vv3 + (vv2 - vv3) + (vv1 - vv3));
//			lastList [3] = (vv3);
		} else if (d2 > d1 && d2 > d3) {
			lastList [3] = (vv1 + (vv3 - vv1) + (vv2 - vv1));
//			lastList [3] = (vv1);
		} else if (d3 > d1 && d3 > d2) {
			lastList [3] = (vv2 + (vv3 - vv2) + (vv1 - vv2));
//			lastList [3] = (vv2);
		}


//		CreateSphere (lastList[0]);
//		CreateSphere (lastList[1]);
//		CreateSphere (lastList[2]);
//		GameObject gg = CreateSphere (lastList[3]);
//		gg.GetComponent<Renderer>().material.color = Color.red;


		// Use the triangulator to get indices for creating triangles
		Triangulator tr = new Triangulator(lastList);
		int[] indices = tr.Triangulate();

		// Create the Vector3 vertices
		Vector3[] vertices = new Vector3[lastList.Length];
		for (int i=0; i<vertices.Length; i++) {
			vertices[i] = go.transform.InverseTransformPoint(new Vector3(lastList[i].x, cen.y, lastList[i].y));
		}

		// Create the mesh
		Mesh msh = new Mesh();
		msh.vertices = vertices;
		msh.triangles = indices;
		msh.RecalculateNormals();
		msh.RecalculateBounds();
		MeshUtility.Optimize (msh);
		AssetDatabase.CreateAsset (msh, "Assets/"+g.name+"-Mesh");
		AssetDatabase.SaveAssets();

		// Set up game object with mesh;

		go.AddComponent(typeof(MeshRenderer));
		MeshFilter filter = go.AddComponent(typeof(MeshFilter)) as MeshFilter;
		filter.mesh = msh;

			
	}

	static bool IsPointInOABB (Vector3 point, BoxCollider box)
	{
		point = box.transform.InverseTransformPoint( point ) - box.center;

		float halfX = (box.size.x * 0.5f);
		float halfY = (box.size.y * 0.5f);
		float halfZ = (box.size.z * 0.5f);
		if( point.x < halfX && point.x > -halfX && 
			point.y < halfY && point.y > -halfY && 
			point.z < halfZ && point.z > -halfZ )
			return true;
		else
			return false;
	}

	static void CheckBoxIntersection(GameObject go, Transform transform){
		//Get the mesh you want to check
		Mesh mesh = go.GetComponent<MeshFilter>().sharedMesh; 
		Debug.Log (mesh.vertexCount);
		Vector3[] vertices = mesh.vertices;
		int i = 0;
		foreach(Vector3 v in vertices){
			//Raycast from the middle to each vertex
//			Ray ray = new Ray(go.transform.position, transform.TransformPoint(v));
//			RaycastHit hit;
//			GeometryUtility.

//			if (Physics.Raycast(ray, out hit, 100)) 
//				Debug.Log("Intersecting " + hit.collider.name);
		}
	}
}
