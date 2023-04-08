using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 10.0f;
    private Vector3 turnAngle = new Vector3(0, 90, 0);
    private Vector3 currentAngle = new Vector3(0, 0, 0);
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);


        if (Input.GetKeyDown(KeyCode.A))
        {
            if (transform.rotation.eulerAngles == new Vector3(0, 0, 0))
                transform.Rotate(new Vector3(0, -90, 0));
            else if (transform.rotation.eulerAngles == new Vector3(0, 180, 0))
                transform.Rotate(new Vector3(0, 90, 0));
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            if (transform.rotation.eulerAngles == new Vector3(0, 0, 0))
                transform.Rotate(new Vector3(0, 90, 0));
            else if (transform.rotation.eulerAngles == new Vector3(0, 180, 0))
                transform.Rotate(new Vector3(0, -90, 0));
        }
        else if(Input.GetKeyDown(KeyCode.W))
        {
            if (transform.rotation.eulerAngles == new Vector3(0, 270, 0))
                transform.Rotate(new Vector3(0, 90, 0));
            else if (transform.rotation.eulerAngles == new Vector3(0, 90, 0))
                transform.Rotate(new Vector3(0, -90, 0));
        }
        else if(Input.GetKeyDown(KeyCode.S))
        {
            if (transform.rotation.eulerAngles == new Vector3(0, 270, 0))
                transform.Rotate(new Vector3(0, -90, 0));
            else if (transform.rotation.eulerAngles == new Vector3(0, 90, 0))
                transform.Rotate(new Vector3(0, 90, 0));
        }
        Debug.Log(transform.rotation.eulerAngles);
    }
}
