using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// The PlayerManager class will know what characters the player select, and manage the selection.
/// </summary>
public static class PlayerManager 
{
	public static Mummy currentlySelected = null;

	/* Input:
	 * A mummy clicked on by the player.
	 * The function will change the currently selected character.
	*/
	public static void SetCurrentlySelected(Mummy mummy)
	{
		if (currentlySelected == mummy)
		{
			mummy.isSelected = false;
			currentlySelected = null;


		}
		else 
		{
			mummy.SelectMummy (true);
			if (currentlySelected != null) currentlySelected.SelectMummy(false);
			currentlySelected = mummy;
		}
	}
		


}


