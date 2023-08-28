using UnityEngine;
using System.Collections.Generic;

public class Skate : MonoBehaviour
{
    public float glue;
    public float jumpForce;
    public float mouseSpeed;
    public float speed;
    public float maxSpeed;
    public float force;

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Transform player = other.gameObject.transform;

            player.position = Vector3.Lerp(player.position, transform.position, glue);

            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            transform.rotation *= Quaternion.Euler(mouseY * mouseSpeed, 0, mouseX * mouseSpeed);

            if (Input.GetKey(KeyCode.W))
            {
                if (speed < maxSpeed) speed += Time.deltaTime * force;
                GetComponent<Rigidbody>().AddForce(player.forward * speed);
            }
            else
            {
                speed = 0;
            }

            if (Input.GetKeyDown(KeyCode.Space)) GetComponent<Rigidbody>().AddForce(player.up * jumpForce);
        }
    }
}