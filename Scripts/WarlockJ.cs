using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.Networking;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class WarlockJ : NetworkBehaviour {
    [SerializeField] ButtonHandler shootButton;
    [SerializeField] Joystick joystick;
    [SerializeField] GameObject aim;
    [SerializeField] GameObject SmiteSpell;
    [SerializeField] GameObject hud;
    [SerializeField] float movementSpeed;
    [SerializeField] GameObject point;
    [SerializeField] Camera cam;
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
    private float lavaDamage = 6f;
    private bool canWalk = true;
    private bool move = false;
    private bool toMove;
    private Animator anim;
    private bool shooting = false;
    private Vector3 shootLocation;
    private Quaternion projectileQuaternion;
    private GameObject target;
    private bool smiting;
    private bool hitBySpell;


    private float fireCooldown = .5f;
    private float fireCooldownLeft = 0;

    //Used to differentiate between local player, and other player
    public override void OnStartLocalPlayer() {
        tag = "Player";
        Canvas hud2 = hud.GetComponent<Canvas>();
        hud2.enabled = true;
    }
    
    // Use this for initialization
    void Start() {
        anim = GetComponent<Animator>();
        myBody = GetComponent <Rigidbody2D>();
        float distance = transform.position.z - cam.transform.position.z;
        Vector3 leftmost = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, distance));
        Vector3 rightmost = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, distance));

        xmin = leftmost.x + padding;
        xmax = rightmost.x - padding;
        targetPosition = transform.position;
    }

    //Update the Warlocks Health. Amount will be subtracted (+ amount subtracts health)
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

    //Return this warlocks health
    public float getHealth() {
        return health;
    }

    // Command shoot fireball shoots fireball to the network
    [Command]
    public void Cmdshootfireball() {
        if (fireCooldownLeft <= 0) {
            canWalk = false;
            fireCooldownLeft = fireCooldown;
            shooting = true;
            Debug.Log(shooting);

        }
    }

    // Button handler IN PROGRESS
    [Command]
    protected virtual void Cmdfire() {
        Vector3 location = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
        GameObject beam = Instantiate(Projectile, location, Quaternion.identity) as GameObject;
        //beam.GetComponent<Rigidbody2D>().velocity = new Vector3(targetPosition.x, projectileSpeed, lookVec.z);
        //beam.transform.Translate(targetPosition.x, projectileSpeed, 0);
        //beam.GetComponent<Rigidbody2D>().velocity = beam.transform.TransformDirection(new Vector3(shootLocation.x, projectileSpeed, lookVec.z));
        
        //shooting = false;
        //beam.GetComponent<Rigidbody2D>().velocity = target * 10;
        beam.GetComponent<Rigidbody2D>().velocity = aim.transform.TransformDirection(new Vector3(0, -projectileSpeed, lookVec.z));
        //NetworkServer.Spawn(Projectile);
    }

    //Start cooldown on smite spell
    public void smite() {
        StartCoroutine(smiteDelay());
    }

    //Delay of 2 seconds on smite spell
    IEnumerator smiteDelay() {
        smiting = true;
        canWalk = false;
        yield return new WaitForSeconds(2f);
        GameObject smite = Instantiate(SmiteSpell, transform.position, Quaternion.identity) as GameObject;
        canWalk = true;
        smiting = false;

    }

    public void hit() {
        StartCoroutine(hitDelay());
    }

    IEnumerator hitDelay() {
        hitBySpell = true;
        canWalk = false;
        yield return new WaitForSeconds(.5f);
        targetPosition = transform.position;
        canWalk = true;
        hitBySpell = false;
    }
    // Update is called once per frame
    protected virtual void Update() {
        if (!isLocalPlayer) {
            return;
        }
        fireCooldownLeft -= Time.deltaTime;

        //CLICK TO MOVE
        if (shooting) {
            canWalk = false;
            //if (!EventSystem.current.IsPointerOverGameObject()) { // MOUSE
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) { // TOUCH MOBILE
                if (!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) {
                    Plane playerPlane = new Plane(Vector3.forward, transform.position);
                    Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                    targetPosition = cam.ScreenToWorldPoint(Input.mousePosition);
                    float hitdist = 0.0f;

                    Vector3 mousePositionVector3 = cam.ScreenToWorldPoint(Input.mousePosition);
                    if (playerPlane.Raycast(ray, out hitdist)) {
                        targetPosition = ray.GetPoint(hitdist);
                        mousePositionVector3 = targetPosition;
                    }

                    Debug.Log("targetPosition = " + targetPosition + "Mouse position = " + mousePositionVector3);
                    Vector3 targetdir = mousePositionVector3 - transform.position;
                    projectileQuaternion = Quaternion.LookRotation(Vector3.forward, targetdir);
                    if (Input.GetMouseButtonDown(0)) {
                        GameObject fireball = Instantiate(Projectile, transform.position, projectileQuaternion) as GameObject;
                        Rigidbody2D rigid = fireball.GetComponent<Rigidbody2D>();
                        rigid.velocity = fireball.transform.up * 10;
                        shooting = false;
                    }
                }
            }
        } else {
            if (smiting) {
                targetPosition = transform.position;
            }
            if (Input.GetMouseButtonDown(0)) {
                if (!smiting && !hitBySpell) {
                    canWalk = true;
                }
                Plane playerPlane = new Plane(Vector3.forward, transform.position);
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                //if (!EventSystem.current.IsPointerOverGameObject()) { // MOUSE 
                if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) { // TOUCH MOBILE
                    if (!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) {
                        toMove = true;
                    }
                } else {
                    toMove = false;
                }
                targetPosition = cam.ScreenToWorldPoint(Input.mousePosition);
                float hitdist = 0.0f;

                if (playerPlane.Raycast(ray, out hitdist) && toMove && !smiting) {
                    targetPosition = ray.GetPoint(hitdist);
                    float x = targetPosition.x;
                    float y = targetPosition.y;
                    float diff = (Mathf.Abs(y - transform.position.y)) - (Mathf.Abs(x - transform.position.x));

                    // Animate the Warlock while walking
                    if (toMove) {
                        if (diff >= 0) {
                            if (y > transform.position.y) {
                                anim.SetBool("walkingUp", true);
                            } else {
                                anim.SetBool("walkingDown", true);
                            }
                        } else {
                            if (x > transform.position.x) {
                                anim.SetBool("walkingRight", true);
                            } else {
                                anim.SetBool("walkingLeft", true);
                            }
                        }
                    }
                    // Create a point where the user clicks, the Warlock walks to this point
                    Instantiate(point, targetPosition, Quaternion.identity);
                    targetPosition.z = transform.position.z;
                    
            }
        }
        
            if (toMove && canWalk && !smiting) {
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, Time.deltaTime * movementSpeed);
                if (transform.position == targetPosition) {
                    clearBools();
                    anim.SetBool("isIdle", true);
                }
            }
        }


    }
    // Set all Animator Controller bools to false
    private void clearBools() {
        anim.SetBool("walkingDown", false);
        anim.SetBool("walkingUp", false);
        anim.SetBool("walkingLeft", false);
        anim.SetBool("walkingRight", false);

    }



    void OnTriggerStay2D(Collider2D collision) {
        if (collision.tag == "Lava") {
            Debug.Log("In lava" + " " + Time.deltaTime);
            float damage = (lavaDamage * Time.deltaTime);
            updateHealth(damage);
        } 
    }
}

// Update is called once per frame
/*
protected virtual void FixedUpdate() {
    //JOYSTICK MOVEMENT Warlock with 2 Joysticks
    /*
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
        //Cmdfire();
        //InvokeRepeating("fire", 0.00001f, firingRate);
    }


    //restrict the player to the gamespace
    float newX = Mathf.Clamp(transform.position.x, xmin, xmax);
    transform.position = new Vector3(newX, transform.position.y, transform.position.z);

}
*/
