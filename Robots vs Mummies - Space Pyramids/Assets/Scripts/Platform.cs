using UnityEngine;
using System.Collections;

public class Platform : MonoBehaviour
{
    public int level = 0;
    public Transform ladderUp = null;
    public Transform ladderDown = null;
	/// <summary>
	/// Raises the trigger enter2 d event.
	/// </summary>
    void OnTriggerEnter2D(Collider2D other)
    {
        Mummy m = other.transform.GetComponent<Mummy>();
        if (m) m.ChangeDirection();
    }
}
