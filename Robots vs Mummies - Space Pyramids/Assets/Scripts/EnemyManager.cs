using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// REWRITE PROBABLY NEEDED
/// </summary>
public class EnemyManager : MonoBehaviour
{
    public Transform nasa = null;
    public int nasaDaployTime = 120;
    private float nasaSpeed = 0.1f;

    public float pyramidMoveSpeed = 0.1f;
    public float movePyramidBy = 0.2f;

    private float time = 0;
    private bool didSpawnNasa = false;

    private Transform pyramid = null;

    void Start ()
    {
        if (nasa == null) Debug.LogError("ERROR: NASA not assigned to enemy manager");

        pyramid = GameObject.Find("EverythingInPyramid").transform;
        if (pyramid == null) Debug.LogError("ERROR: Pyramid not found");
	}
	
	void Update ()
    {
        time += Time.deltaTime;
        if (time > nasaDaployTime && !didSpawnNasa)
        {
            //pyramid.position = new Vector3(pyramid.position.x + movePyramidBy, pyramid.position.y, pyramid.position.z);
            StartCoroutine("MovePyramid", new Vector3(pyramid.position.x + movePyramidBy, pyramid.position.y, pyramid.position.z));
            Instantiate(nasa, new Vector3(pyramid.position.x - movePyramidBy - 1.2f, pyramid.position.y - 2, pyramid.position.z), Quaternion.identity);
            didSpawnNasa = true;
        }
	}


	// Nitzan's comment: why do we need this function?
	/*
	 * This function moves the player's pyramid.
	 * Input:
	 * Vector3 destination - the new position of the player's pyramid.
	*/
    IEnumerator MovePyramid (Vector3 destination)
    {
        while (Vector3.Distance(transform.position, destination) > 0.05f)
        {
            pyramid.position = Vector3.Lerp(pyramid.position, destination, pyramidMoveSpeed * Time.deltaTime);

            yield return null;
        }
    }
}
