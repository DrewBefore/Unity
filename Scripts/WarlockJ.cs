using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.Networking;

public class WarlockJ : NetworkBehaviour {
    [SerializeField] ButtonHandler shootButton;
    [SerializeField] Joystick joystick;
    [SerializeField] GameObject aim;
    [SerializeField] GameObject SmiteSpell;
    [SerializeField] GameObject hud;
    protected Vector3 lookVec;
    protected float speed = 2.0f;
    protected float boostMultiplyer = 2f;
    protected float padding = 1f;
    protected float xmin;
    protected float xmax;
    protected float ymin = -9.05f;
    protected float ymax = 9f;
    public GameObject Projectile;
    protected float projectileSpeed = 5;
    protected float firingRate = 2f;
    protected float health = 100f;
    protected int maxHealth = 100;
    public GameObject healthBar;
    public Rigidbody2D myBody;
    private Vector3 targetPosition;
    private float movementSpeed = 1.5f;
    private float lavaDamage = 6f;
    private bool canWalk = true;

    float fireCooldown = .5f;
    float fireCooldownLeft = 0;

    //Used to differentiate between local player, and other player
    public override void OnStartLocalPlayer() {
        tag = "Player";
        Canvas hud2 = hud.GetComponent<Canvas>();
        hud2.enabled = true;
    }
    // Use this for initialization
    void Start() {
        myBody = GetComponent <Rigidbody2D>();
        float distance = transform.position.z - Camera.main.transform.position.z;
        Vector3 leftmost = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, distance));
        Vector3 rightmost = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, distance));

        xmin = leftmost.x + padding;
        xmax = rightmost.x - padding;

        targetPosition = transform.position;
    }

    public void updateHealth(float amount) {
        float oldHP = health;
        health = health - amount;
        setHealthBar(health);
        float healthScale = (float)health / maxHealth;
        setHealthBar(healthScale);
        Debug.Log("health is now " + health);
    }

    public void setHealthBar(float myHealth) {
        healthBar.transform.localScale = new Vector3(Mathf.Clamp(myHealth, 0f, 1f), healthBar.transform.localScale.y);
    }

    public float getHealth() {
        return health;
    }

    [Command]
    public void Cmdshootfireball() {
        if (fireCooldownLeft <= 0) {
            fireCooldownLeft = fireCooldown;
            Cmdfire();
        }
    }
    [Command]
    protected virtual void Cmdfire() {
        Vector3 location = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        GameObject beam = Instantiate(Projectile, location, Quaternion.identity) as GameObject;
        //beam.GetComponent<Rigidbody2D>().velocity = new Vector3(lookVec.x, projectileSpeed, lookVec.z);
        //beam.transform.Translate(0, projectileSpeed, 0);
        beam.GetComponent<Rigidbody2D>().velocity = aim.transform.TransformDirection(new Vector3(lookVec.x, projectileSpeed, lookVec.z));
        NetworkServer.Spawn(Projectile);
    }

    public void smite() {
        StartCoroutine(smiteDelay());
    }

    IEnumerator smiteDelay() {
        canWalk = false;
        yield return new WaitForSeconds(2f);
        GameObject smite = Instantiate(SmiteSpell, transform.position, Quaternion.identity) as GameObject;
        canWalk = true;

    }
    /*
    protected virtual void FixedUpdate() {
        fireCooldownLeft -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Space) && fireCooldownLeft <= 0) {
            fireCooldownLeft = fireCooldown;
            fire();
        } else if (Input.GetMouseButtonDown(0)) {
            targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition.z = transform.position.z;
        }
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime * 20);
    }
    */

    // Update is called once per frame
    protected virtual void FixedUpdate() {
        if (!isLocalPlayer) {
            return;
        }
        Vector2 moveVec = new Vector2(CrossPlatformInputManager.GetAxis("Horizontal"), CrossPlatformInputManager.GetAxis("Vertical"));
        //bool isboosting = CrossPlatformInputManager.GetButton("Boost"); How to access the fireball button
        lookVec = new Vector3(CrossPlatformInputManager.GetAxis("Horizontal_2"), CrossPlatformInputManager.GetAxis("Vertical_2"), 50000);
        if (canWalk) {
            myBody.AddForce(moveVec * 10);
        }
        if (lookVec.x != 0 && lookVec.y != 0) {
            aim.transform.rotation = Quaternion.LookRotation(lookVec, Vector3.back);
            //transform.rotation = Quaternion.LookRotation(lookVec, Vector3.back);

        }
        //transform.LookAt((transform.position + lookVec));
        fireCooldownLeft -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Space) && fireCooldownLeft <= 0) {
            fireCooldownLeft = fireCooldown;
            Cmdfire();
            //InvokeRepeating("fire", 0.00001f, firingRate);
        }


        if (Input.GetKeyUp(KeyCode.Space)) {
            CancelInvoke("fire");
        }
        if (Input.GetKey(KeyCode.LeftArrow) && (transform.position.x > xmin)) {
            transform.position += Vector3.left * speed * Time.deltaTime;
        } else if (Input.GetKey(KeyCode.RightArrow) && (transform.position.x < xmax)) {
            transform.position += Vector3.right * speed * Time.deltaTime;
        } else if (Input.GetKey(KeyCode.UpArrow) && (transform.position.y < ymax)) {
            transform.position += Vector3.up * speed * Time.deltaTime;
        } else if (Input.GetKey(KeyCode.DownArrow) && (transform.position.y > ymin)) {
            transform.position += Vector3.down * speed * Time.deltaTime;
        }

        //restrict the player to the gamespace
        //float newX = Mathf.Clamp(transform.position.x, xmin, xmax);
        //transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }

    void OnTriggerStay2D(Collider2D collision) {
        if (collision.tag == "Lava") {
            Debug.Log("In lava" + " " + Time.deltaTime);
            float damage = (lavaDamage * Time.deltaTime);
            updateHealth(damage);
        } 
    }


    /*
    void OnTriggerEnter2D(Collider2D collision) {
          if (collision.tag == "Spell") {
              health -= 10;
              print("player " + health);
              Destroy(collision.gameObject);
          }
     */
}
