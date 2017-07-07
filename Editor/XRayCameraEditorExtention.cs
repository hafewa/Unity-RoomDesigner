using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

[CustomEditor (typeof(XRayCamera))]
public class XRayCameraEditorExtention : Editor {
	
	private bool runInEditor;

	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();

		runInEditor = GUILayout.Toggle (runInEditor, "Run in editor");
		XRayCamera xrc = target as XRayCamera;
		xrc.SetRunInEditor (runInEditor);

	}

}
