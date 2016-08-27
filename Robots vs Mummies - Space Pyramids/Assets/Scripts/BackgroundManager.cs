using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BackgroundManager : MonoBehaviour
{
    public float speed = 0.25f;
    public Transform stars = null;

    private List<Transform> allStars;
    private float bottomLimit;
    private int yIterations;
    private float height;

	void Start ()
    {
        allStars = new List<Transform>();
        SpawnStars();

        if (stars == null) Debug.LogError("ERROR: No stars linked to BackgroundManager");
	}
	
	void Update ()
    {
	    foreach (Transform star in allStars)
        {
            if (star.position.y <= bottomLimit)
            {
                star.position = new Vector3(star.position.x, star.position.y + yIterations * height, star.position.z);
            }
            else
            {
                star.position = new Vector3(star.position.x, star.position.y - speed * Time.deltaTime, star.position.z);
            }
        }
	}

    private void SpawnStars()
    {
        Vector3 bottomLeftCorner = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, -Camera.main.transform.position.z));
        Vector3 bottomRightCorner = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, -Camera.main.transform.position.z));
        Vector3 topLeftCorner = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, -Camera.main.transform.position.z));
        float width = stars.GetComponent<SpriteRenderer>().bounds.size.x;
        height = stars.GetComponent<SpriteRenderer>().bounds.size.y;
        int xIterations = Mathf.CeilToInt((bottomRightCorner.x - bottomLeftCorner.x) / width);
        yIterations = Mathf.CeilToInt((topLeftCorner.y - bottomLeftCorner.y) / height) + 1;
        bottomLimit = bottomLeftCorner.y - height / 2;

        for(int i = 0; i <= xIterations; i++)
        {
            for(int j = 0; j <= yIterations; j++)
            {
                allStars.Add((Transform)Instantiate(stars, new Vector3(bottomLeftCorner.x + i * width, bottomLeftCorner.y + j * height, 0), Quaternion.identity));
            }
        }
    }
}
