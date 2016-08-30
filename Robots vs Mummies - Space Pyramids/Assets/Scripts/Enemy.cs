using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    public List<Transform> soldiers; //list of prefabs the enemy will spawn (can be empty)
    public float spawnInterval = 5;
    public float attackInterval = 2;
    public float moveSpeed = 0.1f;
	public bool spawned = false; // A boolean representing the current state of the enemy.
	public int hp = 10;

    protected float time = 0;

	void Start ()
    {
	
	}
	
	void Update ()
    {
        if ((time += Time.deltaTime) >= spawnInterval) Spawn();
		if (spawned)
		{
			Attack ();
			Hit();
		}
	}

    protected virtual void Spawn()
    {
		spawned = true;
    }
		
    protected virtual void Attack()
    {

    }

    public virtual void Hit()
    {

    }
}
