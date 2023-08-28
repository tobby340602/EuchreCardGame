using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.UI.GridLayoutGroup;

public class CardAnimationHandler : MonoBehaviour {
	
	/// <summary>
	/// Animates the movement of a card
	/// </summary>
	/// <param name="pos"></param>
	/// <param name="target"></param>
	/// <returns></returns>
	public IEnumerator FlyTo(Vector3 pos, Card target) {
		while (target.animating)
			yield return null;
		pos.z = 0;
		float speed = 3f;
        target.animating = true;

		while ((target.transform.position - pos).magnitude > 0) {
			Vector3 dir = (pos - target.transform.position);
			if (dir.magnitude > speed)
				dir = dir.normalized * speed;
			target.transform.position += dir;
			yield return null;
		}

	//	if (audio && GameManager.PlayAudio)
		//	FMODUnity.RuntimeManager.PlayOneShot("event:/cardPlace" + Random.Range(1, 4), target.transform.position);

		target.transform.position = pos;
		//yield return null;
		target.animating = false;
	}

    /// <summary>
    /// Draws a number of cards from the game deck and animates their arrival to the player's hand
    /// </summary>
    /// <param name="playerName"></param>
    /// <param name="deckName"></param>
    /// <param name="amt"></param>
    /// <returns></returns>
    public IEnumerator Deal(string playerName, string deckName, int amt, StateMachineSystem.StateMachine targetMachine) {
		if (amt == 0)
			yield break;

		Card[] drawn = targetMachine.Memory.GetData<Deck>(deckName).Draw(amt);
		Player p = targetMachine.Memory.GetData<Player>(playerName);
		
		for (int i = 0; i < drawn.Length; i++) {
			Card card = drawn[i];
			//StartCoroutine(Orient(card, p.gameObject.name, targetMachine));
            p.AddCard(card);

			card.gameObject.transform.rotation = Quaternion.Euler(0.0f, card.transform.eulerAngles.y, p.gameObject.transform.rotation.eulerAngles.z + 90.0f);			
		}
		p.AssertHandSpriteOrdering();
		
			/*foreach (Card c in drawn) 
				while (c.animating)
					yield return null;
		*/
	//	if (GameManager.PlayAudio)
	//		FMODUnity.RuntimeManager.PlayOneShot("event:/cardSlide" + Random.Range(1,4), drawn[0].transform.position);

		//This handles cards moving to the player's hand from the deck as well
		yield return AdjustHand(playerName, targetMachine);
        for (int i = 0; i < drawn.Length; i++)
        {
            if (p.isHuman || GameManager.ShowAllCards)
			{
				GameManager.cardAnimator.Flip(drawn[i]);

            }

            //StartCoroutine(Flip(drawn[i]));
        }
    }

	/// <summary>
	/// Orients a card to line up with a player's hand. if string is empty or null it orients with the screen.
	/// </summary>
	/// <param name="target"></param>
	/// <param name="playerName"></param>
	/*/// <returns></returns>
	public IEnumerator Orient(Card target, string playerName, StateMachineSystem.StateMachine targetMachine) {
			while (target.animating)
				yield return null;
		target.animating = true;
	
		float orientation = (string.IsNullOrEmpty(playerName)) ? 0.0f : targetMachine.Memory.GetData<Player>(playerName).gameObject.transform.eulerAngles.z + 90.0f;
		Quaternion goalRot = Quaternion.Euler(0.0f, target.transform.eulerAngles.y, orientation);

			while ((goalRot.eulerAngles - target.transform.rotation.eulerAngles).magnitude > 0.1f) {
				target.transform.rotation = Quaternion.Lerp(target.transform.rotation, goalRot, 0.4f);
				yield return null;
			}
		
		
		//Enforce final position
		target.transform.rotation = goalRot;
		//End animation
		target.animating = false;
	}
*/

	/// <summary>
	/// Adjusts the cards within the player's hand to match calucalted positions
	/// </summary>
	/// <param name="playerName"></param>
	/// <returns></returns>
	public IEnumerator AdjustHand(string playerName, StateMachineSystem.StateMachine targetMachine) {
		Player p = targetMachine.Memory.GetData<Player>(playerName);
		p.AssertHandSpriteOrdering();
		Vector3[] pos = GetCardPlacementOffsets(p.gameObject, p.GetHand().Count);

		//Move all cards
		for (int i = 0; i < p.GetHand().Count; i++) {
			p.GetHand()[i].goalPosition = pos[i];
			StartCoroutine(FlyTo(pos[i], p.GetHand()[i]));
			if(p.isHuman)
				p.GetHand()[i].transform.localScale = Vector3.one * 1.5f;
		}

		//Wait for all sub animations to finish
			for (int i = 0; i < p.GetHand().Count; i++) 
				while (p.GetHand()[i].animating)
					yield return null;
			
	}

	public void ShakeRand(Card card)
	{	
		card.transform.rotation = Quaternion.Euler(0, card.transform.rotation.eulerAngles.y, 0);
        card.transform.Rotate(0, 0, Random.Range(-30, 30));
    }

    /// <summary>
    /// Animates the flipping of a card and chances its face-position value
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public void Flip(Card target) {
        target.transform.Rotate(0.0f, 180, 0.0f);
		target.faceDown = !target.faceDown;
	}


	/// <summary>
	/// Calculates placement and rotation for a set of cards in a location. (Z is rotation)
	/// </summary>
	/// <returns></returns>
	public Vector3[] GetCardPlacementOffsets(GameObject handObject, int amt) {
		Vector3[] ret = new Vector3[amt];
		//How wide we assume a card is
		float cardWidth;
		//Center point of this hand
		Vector3 point = handObject.transform.position;
		//Euler angle this hand rests on at the 'circular table'
		float eulerDir = handObject.transform.rotation.eulerAngles.z;
		if (handObject.GetComponent<Player>().isHuman)
			cardWidth = 4f;
		else
			cardWidth = 2f;
		
		//Calculate the normal facing away from the circle
		Vector3 normal = new Vector3(Mathf.Cos(eulerDir * Mathf.Deg2Rad), Mathf.Sin(eulerDir* Mathf.Deg2Rad),0.0f);
		//Make it face inward
		normal *= -1f;
		Vector3 horizAxis = Vector3.Cross(new Vector3(0,0,1), normal);

		float step = 1.0f / (float)amt; 
		Vector3 offset = horizAxis * cardWidth * (amt / 2.0f);

		for (int i = 0; i < amt; i++) {
			ret[i] = Vector3.Lerp(point + offset, point - offset, step * i) - (horizAxis * (cardWidth / 2.0f));// - ((amt % 2 != 0) ?  : Vector3.zero);
			ret[i].z = 0.0f;
			Debug.DrawRay(ret[i], normal, Color.green);
		}

		return ret;
	}
}
