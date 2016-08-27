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
    public float speed = 0.5f;
    public Platform platform = null;
    private Platform nextPlatform = null;

    private Queue<Task> tasks;
    private bool onLadder = false;

	void Start ()
    {
        tasks = new Queue<Task>();

        StartCoroutine("TaskManager");
	}
	
	void Update ()
    {
        if (Input.GetMouseButtonDown(0))
        {
            AddTask(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z)));
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
                //Rigidbody2D r = transform.GetComponent<Rigidbody2D>();
                BoxCollider2D c = transform.GetComponent<BoxCollider2D>();
                //r.gravityScale = 0;
                c.enabled = false;

                float prevSpeed = speed;
                speed = Mathf.Abs(speed);
                float halfHeight = transform.GetComponent<SpriteRenderer>().bounds.size.y / 2;
                Task t = tasks.Dequeue();
                Vector3 destination = NormalizeDestination(t.destination);
                SetPlatforms(false, true);
                //Debug.Log(originalPosition.ToString() + "  ->  " + destination.ToString());
                while (Mathf.Abs(transform.position.x - destination.x) > 0.01 || Mathf.Abs(transform.position.y - halfHeight - destination.y) > 0.01)
                {
                    transform.position = Navigate(destination);
                    yield return null;
                }
                //Debug.Log("Arrived");
                speed = prevSpeed;

                //r.gravityScale = 1;
                c.enabled = true;
            }
            else
            {
                transform.position = new Vector3(transform.position.x + speed * Time.deltaTime, transform.position.y, transform.position.z);
            }
            yield return null;
        }
    }

    private Vector3 Navigate (Vector3 destination)
    {
        float halfHeight = transform.GetComponent<SpriteRenderer>().bounds.size.y / 2;
        float platformHalfHeight = platform.transform.GetComponent<SpriteRenderer>().bounds.size.y / 2;
        if (Mathf.Abs(destination.y - (transform.position.y - halfHeight)) < 0.01) //on target platform
        {
            //Debug.Log("on target platform");
            SetPlatforms(false, true);
            if (destination.x > transform.position.x)
                return new Vector3(transform.position.x + speed * Time.deltaTime, transform.position.y, transform.position.z);
            else
                return new Vector3(transform.position.x - speed * Time.deltaTime, transform.position.y, transform.position.z);
        }
        else if (destination.y > transform.position.y - halfHeight) //currently below target platform - go up
        {
            //Debug.Log("currently below target platform");
            if (onLadder) //continue moving up the ladder
            {
                destination = new Vector3(nextPlatform.transform.position.x, nextPlatform.transform.position.y + platformHalfHeight, nextPlatform.transform.position.z);
                if (destination.y < transform.position.y - halfHeight + speed * Time.deltaTime)
                    onLadder = false;
                return new Vector3(transform.position.x, transform.position.y + speed * Time.deltaTime, transform.position.z);
            }
            else //move to current ladder
            {
                SetPlatforms(true, false);
                float dif = platform.ladderUp.position.x - transform.position.x;
                if (Mathf.Abs(dif) < 0.05)
                    onLadder = true;
                else if (dif > 0)
                    return new Vector3(transform.position.x + speed * Time.deltaTime, transform.position.y, transform.position.z);
                else
                    return new Vector3(transform.position.x - speed * Time.deltaTime, transform.position.y, transform.position.z);
            }
        }
        else if (transform.position.y - halfHeight > destination.y) //currently above target platform - go down
        {
            //Debug.Log("currently above target platform");
            if (onLadder) //continue moving down the ladder
            {
                destination = new Vector3(nextPlatform.transform.position.x, nextPlatform.transform.position.y + platformHalfHeight, nextPlatform.transform.position.z);
                if (transform.position.y - halfHeight - speed * Time.deltaTime < destination.y)
                    onLadder = false;
                return new Vector3(transform.position.x, transform.position.y - speed * Time.deltaTime, transform.position.z);
            }
            else //move to current ladder
            {
                SetPlatforms(false, false);
                float dif = platform.ladderDown.position.x - transform.position.x;
                if (Mathf.Abs(dif) < 0.05)
                    onLadder = true;
                else if (dif > 0)
                    return new Vector3(transform.position.x + speed * Time.deltaTime, transform.position.y, transform.position.z);
                else
                    return new Vector3(transform.position.x - speed * Time.deltaTime, transform.position.y, transform.position.z);
            }
        }
        //Debug.Log("end");
        return transform.position;
    }

    /*
    private Platform GetCurrentPlatform()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Platform");
        foreach (GameObject o in objects)
        {
            if (Mathf.Abs(transform.position.y - o.transform.position.y - transform.GetComponent<SpriteRenderer>().bounds.size.y / 2) < 0.1)
                return o.transform.GetComponent<Platform>();
        }
        return null;
    }

    private Vector3 SetTargetToNextPlatform(bool up)
    {
        Debug.Log(up);
        if (up)
        {
            Transform l = GetCurrentPlatform().ladderUp;

            GameObject[] objects = GameObject.FindGameObjectsWithTag("Platform");
            foreach (GameObject o in objects)
            {
                if (o.transform.GetComponent<Platform>().ladderDown == l)
                    return new Vector3(transform.position.x, o.transform.position.y + transform.GetComponent<SpriteRenderer>().bounds.size.y / 2, transform.position.z);
            }
        }
        else
        {
            Transform l = GetCurrentPlatform().ladderDown;

            GameObject[] objects = GameObject.FindGameObjectsWithTag("Platform");
            foreach (GameObject o in objects)
            {
                if (o.transform.GetComponent<Platform>().ladderUp == l)
                    return new Vector3(transform.position.x, o.transform.position.y + transform.GetComponent<SpriteRenderer>().bounds.size.y / 2, transform.position.z);
            }
        }
        return new Vector3(0, 0, 0);
    }
    */

    private void SetPlatforms(bool up, bool ignore)
    {
        onLadder = false;

        GameObject[] objects = GameObject.FindGameObjectsWithTag("Platform");
        foreach (GameObject o in objects)
        {
            if (Mathf.Abs(transform.position.y - o.transform.position.y - transform.GetComponent<SpriteRenderer>().bounds.size.y / 2) < 0.1)
                platform = o.transform.GetComponent<Platform>();
        }

        if (ignore)
        {
            nextPlatform = null;
            return;
        }
        if (up)
        {
            Transform l = platform.ladderUp;

            objects = GameObject.FindGameObjectsWithTag("Platform");
            foreach (GameObject o in objects)
            {
                if (o.transform.GetComponent<Platform>().ladderDown == l)
                    nextPlatform = o.transform.GetComponent<Platform>();
            }
        }
        else
        {
            Transform l = platform.ladderDown;

            objects = GameObject.FindGameObjectsWithTag("Platform");
            foreach (GameObject o in objects)
            {
                if (o.transform.GetComponent<Platform>().ladderUp == l)
                    nextPlatform = o.transform.GetComponent<Platform>();
            }
        }
    }

    private Vector3 NormalizeDestination(Vector3 destination)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Platform");
        foreach (GameObject o in objects)
        {
            if (destination.y - o.transform.position.y < 0.5 && destination.y - o.transform.position.y >= 0)
            {
                float xVal;
                if (destination.x < o.transform.position.x - o.GetComponent<SpriteRenderer>().bounds.size.x / 2 + transform.GetComponent<SpriteRenderer>().bounds.size.x / 2)
                    xVal = o.transform.position.x - o.GetComponent<SpriteRenderer>().bounds.size.x / 2 + transform.GetComponent<SpriteRenderer>().bounds.size.x / 2;
                else if (destination.x > o.transform.position.x + o.GetComponent<SpriteRenderer>().bounds.size.x / 2 - transform.GetComponent<SpriteRenderer>().bounds.size.x / 2)
                    xVal = o.transform.position.x + o.GetComponent<SpriteRenderer>().bounds.size.x / 2 - transform.GetComponent<SpriteRenderer>().bounds.size.x / 2;
                else
                    xVal = destination.x;

                return new Vector3(xVal, o.transform.position.y + o.GetComponent<SpriteRenderer>().bounds.size.y / 2, destination.z);
            }
        }
        return new Vector3(0, 0, 0);
    }
}
