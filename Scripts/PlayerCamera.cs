using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour {
    [SerializeField] WarlockJ player;
    private Transform position;

	// Use this for initialization
	void Start () {
        position = player.transform;
	}
	
	// Update is called once per frame
	void Update () {
        delay();
	}

    IEnumerator delay(){
        yield return new WaitForSeconds(6f);
        position = player.transform;
    }
}
