using UnityEngine;
using System.Collections;

/// <summary>
/// Interactable.
/// </summary>
public class Interactable : MonoBehaviour
{
	/// <summary>
	/// Raises the mouse down event.
	/// </summary>
    public void OnMouseDown()
    {
		// I think this check is obsolute.
        if (Input.GetKey("mouse 0"))
        {
            GameObject[] mummies = GameObject.FindGameObjectsWithTag("Mummy");
            foreach (GameObject mummy in mummies)
            {
                Mummy m = mummy.GetComponent<Mummy>();
                if (m.isSelected)
                {
                    Action(m);
                    if (m.marker)
                        StartCoroutine("WaitAndMove", m.marker);
                }
            }
        }
    }
	/*
	 * Input:
	 * Transform marker - The marker transform. It will be given a new position.
	*/ 
    IEnumerator WaitAndMove(Transform marker)
    {
        yield return new WaitForEndOfFrame();
        marker.position = new Vector3(transform.position.x, transform.position.y + transform.GetComponent<SpriteRenderer>().bounds.size.y / 2, 0);
    }

	/*
	 * Every class that inherits from Iteractable must have an Action function.
	 * Input:
	 * Mummy m - The mummy that performs the Action.
	*/
    public virtual void Action(Mummy m)
    {

    }
	/* 
	 * Every class that inherits from Iteractable must have an Interact function.
	 * Input:
	 * Tranform interactorTransform - The Transform of the mummy interacting with the Interactable.
	*/
	public virtual void Interact(Transform interactorTransform = null)
    {
        
    }
}

