using UnityEngine;
using System.Collections;

public class Interactable : MonoBehaviour
{
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
                    Action(m);
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

    public virtual void Action(Mummy m)
    {

    }

	public virtual void Interact(Transform interactorTransform = null)
    {
        
    }
}

public class Item
{
    public enum type { WOOD };
    public int amount;
    public type t;

    public Item(type T, int Amount = 1)
    {
        t = T;
        amount = Amount;
    }
}
