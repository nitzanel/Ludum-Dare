using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Task
{
    public Vector3 destination;

    public Task(Vector3 Destination)
    {
        destination = Destination;
    }
}

public class Mummy : MonoBehaviour
{
    public float speed = 0.1f;

    private Queue<Task> tasks;
	void Start ()
    {
        tasks = new Queue<Task>();

        StartCoroutine("TaskManager");
	}
	
	void Update ()
    {
        if (Input.GetMouseButton(0))
        {
            AddTask(new Vector3(0, transform.position.y, transform.position.z));
        }
	}

    public void ChangeDirection()
    {
        speed = -speed;
    }

    public void AddTask(Vector3 destination)
    {
        tasks.Enqueue(new Task(destination));
    }

    IEnumerator TaskManager()
    {
        while (true)
        {
            if (tasks.Count > 0)
            {
                Task t = tasks.Dequeue();
                while (Vector3.Distance(transform.position, t.destination) > 0.05f)
                {
                    transform.position = Vector3.Lerp(transform.position, t.destination, Mathf.Abs(speed) * Time.deltaTime);
                    yield return null;
                }
            }
            else
            {
                transform.position = new Vector3(transform.position.x + speed * Time.deltaTime, transform.position.y, transform.position.z);
            }
            yield return null;
        }
    }
}
