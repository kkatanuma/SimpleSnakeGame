using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectCollision : MonoBehaviour
{
    PlayerController playerController;

    void Awake()
    {
       playerController = GetComponent<PlayerController>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Powerup"))
        {
            Debug.Log("Powerup collected");
            playerController.addNode = true;
            Destroy(other.gameObject);
        }else if(other.gameObject.CompareTag("Wall"))
        {
            Debug.Log("Game Over!!");
            Destroy(gameObject);
        }else if (other.gameObject.CompareTag("Tail"))
        {
            Debug.Log("Game Over!!");
            Destroy(gameObject);
        }
    }
}
