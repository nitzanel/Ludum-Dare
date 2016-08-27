using UnityEngine;
using System.Collections;

public class Platform : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        Mummy m = other.transform.GetComponent<Mummy>();
        if (m) m.ChangeDirection();
    }
}
