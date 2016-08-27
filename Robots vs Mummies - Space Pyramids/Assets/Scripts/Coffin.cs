using UnityEngine;
using System.Collections;

public class Coffin : Interactable
{
	public float startTime = 30f;
	public float timeLeft = 30f;

	public GameObject sleepy_mummy;
	public Sprite coffin_empty;
	public Sprite coffin_full;

	private Transform bar;
	private SpriteRenderer spriteRenderer;
	void Start ()
	{
		timeLeft = startTime;
		bar = transform.GetChild (0);
		spriteRenderer = GetComponent<SpriteRenderer> ();
		spriteRenderer.sprite = coffin_empty;
	}

	void Update()
	{
		timeLeft -= Time.deltaTime;
		if (timeLeft <= 0) 
		{
			timeLeft = 0;
			if (spriteRenderer.sprite != coffin_full) 
			{
				spriteRenderer.sprite = coffin_full;
			}
		}
		bar.localScale = new Vector3 (bar.localScale.x, timeLeft / startTime, bar.localScale.z);
	}

	public override void Action(Mummy m)
	{
		m.AddTask (transform.position, Task.action.WAKE_THE_DEAD, this);
	}

	public override void Interact(Transform interactorTransform= null)
	{
		if (timeLeft <=0) 
		{
			spriteRenderer.sprite = coffin_empty;
			Instantiate (sleepy_mummy, new Vector3(transform.position.x,interactorTransform.position.y,interactorTransform.position.z), Quaternion.identity);
            timeLeft = startTime;
		}
	}

}


