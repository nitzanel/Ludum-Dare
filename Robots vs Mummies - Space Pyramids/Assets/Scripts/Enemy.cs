using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    public List<Transform> soldiers; //list of prefabs the enemy will spawn (can be empty)
    public float spawnInterval = 5;
    public float attackInterval = 2;
    public float speed = 0.1f;
    public int hp = 10;

    protected float time = 0;

	void Start ()
    {
	
	}
	
	void Update ()
    {
        if ((time += Time.deltaTime) >= spawnInterval) Spawn();
	}

    protected virtual void Spawn()
    {

    }

    protected virtual void Attack()
    {

    }

    public virtual void Hit()
    {

    }
}
