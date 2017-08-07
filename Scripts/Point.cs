using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour {
    private float timer = .8f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        timer -= Time.deltaTime;
		if(Input.GetMouseButtonDown(0)){
			GameObject.Destroy(this.gameObject);
		}
        if (timer <= 0) {
            GameObject.Destroy(this.gameObject);
        }
	}
}
