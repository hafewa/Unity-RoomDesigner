using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor (typeof(FloorMesh))]
public class FloorMeshEditorExtention : Editor {

	FloorMesh floor = null;

	int[] triangles;
	Vector3[] vertices;

	private bool manipulating = false;
	private Vector3 basePoint;

	void OnEnable ()
	{
		//Character コンポーネントを取得
		floor = (FloorMesh) target;
		triangles = floor.GetComponent<MeshFilter> ().sharedMesh.triangles;
		vertices = floor.GetComponent<MeshFilter> ().sharedMesh.vertices;

		basePoint = vertices [0];

		SceneView.onSceneGUIDelegate += SceneGUI;
	}

	void SceneGUI(SceneView sceneView)
	{
//		Event cur = Event.current;
//
//		if (Event.current.type == EventType.MouseDown)
//		{
//			Debug.Log ("MouseDown");
//		}
//		if (Event.current.type == EventType.MouseMove) {
//			Debug.Log ("MouseMove");
//
//		}
//		if (Event.current.type == EventType.MouseMove) {
//			Debug.Log ("MouseUp");
//
//		}
	}

	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();

		EditorGUILayout.Vector3Field ("Base", basePoint);

		bool m = manipulating;
		manipulating = EditorGUILayout.Toggle ("Manpulate", manipulating);
		if (manipulating != m) {
			SceneView.RepaintAll();
		}

	}

	Vector3 s;

	void OnSceneGUI ()
	{

		if (!manipulating)
			return;

		Tools.current = Tool.None;


		float size = 0.25f;

		Transform transform = floor.transform;
		Vector3 v = transform.TransformPoint (basePoint);

//		Handles.color = Color.red;
//		Handles.ArrowCap(0,
//			v,
//			transform.rotation * Quaternion.LookRotation(new Vector3(1, 0, 0)),
//			size);
//
//		Handles.color = Color.green;
//		Handles.ArrowCap(0,
//			v,
//			transform.rotation * Quaternion.LookRotation(new Vector3(0, 1, 0)),
//			size);
//		Handles.color = Color.blue;
//
//		Handles.ArrowCap(0,
//			v,
//			transform.rotation * Quaternion.LookRotation(new Vector3(0, 0, 1)),
//			size);


//		Vector3 vv = Handles.PositionHandle (v, transform.rotation);
//		Vector3 vDiff = vv - v;
//
//		transform.position += vDiff;
//		Debug.Log (transform.position);

		Camera sceneCamera = SceneView.lastActiveSceneView.camera;

		float dist = Vector3.Distance (sceneCamera.transform.position, v);


		s = Vector3.one;
		Vector3 ss = Handles.ScaleHandle (s, v, transform.rotation, dist * size);

		Vector3 v1 = (vertices [1] - vertices [0]) * ss.x;
		vertices [1] = vertices [0] + v1;
		Debug.Log (ss);
		Debug.Log (v1);

		ss = s;

		Mesh msh = floor.GetComponent<MeshFilter> ().sharedMesh;
		msh.vertices = vertices;
		msh.RecalculateNormals ();
		msh.RecalculateBounds ();
	
	}

	void OnDisable(){
		
	}
}
