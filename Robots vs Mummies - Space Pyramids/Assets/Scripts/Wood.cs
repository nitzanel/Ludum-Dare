using UnityEngine;
using System.Collections;

public class Wood : Interactable
{
    public override void Action(Mummy m)
    {
        m.AddTask(transform.position, Task.action.WOOD);
    }

    public override void Interact()
    {
        
    }
}
