using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreLinks : MonoBehaviour {
	public TextMeshPro playerScore = null, opponentScore = null, playerTrickScore = null, opponentTrickScore = null, caller = null;
     
	public void UpdateScore(int team, int score) {
		if (team == 0)
			playerScore.text = "" + score;
		else
			opponentScore.text = "" + score;
	}
    public void UpdateTrickScore(int team, int score)
    {
        if (team == 0)
            playerTrickScore.text = "" + score;
        else
            opponentTrickScore.text = "" + score;
    }

    public void UpdateCaller(string text)
    {
       caller.text = text;
    }
}
