using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smite : MonoBehaviour {
    List<WarlockJ> players = new List<WarlockJ>();
    public WarlockJ player;
    [SerializeField] GameObject spell;
    // Use this for initialization
    void Start () {
        Destroy(gameObject, 1.0f);

    }

    // Update is called once per frame
    void Update () {
		
	}

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Player") {
            GameObject p = collision.gameObject;
            player = p.GetComponent<WarlockJ>();
            players.Add(player);

            var myX = gameObject.transform.position.x;
            var myY = gameObject.transform.position.y;
            var otherX = collision.transform.position.x;
            var otherY = collision.transform.position.y;

            var x = Mathf.Abs(myX - otherX);
            var y = Mathf.Abs(myY - otherY);
            var difference = x + y;

            //PushBack Enemy Warlock
            if (difference > .1) {
                var magnitude = 300;
                var force = transform.position - collision.transform.position;
                force = -force.normalized;
                collision.gameObject.GetComponent<Rigidbody2D>().AddForce(force * magnitude);
            }
            player.updateHealth(10);
        }
    }

}
