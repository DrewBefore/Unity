using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPU : MonoBehaviour {
    public float cpuspeed = 15.0f;
    public float cpupadding = 1f;
    float cpuxmin;
    float cpuxmax;
    public GameObject cpuProjectile;
    public float cpuprojectileSpeed;
    public float cpufiringRate = .2f;
    private int health = 100;

    // Use this for initialization
    void Start() {
        float distance = transform.position.z - Camera.main.transform.position.z;
        Vector3 leftmost = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, distance));
        Vector3 rightmost = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, distance));
        cpuxmin = leftmost.x + cpupadding;
        cpuxmax = rightmost.x - cpupadding;

    }

    public void updateHealth(int amount) {
        health = health - amount;
        print(health);
    }

    void cpufire() {
        Vector3 location = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
        GameObject beam = Instantiate(cpuProjectile, location, Quaternion.identity) as GameObject;
        beam.GetComponent<Rigidbody2D>().velocity = new Vector3(0, -cpuprojectileSpeed, 0);
    }

    // Update is called once per frame
    void Update() {
        float value = Random.value;
        if (Input.GetKeyDown(KeyCode.Space)) {
            InvokeRepeating("cpufire", 0.0000000001f, cpufiringRate);
        }
        if (Input.GetKeyUp(KeyCode.Space)) {
            CancelInvoke("cpufire");
        }
        if (value < .07 && value > .06) {
            cpufire();
        }
        if (value < .5) {
            transform.position += Vector3.left * cpuspeed * Time.deltaTime;
        } else if (value > .5) {
            transform.position += Vector3.right * cpuspeed * Time.deltaTime;
        }

        // restric the player to the gamespace
        float newX = Mathf.Clamp(transform.position.x, cpuxmin, cpuxmax);
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }
}
