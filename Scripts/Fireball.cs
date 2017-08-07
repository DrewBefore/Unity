using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour {

    public GameObject Explosion;
    public WarlockJ player;
    private CapsuleCollider2D collider;
    private float timer = .1f;

	// Use this for initialization
	void Start () {
        collider = GetComponent<CapsuleCollider2D>();
        collider.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
        timer -= Time.deltaTime;
        if (timer <= 0) {
            collider.enabled = true;
        }
	}

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Player") {
            GameObject explosion = Instantiate(Explosion, transform.position, Quaternion.identity) as GameObject;
            Destroy(explosion, 1.0f);
            GameObject p = collision.gameObject;
            player = p.GetComponent<WarlockJ>();
            player.updateHealth(10f);
            player.hit();
            Destroy(this.gameObject);

            //Push the player
            var magnitude = 300;
            var force = transform.position - collision.transform.position;
            force = -force.normalized;
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(force * magnitude);

        } else if (collision.tag == "Spell") {
            print("fbcollision");
            GameObject explosion = Instantiate(Explosion, transform.position, Quaternion.identity) as GameObject;
            Destroy(explosion, 1.0f);
            Destroy(this.gameObject);
        }
    }
}
