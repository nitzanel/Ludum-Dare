using UnityEngine;
using System.Collections;

public class Furnace : Interactable
{
    public float startTime = 60f;
    public float timeLeft = 60f;

    private Transform bar;

	void Start ()
    {
        timeLeft = startTime;
        bar = transform.GetChild(0);
	}
	
    void Update()
    {
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0)
        {
            timeLeft = 0;
            Application.LoadLevel("GameOver");
        }
        bar.localScale = new Vector3(bar.localScale.x,  timeLeft / startTime, bar.localScale.z);
    }

    public override void Action(Mummy m)
    {
        m.AddTask(transform.position, Task.action.FURNACE, this);
    }

	public override void Interact(Transform interactorTransform = null)
    {
        timeLeft = startTime;
    }

    /*
    public void OnMouseDown()
    {
        if (Input.GetKey("mouse 0"))
        {
            GameObject[] mummies = GameObject.FindGameObjectsWithTag("Mummy");
            foreach (GameObject mummy in mummies)
            {
                Mummy m = mummy.GetComponent<Mummy>();
                if (m.isSelected)
                {
                    m.AddTask(transform.position);
                    if (m.marker)
                        StartCoroutine("WaitAndMove", m.marker);
                }
            }
        }
    }

    IEnumerator WaitAndMove(Transform marker)
    {
        yield return new WaitForEndOfFrame();
        marker.position = new Vector3(transform.position.x, transform.position.y + transform.GetComponent<SpriteRenderer>().bounds.size.y / 2, 0);
    }
     * */
}
