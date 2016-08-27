using UnityEngine;
using System.Collections;

public class Platform : MonoBehaviour
{
    public int level = 0;
    public Transform ladderUp = null;
    public Transform ladderDown = null;

    void OnTriggerEnter2D(Collider2D other)
    {
        Mummy m = other.transform.GetComponent<Mummy>();
        if (m) m.ChangeDirection();
    }
}
