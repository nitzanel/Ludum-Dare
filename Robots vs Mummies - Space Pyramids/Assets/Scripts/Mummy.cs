using UnityEngine;
using System.Collections;

public class Mummy : MonoBehaviour
{
    public float speed = 0.1f;

	void Start ()
    {
	
	}
	
	void Update ()
    {
        transform.position = new Vector3(transform.position.x + speed * Time.deltaTime, transform.position.y, transform.position.z);
	}

    public void ChangeDirection()
    {
        speed = -speed;
    }
}
