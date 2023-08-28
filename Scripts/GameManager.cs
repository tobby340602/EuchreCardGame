using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	#region Public Members
	public static CardAnimationHandler cardAnimator => instance.cardAnimationHandler;
	public static DeckAsset DeckAsset => instance._deckAsset;
	//public static LayerMasks Masks => instance._masks;
	public static ScoreLinks Scoreboard => instance._scoreLinks;
	public static bool ShowAllCards => instance._showAllCards;
	public static bool HumanPlayer => instance._humanPlayer;
	public static PointSpread DefaultPointSpread => instance.defaultPointSpread;
	public static string[] Players => instance.players;
	public static GeneticHandler GeneticHandler => instance.geneHandler;
	public static StateMachineSystem.StateMachine StateMachines => instance.stateMachines;
	static AudioSource AudioSourceCard => instance.audioSource;
	static AudioClip AudioSourcePlace => instance.placeAudio;
	static AudioClip AudioSourceDeal => instance.dealAudio;

    #endregion

    #region Private Members
    private static GameManager instance = null;
    private StateMachineSystem.StateMachine stateMachines;
    #endregion

    #region Serialized Members
    [SerializeField]
	private bool _showAllCards = false;
	[SerializeField]
	private bool _humanPlayer = true;
	[SerializeField]
	private CardAnimationHandler cardAnimationHandler = null;
	[SerializeField]
	private GameObject cardHolder = null;
	[SerializeField]
	private GameObject trumpSelectorUI = null;
    [SerializeField]
    private string[] players;
	//private LayerMasks _masks;
    #region Static Linked
	
	
	private ScoreLinks _scoreLinks = null;
	[SerializeField]
	private DeckAsset _deckAsset = null;

	//[SerializeField]
	//[HideInInspector]
	//private DynamicContainer _dynamicContainer = new DynamicContainer();
	#endregion

	#region Spawned Objects
	[SerializeField]
	private GameObject cardPrefab = null;
	[SerializeField]
	private GameObject playerPrefab = null;
	[SerializeField]
	private GameObject[] trumpStamps = null;
	[SerializeField]
	private Transform trumpStampPot = null;	
	[SerializeField]
	private GameObject passSticker = null;
	[SerializeField]
	private GameObject callSticker = null;
	[SerializeField]
	private GameObject aloneSticker = null;
	[SerializeField]
	private PointSpread defaultPointSpread = default;
	[SerializeField]
	private GeneticHandler geneHandler = new GeneticHandler();
	[SerializeField]
	private AudioClip placeAudio = null;
	[SerializeField]
	private AudioClip dealAudio = null;

	private AudioSource audioSource = null;
	#endregion

	#endregion

	// Start is called before the first frame update
	void Start() {
		if (instance != null) {
			Debug.Log("Multiple game managers in scene.");
			Debug.Break();
		}
		instance = this;
		instance.audioSource = GetComponent<AudioSource>();
		instance._scoreLinks = GetComponent<ScoreLinks>();
		stateMachines = new StateMachineSystem.StateMachine();
		stateMachines.Transition<StateMachineSystem.EuchreStates.SetupGame>();
	}



	/// <summary>
	/// Returns locations to spawn player hands at. (Z is euler angle)
	/// </summary>
	/// <param name="playerAmt"></param>
	/// <returns></returns>
	public static Vector3[] GetHandLocations(int playerAmt) {
		Vector3[] ret = new Vector3[playerAmt];
		float circleSize = 11;
		float step = (360.0f / (float)playerAmt);
		for (int i = playerAmt - 1; i >= 0; i--) {
			float angle = ((step * i));
			//Bound angle in the positive
			if (angle < 0.0f) angle += 360.0f;
			if(i % 2 == 0)
                ret[i] = new Vector3(2 * circleSize * Mathf.Cos(angle * Mathf.Deg2Rad), circleSize * Mathf.Sin(angle * Mathf.Deg2Rad), angle);
			else
	            ret[i] = new Vector3(circleSize * Mathf.Cos(angle * Mathf.Deg2Rad), circleSize * Mathf.Sin(angle * Mathf.Deg2Rad), angle);
		}
		return ret;
	}
	
	public static void playDealAudio()
	{
		AudioSourceCard.clip = AudioSourceDeal;
		AudioSourceCard.Play();
    }

	public static void playPlaceAudio()
    {
        AudioSourceCard.clip = AudioSourcePlace;
        AudioSourceCard.Play();
    }

    public static Player SpawnPlayerPrefab() {
		return Instantiate(instance.playerPrefab).GetComponent<Player>();
	}

	public static Card SpawnCardPrefab(CardAsset cardAsset) {
		Card spawnedCard = Instantiate(instance.cardPrefab).GetComponent<Card>();
		spawnedCard.transform.parent = instance.cardHolder.transform;
		spawnedCard.Initialize(cardAsset);
		return spawnedCard;

	}

	public static TrumpSelector GetTrumpSelector() {
		return instance.trumpSelectorUI.GetComponent<TrumpSelector>();
	}


	public static GameObject SpawnTrumpIndicator(Card.Suit suit, int team) {
		GameObject spawnedObject = Instantiate(instance.trumpStamps[(int)suit], instance.trumpStampPot);
		return spawnedObject;
	}


	/// <summary>
	/// types 1==Passing, 2==Calling, 3==Alone
	/// </summary>
	/// <param name="playerID"></param>
	/// <param name="type"></param>
	/// <returns></returns>
	public static GameObject SpawnAIText(int playerID, int type, StateMachineSystem.StateMachine targetMachine) {
		GameObject spawnedOject;
		if (type == 1)
			spawnedOject = Instantiate(instance.passSticker);
		else if (type == 2)
			spawnedOject = Instantiate(instance.callSticker);
		else
			spawnedOject = Instantiate(instance.aloneSticker);

		spawnedOject.transform.position = Vector3.Lerp(Vector3.zero, targetMachine.Memory.GetData<Player>("Player"+playerID).gameObject.transform.position,0.7f);
		if (type == 3)
			spawnedOject.transform.position += Vector3.down;

		return spawnedOject;
	}

	public static GameManager AccessInstance() {
		return instance;
	}

/*
	[System.Serializable]
	public class LayerMasks
	{
		public LayerMask notCards;
	}*/

}
