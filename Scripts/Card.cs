﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour {

	public enum Suit { Hearts, Diamonds, Spades, Clubs}
	public enum Value { Two = 1, Three = 2, Four=3, Five=4, Six=5, Seven=6, Eight=7, Nine=8, Ten=9, Jack=10, Queen=11, King=12, Ace=13 }

	public CardAsset data;

	public Value value => data.value;
	public Suit suit => data.suit;

	[SerializeField]
	private SpriteRenderer frontRenderer = null;

	[SerializeField]
	private SpriteRenderer backRenderer = null;

	public bool animating = false;

	public Player owner = null;

	public int storedOrdering = 0;

	/// <summary>
	/// Where this card wants to be when not being interacted with by a player. Used for returning the card where it should be after player interaction.
	/// </summary>
	public Vector3 goalPosition;

	/// <summary>
	/// Tracks the current visible face of the card
	/// </summary>
	public bool faceDown = true;


	float clickTimeStamp = Mathf.Infinity;
	float clickThreshold = 0.2f;

	public void Initialize(CardAsset data, Sprite cardBack = null) {
		this.data = data;

		frontRenderer.sprite = data.sprite;

		if (cardBack != null)
			backRenderer.sprite = cardBack;

		this.gameObject.name = Shortname();
	}

	public void SetOrdering(int o) {
		frontRenderer.sortingOrder = o;
		backRenderer.sortingOrder = o;
	}

	public string Shortname() {
		return value.Shortname() + suit.Shortname();
	}

	public void OnMouseDown() {
		if (owner != null && owner.isHuman) {
			clickTimeStamp = Time.time;
		}
	}

	public void OnMouseUp() {
		if (owner != null && owner.isHuman) {
			if (Time.time - clickTimeStamp <= clickThreshold)
				this.PostNotification("MouseClickCard", this);
			//else
			//	this.PostNotification("MouseUpCard", this);
		}
	}
/*
	public void OnMouseOver() {
		if (owner != null && owner.isHuman)
			this.PostNotification("MouseOverCard", this);
		//
	}*/
/*
	public void OnMouseExit() {
		if (owner != null && owner.isHuman)
			this.PostNotification("MouseExitCard", this);
	}*/
/*
	public void OnMouseEnter() {
		if (owner != null && owner.isHuman)
			this.PostNotification("MouseEnterCard", this);
	}*/
    public void Update()
    {
        if (owner != null && owner.isHuman && Input.mousePosition.y > 0)
		{
			bool over = false;
			List<Card> temp = new List<Card>();
			RaycastHit2D[] hit = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            foreach (RaycastHit2D hit2d in hit)
            {
                if (hit2d.collider.gameObject.tag == "PlayZone")
                {
                    Card card = hit2d.collider.gameObject.GetComponent<Card>();
					temp.Add(card);
                }
            }
			if (temp.Contains(this))
			{
				if(temp.Count == 1)
					over = true;
				else
					foreach(Card card in temp)
					{
						if (card.frontRenderer.sortingOrder < frontRenderer.sortingOrder)
						{ 
							over = true;
							break; 
						}
					}
			}
			if (over)
				this.PostNotification("MouseOverCard", this);
			else
				this.PostNotification("MouseExitCard", this);
        }
	}
}
