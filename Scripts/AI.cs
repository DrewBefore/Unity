using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AI : WarlockJ {
    private CapsuleCollider2D myCollider;

	// Use this for initialization
	void Start () {
        float distance = transform.position.z - Camera.main.transform.position.z;
        Vector3 leftmost = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, distance));
        Vector3 rightmost = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, distance));
        xmin = leftmost.x + padding;
        xmax = rightmost.x - padding;
    }

    [Command]
    protected override void Cmdfire() {
        Vector3 location = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
        GameObject beam = Instantiate(Projectile, location, Quaternion.identity) as GameObject;
        beam.GetComponent<Rigidbody2D>().velocity = new Vector3(0, -projectileSpeed, 0);
    }

    // Update is called once per frame
    void Update() {
        float value = Random.value;
        if (value < .07 && value > .06) {
            Cmdfire();
        }
        if (value < .5) {
            transform.position += Vector3.left * speed * Time.deltaTime;
        } else if (value > .5) {
            transform.position += Vector3.right * speed * Time.deltaTime;
        }

        // restric the player to the gamespace
        float newX = Mathf.Clamp(transform.position.x, xmin, xmax);
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }

    protected override void FixedUpdate() {

    }
}
