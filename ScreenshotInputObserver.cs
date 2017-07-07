using UnityEngine;
using System.Collections;

public class ScreenshotInputObserver : MonoBehaviour {

	public CameraSaveLoad.CameraSaveLoadManager cameraManager;

	// Use this for initialization
	void Start () {
		
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.P)) {
			if(Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftAlt)){
				cameraManager.TakeScreenshot();
			}
		}
	}
}
