using UnityEngine;
using System.Collections;

public class Volume : MonoBehaviour
{
    public float volume = 0.1f;

	// Use this for initialization
	void Start ()
    {
        AudioListener.volume = volume;
	}
}
