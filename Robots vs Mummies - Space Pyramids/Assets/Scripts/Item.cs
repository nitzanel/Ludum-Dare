using System;

/// <summary>
/// The base Item class. All items will inherit from it.
/// </summary>
public class Item
{
	// I think we should put this enum out of the class.
	// Actually, lets put all the enums in one file, so it will be eaier to modify them.
	public enum type { WOOD };
	public int amount;
	public type t;


	/// <summary>
	/// Initializes a new instance of the <see cref="Item"/> class.
	/// </summary>
	/// Input:
	/// type T - the item type.
	/// int Amount - The item Amout. Defaults to 1.
	public Item(type T, int Amount = 1)
	{
		t = T;
		amount = Amount;
	}
}
