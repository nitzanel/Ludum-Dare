﻿using UnityEngine;
using System.Collections;
/// <summary>
/// Item to icon.
/// </summary>
public class ItemToIcon : MonoBehaviour
{
    public Item.type type = Item.type.WOOD;
    public Sprite wood = null;

	// Use this for initialization
	void Start ()
    {
        switch (type)
        {
            case Item.type.WOOD:
                GetComponent<SpriteRenderer>().sprite = wood;
                break;
        }
	}
}
