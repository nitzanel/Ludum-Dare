using UnityEngine;
using System.Collections;

/// <summary>
/// TASK TASK TASK
/// </summary>
[System.Serializable]
public class Task
{
	public Vector3 destination;
	public enum action { WALK, FURNACE, WOOD, WAKE_THE_DEAD };
	public action a;
	public Interactable calledMe;

	public Task(Vector3 Destination, action A = action.WALK, Interactable CalledMe = null)
	{
		destination = Destination;
		a = A;
		calledMe = CalledMe;
	}
}
/// <summary>
/// END END END
/// </summary>
