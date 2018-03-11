using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PC2dController : MonoBehaviour {

    private PC2dStateManager stateManager;
	// Use this for initialization
	void Start () {
        stateManager = new PC2dStateManager();
        stateManager.OnStateChange += statechanged;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space)) {
            stateManager.ChangeStateTo(UserState.Rising);
        }
	}

    //监听状态改变
    void statechanged(UserState fromState, UserState toState) {
        Debug.Log(fromState.ToString() + " => " + toState.ToString());
    }
}
