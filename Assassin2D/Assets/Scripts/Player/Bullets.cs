using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Timothy Garrett

public class Bullets : BaseClass {
    // Variables for the speed and existence of the bullet object.
    public float velocity;
    public float lifetime;

	// Use this for initialization
	void Start () {
        StartCoroutine("killtimer");
	}
	
	// Update is called once per frame
	protected override void CustomUpdate () {
        // Moves the position based on where the player is looking and the speed of the bullet.
        transform.position += (transform.up * Time.deltaTime * velocity);
	}
    // Will destroy the bullet after so many seconds.
    IEnumerator killtimer() {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }
    // If our bullet collides with an object we check its tag and deteremine
    // what needs to be done then destory the Bullet object.
    private void OnTriggerEnter2D(Collider2D other)
    {
        // If the Object has the tag enemy.
        if(other.tag == "Enemy")
        {
            // Kills the enemy and destroys the bullet Object.
            other.GetComponent<Enemy>().OnDeath();
            Destroy(gameObject);
        }
        // If the Object has the tag VIP.
        else if (other.tag == "VIP")
        {
            // Kills the VIP and destroys the bullet Object.
            other.GetComponent<VIP>().OnDeath();
            Destroy(gameObject);
        }
        // If the Object has the tag Wall.
        else if (other.tag == "Wall")
        {
            // It will just destroy the Bullet Object.
            Destroy(gameObject);
        }
    }

}
