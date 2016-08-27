using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// TASK TASK TASK
/// </summary>
[System.Serializable]
public class Task
{
    public Vector3 destination;
    public enum action { WALK, FURNACE, WOOD };
    public action a;
    public Interactable calledMe;

    public Task(Vector3 Destination, action A = action.WALK, Interactable CalledMe = null)
    {
        destination = Destination;
        a = A;
        calledMe = CalledMe;
    }
}
/// <summary>
/// END END END
/// </summary>

public class Mummy : MonoBehaviour
{
    public bool isSelected = false;

    public Transform marker = null;

    public float speed = 0.5f;
    public Platform platform = null;
    private Platform nextPlatform = null;

    private Queue<Task> tasks;
    private List<Item> inventory;
    private bool onLadder = false;

	void Start ()
    {
        tasks = new Queue<Task>();
        inventory = new List<Item>();

        StartCoroutine("TaskManager");
	}
	
	void Update ()
    {
        if (Input.GetMouseButtonDown(0) && isSelected)
        {
            AddTask(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z)));
        }
	}

    public void OnMouseDown()
    {
        if (Input.GetKey("mouse 0"))
        {
            isSelected = !isSelected;
            if (isSelected)
                transform.GetComponent<SpriteRenderer>().color = Color.yellow;
            else
                transform.GetComponent<SpriteRenderer>().color = Color.white;
            transform.GetComponent<AudioSource>().Play();
        }
    }

    public void ChangeDirection()
    {
        speed = -speed;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    public void AddTask(Vector3 destination, Task.action action = Task.action.WALK, Interactable calledMe = null)
    {
        tasks.Enqueue(new Task(destination, action, calledMe));
    }

    IEnumerator TaskManager()
    {
        while (true)
        {
            if (tasks.Count > 0)
            {
                //Move to task:
                BoxCollider2D c = transform.GetComponent<BoxCollider2D>();
                c.enabled = false;

                float prevSpeed = speed;
                if (speed < 0) ChangeDirection();
                float halfHeight = transform.GetComponent<SpriteRenderer>().bounds.size.y / 2;
                Task t = tasks.Dequeue();
                Vector3 destination = NormalizeDestination(t.destination);
                if (marker) marker.position = new Vector3(destination.x, destination.y + transform.GetComponent<SpriteRenderer>().bounds.size.y / 2, 0);
                SetPlatforms(false, true);
                while (Mathf.Abs(transform.position.x - destination.x) > 0.01 || Mathf.Abs(transform.position.y - halfHeight - destination.y) > 0.01)
                {
                    transform.position = Navigate(destination);
                    yield return null;
                }
                marker.position = new Vector3(100, 0, 0);
                if (speed != prevSpeed) ChangeDirection();

                c.enabled = true;

                //Do Task:
                switch (t.a)
                {
                    case Task.action.FURNACE:
                        foreach (Item item in inventory)
                        {
                            if (item.name == "WOOD" && item.amount > 0)
                            {
                                item.amount--;
                                t.calledMe.Interact();
                            }
                        }
                        break;
                    case Task.action.WOOD:
                        bool found = false;
                        foreach (Item item in inventory)
                        {
                            if (item.name == "WOOD")
                            {
                                item.amount++;
                                found = true;
                            }
                        }
                        if (!found)
                            inventory.Add(new Item("WOOD"));
                        break;
                }
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
            {
                if (speed < 0) ChangeDirection();
                return new Vector3(transform.position.x + speed * Time.deltaTime, transform.position.y, transform.position.z);
            }
            else
            {
                if (speed > 0) ChangeDirection();
                return new Vector3(transform.position.x + speed * Time.deltaTime, transform.position.y, transform.position.z);
            }
        }
        else if (destination.y > transform.position.y - halfHeight) //currently below target platform - go up
        {
            //Debug.Log("currently below target platform");
            if (onLadder) //continue moving up the ladder
            {
                destination = new Vector3(nextPlatform.transform.position.x, nextPlatform.transform.position.y + platformHalfHeight, nextPlatform.transform.position.z);
                if (speed < 0) ChangeDirection();
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
                {
                    if (speed < 0) ChangeDirection();
                    return new Vector3(transform.position.x + speed * Time.deltaTime, transform.position.y, transform.position.z);
                }
                else
                {
                    if (speed > 0) ChangeDirection();
                    return new Vector3(transform.position.x + speed * Time.deltaTime, transform.position.y, transform.position.z);
                }
            }
        }
        else if (transform.position.y - halfHeight > destination.y) //currently above target platform - go down
        {
            //Debug.Log("currently above target platform");
            if (onLadder) //continue moving down the ladder
            {
                destination = new Vector3(nextPlatform.transform.position.x, nextPlatform.transform.position.y + platformHalfHeight, nextPlatform.transform.position.z);
                if (speed > 0) ChangeDirection();
                if (transform.position.y - halfHeight + speed * Time.deltaTime < destination.y)
                    onLadder = false;
                return new Vector3(transform.position.x, transform.position.y + speed * Time.deltaTime, transform.position.z);
            }
            else //move to current ladder
            {
                SetPlatforms(false, false);
                float dif = platform.ladderDown.position.x - transform.position.x;
                if (Mathf.Abs(dif) < 0.05)
                    onLadder = true;
                else if (dif > 0)
                {
                    if (speed < 0) ChangeDirection();
                    return new Vector3(transform.position.x + speed * Time.deltaTime, transform.position.y, transform.position.z);
                }
                else
                {
                    if (speed > 0) ChangeDirection();
                    return new Vector3(transform.position.x + speed * Time.deltaTime, transform.position.y, transform.position.z);
                }
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
                if (destination.x < o.transform.position.x - o.GetComponent<SpriteRenderer>().bounds.size.x / 2 + transform.GetComponent<SpriteRenderer>().bounds.size.x)
                    xVal = o.transform.position.x - o.GetComponent<SpriteRenderer>().bounds.size.x / 2 + transform.GetComponent<SpriteRenderer>().bounds.size.x;
                else if (destination.x > o.transform.position.x + o.GetComponent<SpriteRenderer>().bounds.size.x / 2 - transform.GetComponent<SpriteRenderer>().bounds.size.x)
                    xVal = o.transform.position.x + o.GetComponent<SpriteRenderer>().bounds.size.x / 2 - transform.GetComponent<SpriteRenderer>().bounds.size.x;
                else
                    xVal = destination.x;

                return new Vector3(xVal, o.transform.position.y + o.GetComponent<SpriteRenderer>().bounds.size.y / 2, destination.z);
            }
        }
        return new Vector3(0, 0, 0);
    }
}
