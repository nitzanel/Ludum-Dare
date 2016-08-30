using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Mummy : MonoBehaviour
{
	
    public bool isSelected = false;

    public Transform icon = null;
    public Transform marker = null;

    public float speed = 0.3f;
    public Platform platform = null;
    private Platform nextPlatform = null;

    private Queue<Task> tasks;
    public List<Item> inventory;
    private bool onLadder = false;
	private float taskCooldown = 0.1f;
	private float currentCooldown = 0.0f;
	void Start ()
    {
        tasks = new Queue<Task>();
        inventory = new List<Item>();
		marker = GameObject.Find ("Marker").transform;

        StartCoroutine("TaskManager");
	}
	
	void Update ()
    {
		currentCooldown -= Time.deltaTime;
		if (Input.GetMouseButtonDown (0) && isSelected) 
		{
			if (currentCooldown <= 0.0f)
			{
				// Adds a task to the mummy at the world position clicked.
				AddTask (Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, transform.position.z)));
				currentCooldown = taskCooldown;
			}
		}


	}

	/* This function is called by the PlayerManager automaticly.
	 * Input:
	 * bool selected - true if the mummy was just selected and was not selected before.
	 * false if the mummy was just selected but was selected before (cancel selection)
	*/
	public void SelectMummy(bool selected)
	{
		isSelected = selected;
		if (selected) 
		{
			transform.GetComponent<SpriteRenderer> ().color = Color.yellow;
			transform.GetComponent<AudioSource> ().Play ();
		}
		else transform.GetComponent<SpriteRenderer>().color = Color.white;
	}

    public void OnMouseDown()
    {
		// it is not required to check for the mouse button,
		//because the function only get called when the mouse button clicked was mouse 0.
        //if (Input.GetKey("mouse 0"))
        //{
		// calls the PlayerManager to select or deselect mummies.
		PlayerManager.SetCurrentlySelected(this);        
    	//    }
    }

	/* This function Changes the direction of the mummy.
	 * When the mummy gets to close to the edge of a platform, the function will be called and the mummy's
	 * movement direction will be inverted.
	 * The function will also cause the mummy's sprite to turn to the other side.
	*/
    public void ChangeDirection()
    {
        speed = -speed;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

	/* This funtion adds a task to the mummy.
	 * Input:
	 * Vector3 destination - the position of the mouse button where the user clicked.
	 * Task.action action - the type of action the mummy will perform. defaults to Task.action.WALK.
	 * Interactable calledMe - the Interactable item/unit/whatever (that inherits from Interactable)
	 * that is the object of the task. example - Furnace. defaults no null. 
	 * This function will be called by Interactable objects (usually).
	*/
    public void AddTask(Vector3 destination, Task.action action = Task.action.WALK, Interactable calledMe = null)
    {
        tasks.Enqueue(new Task(destination, action, calledMe));
    }


	/* This function manages the tasks the mummy has to perform.
	 * The TaskManager runs on another thread, while doing the tasks.
	 * The TaskManager will keep running until it is out of tasks.
	 * The TaskManager call the navigation function by itself.
	*/
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
                            if (item.t == Item.type.WOOD && item.amount > 0)
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
                            if (item.t == Item.type.WOOD)
                            {
                                item.amount++;
                                found = true;
                            }
                        }
                        if (!found)
                            inventory.Add(new Item(Item.type.WOOD));
                        break;
				case Task.action.WAKE_THE_DEAD:
					// Pass the transform of the mummy to the function.
					t.calledMe.Interact (transform);
					break;
                }
				// Update the mummy's Inventory display.
                UpdateInventoryDisplay();
            }
            else
            {
                transform.position = new Vector3(transform.position.x + speed * Time.deltaTime, transform.position.y, transform.position.z);
            }
            yield return null;
        }
    }
	/* This function is the pathfinding algorithem the mummy uses to move around the pyramid.
	 * Input:
	 * Vector3 destination - The place where the mummy need to reach. 
	 * The Vector must be normalized with the function NormalizeDesitnation.
	*/
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

	/*
	* This function finds what platform the mummy should move to next.
	* Input:
	* bool up - true: the mummy should go up. false: the mummy should go down.
	* bool ignore: true: the mummy reached the platform it needed, and should not go up or down anymore.
	* false: The mummy hasn't reached its destination yet.
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
	/* This function returns a normalized destination vector.
	 * Input:
	 * Vector3 destination - The destination is the place the mummy need to go before normalization.
	 * Output:
	 * Normalized destination Vector3.
	 * The function is called automaticly by Navigate.
	*/
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

	/*
	 * This function will update the inventory display icon(s) (soon)
	 * The function is called automaticly by the TaskManager function.
	*/
    private void UpdateInventoryDisplay()
    {
        KillInventory();
        int itemCount = 0;
        int itemIndex = 0;
        foreach (Item item in inventory) if (item.amount > 0) itemCount++;
        foreach (Item item in inventory)
        {
            if (item.amount > 0)
            {
                Transform t = (Transform)Instantiate(icon);
                t.GetComponent<ItemToIcon>().type = item.t;
                t.position = new Vector3(transform.position.x + (itemIndex - itemCount / 2) * icon.GetComponent<SpriteRenderer>().bounds.size.x  / 2, transform.position.y + transform.GetComponent<SpriteRenderer>().bounds.size.y / 2, transform.position.z);
                t.SetParent(transform);
                itemIndex++;
            }
        }
    }

	/* This function will destroy the mummy's inventroy.
	 * 
	 * 
 	*/
    private void KillInventory()
    {
        while (transform.childCount > 0)
        {
            Transform t = transform.GetChild(0);
            t.transform.SetParent(null);
            Destroy(t.gameObject);
        }
    }
}
