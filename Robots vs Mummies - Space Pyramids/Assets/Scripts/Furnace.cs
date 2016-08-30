using UnityEngine;
using System.Collections;

/// <summary>
/// Furnace.
/// </summary>
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
		
}
