using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 10.0f;
    public bool gameOver = false;
    public GameObject tailPrefab;
    public GameObject playerPrefab;
    private List<GameObject> body = new List<GameObject>();
    public float tailOffset = 1.25f;
    private bool changeDirection = false;
    private Quaternion currentDirection;
    // Start is called before the first frame update
    void Start()
    {
        body.Add(gameObject);
        GameObject newTail = Instantiate(tailPrefab, transform.position - transform.forward * tailOffset, Quaternion.identity);
        body.Add(newTail);
    }

    // Update is called once per frame
    void Update()
    {
        //transform.Translate(Vector3.forward * speed * Time.deltaTime);
        
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (transform.rotation.eulerAngles == new Vector3(0, 0, 0))
                transform.Rotate(new Vector3(0, -90, 0));
            else if (transform.rotation.eulerAngles == new Vector3(0, 180, 0))
                transform.Rotate(new Vector3(0, 90, 0));

            changeDirection = true;
            MoveHead();
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            if (transform.rotation.eulerAngles == new Vector3(0, 0, 0))
                transform.Rotate(new Vector3(0, 90, 0));
            else if (transform.rotation.eulerAngles == new Vector3(0, 180, 0))
                transform.Rotate(new Vector3(0, -90, 0));

            changeDirection = true;
            MoveHead();
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            if (transform.rotation.eulerAngles == new Vector3(0, 270, 0))
                transform.Rotate(new Vector3(0, 90, 0));
            else if (transform.rotation.eulerAngles == new Vector3(0, 90, 0))
                transform.Rotate(new Vector3(0, -90, 0));

            changeDirection = true;
            MoveHead();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            if (transform.rotation.eulerAngles == new Vector3(0, 270, 0))
                transform.Rotate(new Vector3(0, -90, 0));
            else if (transform.rotation.eulerAngles == new Vector3(0, 90, 0))
                transform.Rotate(new Vector3(0, 90, 0));

            changeDirection = true;
            MoveHead();
        }
        Debug.Log(gameObject.transform.rotation.eulerAngles);
    }

    private void LateUpdate()
    {
        MoveTail();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Powerup"))
        {
            Debug.Log("Powerup collected");
            Grow();
            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("Wall"))
        {
            Debug.Log("Game Over!!");
            Destroy(gameObject);
        }
    }

    private void Grow()
    {
        GameObject newTail = Instantiate(tailPrefab, transform.position - transform.forward * tailOffset, Quaternion.identity);
        body.Add(newTail);
    }

    private void MoveTail()
    {
        for (int i = 1; i < body.Count; i++)
        {
            body[i].transform.position = body[i - 1].transform.position - transform.forward * tailOffset;
        }
    }

    private void MoveHead()
    {
        if (changeDirection)
        {
            GameObject newHead = Instantiate(playerPrefab, transform.position, Quaternion.Euler(new Vector3(0,270, 0)));
        }
    }
}
