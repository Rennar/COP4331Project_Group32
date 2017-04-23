using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// James Williamson
// Timothy Garrett 

// Script for user input to control the character
public class CharacterControl : BaseClass {
    // Variables for our player.
	public float maxSpeed;
	public float rotSpeed;
	public float acceleration;
    public GameObject bullet;
    public int ammo;
    private float temp = 0;
    public AudioClip shootSound;
    public AudioClip deathSound;
    public AudioClip stepSound;
    private AudioSource shootsource;
    private AudioSource deathsource;
    private AudioSource stepsource;
    Transform mainCamera;
    Vector3 cameraOffset;

	// Friction; use as percentile (.9, .8, etc). A higher percent means less friction 
	public float friction;

	Rigidbody2D rb2D;


	// Use this for initialization
	void Start () {
        // If the player is not a local player we destroy it and return.
        if (!isLocalPlayer)
        {
            Destroy(this);
            return;
        }
        // Creates our RigidBody player
		rb2D = GetComponent<Rigidbody2D> ();
        // Gives our player the amount of ammo allocated to him.
		ammo = GameManager.gm.gunUpgrade + 1;
        // Sets the players maxSpeed that he can achieve.
		maxSpeed = maxSpeed + .5f * GameManager.gm.playerupgrade;
        // Moves our camera over the Player.
        mainCamera = Camera.main.transform;
        cameraOffset = new Vector3(0f, 0f, -10f);
        MoveCamera();
	}
    // Use this on when the game starts.
    private void Awake()
    {
        // An array that holds all our different audio clips.
        AudioSource [] source = GetComponents<AudioSource>();

        shootsource = source[0];
        deathsource = source[1];
        stepsource = source[2];

        shootsource.clip = shootSound;
        deathsource.clip = deathSound;
        stepsource.clip = stepSound;
    }
    // Update is called once per frame
    protected override void CustomUpdate () {
        // If we push the space bar we will shoot if we have ammo.
        if (Input.GetButtonDown("Jump") && ammo > 0)
        {
            // We call our shoot function and decrement our ammo.
            Shoot();
            ammo--;
        }
	}

	void FixedUpdate(){
        // While the game is not paused we will be able to move around.
		if (!paused) {
            // Variables that get the x and y coordinates of the player and their speed.
			float x = Input.GetAxis ("Horizontal");
			float y = Input.GetAxis ("Vertical");
			float currentSpeed = rb2D.velocity.magnitude;
            // If the speed is greater than zero then the players velocity is at its max.
            // This is determined by how much friction the player is encountering.
            if (currentSpeed > 0) {
				rb2D.velocity = rb2D.velocity * friction;
			}
            // If the angularVelocity is greater than zero then the players angularVelocity is at its max.
            // This is determined by how much friction the player is encountering.
			if (rb2D.angularVelocity > 0) {
				rb2D.angularVelocity = rb2D.angularVelocity * friction;
			}
            // While the player is not at their spawn point.
			if (x != 0 || y != 0) {
				// We will increment their speed depending on their acceleration.
				rb2D.velocity += new Vector2 (x * acceleration, y * acceleration);

				if (Mathf.Abs (x) >= .1f || Mathf.Abs (y) >= .1f) {
					transform.eulerAngles = new Vector3 (0, 0, Mathf.Atan2 (y, x) * 180 / Mathf.PI - 90);
				}

			}
            // If the stepSource is not playing and the player is moving we play the step sound.
			if(!stepsource.isPlaying && rb2D.velocity.magnitude != 0)
			{
				stepsource.Play();
			}
            // The volume is determined by the speed of the player.
			stepsource.volume = currentSpeed / maxSpeed;
            // If the player is not moving then we stop playing the step sound.
			if(currentSpeed == 0)
			{
				stepsource.Stop();
			}
            // If the players current speed is greater than the max set speed then we
            // make the players speed be the max speed.
			if (currentSpeed > maxSpeed) {
				rb2D.velocity = Vector2.ClampMagnitude (rb2D.velocity, maxSpeed);
			}
            // Move the camera with the player.
			MoveCamera ();
		}

        
	}

    void OnTriggerEnter2D(Collider2D other){
        // If the player runs into the booty tag then it will either destroy
        // the Enemy or VIP depending on what the objects tag is.
		if (other.name == "Booty") {
            if(other.transform.parent.tag == "Enemy") 
			    other.GetComponentInParent<Enemy> ().OnDeath();
            if(other.transform.parent.tag == "VIP")
                other.GetComponentInParent<VIP>().OnDeath();
		} 
	}

	void OnCollisionEnter2D(Collision2D other){
        // If the player runs into an object with the tag Enemy then they lose.
		if (other.collider.tag == "Enemy") {
            EventManager.TriggerOnLose ();
		}
	}

    void MoveCamera()
    {
        // Moves the camera to follow the player around.
        mainCamera.position = transform.position;
        mainCamera.Translate(cameraOffset);
        
    }

    
    private void Shoot()
    {
        // Creates the sound of a bullet being fired and creates the bullet in front of the player.
        shootsource.PlayOneShot(shootSound, 1f);
        Instantiate(bullet, transform.position, transform.rotation);
    }

	void OnPlayerDeath(){
        // When the player is killed it creates the sound of death and "Kills" the player.
		deathsource.PlayOneShot(deathSound, 1f);
		Destroy (gameObject);
	}

	public void OnEnable(){
		EventManager.OnLose += OnPlayerDeath;
		EventManager.Pause += Pause;
		EventManager.Resume += Resume;
	}

	public void OnDisable(){
		EventManager.OnLose -= OnPlayerDeath;
		EventManager.Pause -= Pause;
		EventManager.Resume -= Resume;
	}
        
}