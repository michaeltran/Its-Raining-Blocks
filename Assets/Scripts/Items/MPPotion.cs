using UnityEngine;
using System.Collections;

public class MPPotion : Potion
{
    void OnTriggerEnter (Collider other)
    {
        if(other.gameObject.tag == "PlayerCollider")
        {
            AudioSource.PlayClipAtPoint(potionGrab, transform.position);
			other.gameObject.transform.parent.SendMessage ("HealMana", 25);
            Destroy(this.gameObject);
        }
    }
}