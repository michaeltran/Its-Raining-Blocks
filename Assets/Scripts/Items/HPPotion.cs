using UnityEngine;
using System.Collections;

public class HPPotion : Potion
{
    void OnTriggerEnter (Collider other)
    {
        if(other.gameObject.tag == "PlayerCollider")
        {
            AudioSource.PlayClipAtPoint(potionGrab, transform.position);
			other.gameObject.transform.parent.SendMessage ("Heal", 25);
            Destroy(this.gameObject);
        }
    }
}