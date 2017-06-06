using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    [SerializeField] WarlockJ player;
	// Use this for initialization
	void Start () {
        player.transform.position = new Vector3(0,0);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
