using UnityEngine;
using System.Collections;

/// <summary>
/// The Coffin class.
/// 
/// </summary>

public class Coffin : Interactable
{
	// cooldown between spawns of mummies.
	public float startTime = 30f;
	// current time left to wait.
	public float timeLeft = 30f;

	// the mummy the coffin will create.
	public GameObject sleepy_mummy;
	// the sprite the coffin will use while the mummy isn't ready.
	public Sprite coffin_empty;
	// the sprite the coffin will use when the mummy is ready.
	public Sprite coffin_full;

	// the cooldown bar transform.
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
	/*
	* This function adds a task to the mummy.
	* The function is called automaticly.
	* input:
	* Mummy m - the mummy that perform the action.
	*/ 
	public override void Action(Mummy m)
	{
		m.AddTask (transform.position, Task.action.WAKE_THE_DEAD, this);
	}

	/*
	 * The interaction function.
	 * The funciton will create a new mummy if the timeLeft veriable is zero.
	 * Input:
	 * Transform interactorTransform - The Transform of the mummy that does the interaction.
	*/
	public override void Interact(Transform interactorTransform= null)
	{
		if (interactorTransform == null)
		{
			Debug.LogError ("The Interact function of Coffin has been called with a null Transform.");
		}

		if (timeLeft <=0) 
		{
			spriteRenderer.sprite = coffin_empty;
			Instantiate (sleepy_mummy, new Vector3(transform.position.x,interactorTransform.position.y,interactorTransform.position.z), Quaternion.identity);
            transform.GetComponent<AudioSource>().Play();
            timeLeft = startTime;
		}
	}

}


