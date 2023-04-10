using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectCollision : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Powerup"))
        {
            Debug.Log("Powerup collected");
            Destroy(other.gameObject);
        }else if(other.gameObject.CompareTag("Wall"))
        {
            Debug.Log("Game Over!!");
            Destroy(gameObject);
        }
    }
}
