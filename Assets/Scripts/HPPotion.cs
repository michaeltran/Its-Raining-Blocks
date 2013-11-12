using UnityEngine;
using System.Collections;

public class HPPotion : MonoBehaviour
{
    public AudioClip potionGrab;                    // Audioclip to play when the key is picked up.
    
	public float fallSpeed = 350f;
    
    private GameObject player;                      // Reference to the player.
    private Status status;
    
    
    void Awake ()
    {
        // Setting up the references.
        player = GameObject.FindGameObjectWithTag("Player");
        status = player.GetComponent<Status>();
		rigidbody.AddForce(Vector3.down*fallSpeed);
    }
	
	void FixedUpdate()
	{
		//transform.position = Vector3.Lerp (transform.position, targ) //speed * Time.deltaTime;
		//rigidbody.AddForce(Vector3.down*10);
	}
    
    
    void OnTriggerEnter (Collider other)
    {
        // If the colliding gameobject is the player...
        if(other.gameObject.tag == "PlayerCollider")
        {
            //AudioSource.PlayClipAtPoint(potionGrab, transform.position);
            
			status.Heal(25);
			
            // ... and destroy this gameobject.
            Destroy(this.gameObject);
        }
    }
}