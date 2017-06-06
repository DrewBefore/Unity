using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    protected float speed = 15.0f;
    protected float padding = 1f;
    protected float xmin;
    protected float xmax;
    public GameObject Projectile;
    protected float projectileSpeed = 5;
    protected float firingRate = 2f;
    protected int health = 100;
    protected int maxHealth = 100;
    public GameObject healthBar;

    float fireCooldown = .5f;
    float fireCooldownLeft = 0;

	// Use this for initialization
	void Start () {
        float distance = transform.position.z - Camera.main.transform.position.z;
        Vector3 leftmost = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, distance));
        Vector3 rightmost = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, distance));
        xmin = leftmost.x + padding;
        xmax = rightmost.x - padding;
    }

    public void updateHealth(int amount) {
        int oldHP = health;
        health = health - amount;
        print("Using the update health method, Health now = " + health);
        setHealthBar(health);
        float healthScale = (float) health / maxHealth;
        setHealthBar(healthScale);
    }

    public void setHealthBar(float myHealth) {
        print("setting healthbar");
        healthBar.transform.localScale = new Vector3(Mathf.Clamp(myHealth, 0f, 1f), healthBar.transform.localScale.y);
    }

    public int getHealth() {
        return health;
    }

    protected virtual void fire() {
        Vector3 location = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        GameObject beam = Instantiate(Projectile, location, Quaternion.identity) as GameObject;
        beam.GetComponent<Rigidbody2D>().velocity = new Vector3(0, projectileSpeed, 0);
    }

    // Update is called once per frame
    protected virtual void Update () {
        fireCooldownLeft -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Space) && fireCooldownLeft <= 0) {
            fireCooldownLeft = fireCooldown;
            fire();
            //InvokeRepeating("fire", 0.00001f, firingRate);
        }
        if (Input.GetKeyUp(KeyCode.Space)) {
            CancelInvoke("fire");
        }
        if (Input.GetKey(KeyCode.LeftArrow) && (transform.position.x > xmin)) {
            transform.position += Vector3.left * speed * Time.deltaTime;
        } else if (Input.GetKey(KeyCode.RightArrow) && (transform.position.x < xmax)) {
            transform.position += Vector3.right * speed * Time.deltaTime;
        } else if (Input.GetKey(KeyCode.UpArrow)){
            transform.position += Vector3.up * speed * Time.deltaTime;
        } else if (Input.GetKey(KeyCode.DownArrow)) {
            transform.position += Vector3.down * speed * Time.deltaTime;
        }

            // restric the player to the gamespace
            // float newX = Mathf.Clamp(transform.position.x, xmin, xmax);
            // transform.position = new Vector3(newX, transform.position.y, transform.position.z);
        }

  /*  void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Spell") {
            health -= 10;
            print("player " + health);
            Destroy(collision.gameObject);
        }
    }
    */
}
