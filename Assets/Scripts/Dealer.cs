using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages creation and organization of cards in the game.
/// </summary>
public class Dealer : GameBase {

    public GameObject cardPrefab;
    public int challengeNumber;

    private GameObject cardAux;
    private Card cardAuxInfo;
    [SerializeField]
    private List<GameObject> cardsInGame;
    [SerializeField]
    private List<GameObject> challengeCards;
    [SerializeField]
    private GameObject objectiveCard;

    [Space]
    public int timeDelay, screenRadius;

    // Use this for initialization
	void Start () 
    {
        for (int n_suit = 0; n_suit < suitNames.Length; n_suit++)
        {
            for (int n_value = 0; n_value < valueNames.Length; n_value++)
            {
                cardAux = CreateCard(n_suit, n_value);
                cardsInGame.Add(cardAux);
                //cardAux.transform.position = new Vector3(-0.01f * n_value - 0.13f * n_suit, 0f, 0f);
                cardAuxInfo = cardAux.GetComponent<Card>();
                cardAuxInfo.MoveCard(new Vector3(-0.01f * n_value - 0.13f * n_suit, 0f, 0f));
            }
        }
        int count = 0;
        for (int n_challenge = 0; n_challenge < challengeNumber; n_challenge++)
        {
            int randNum = Random.Range(0, cardsInGame.Count - 1);
            float angShared = 2f * Mathf.PI / challengeNumber;
            cardAux = cardsInGame[randNum];
            challengeCards.Add(cardAux);
            cardAuxInfo = cardAux.GetComponent<Card>();
            cardAuxInfo.MoveCard(new Vector3(0f, 3f * Mathf.Cos(angShared * count), 3f * Mathf.Sin(angShared * count)), 0.75f, count * 0.25f);
//            cardAux.transform.position = new Vector3(0f, 3f * Mathf.Cos(angShared * count), 3f * Mathf.Sin(angShared * count));
            cardsInGame.Remove(cardAux);
            count++;
        }
        cardAux = challengeCards[Random.Range(0, challengeCards.Count - 1)];
        cardAuxInfo = cardAux.GetComponent<Card>();
        objectiveCard = CreateCard(cardAuxInfo.suit, cardAuxInfo.value);
//        objectiveCard.transform.position = new Vector3(0.3f, 0f, 0f);
        cardAuxInfo = objectiveCard.GetComponent<Card>();
        cardAuxInfo.MoveCard(new Vector3(-1.5f, 0f, 0f));
        cardAuxInfo.MoveCard(new Vector3(0.3f, 0f, 0f), 0.5f, count * 1.0f);

	}
	
	// Update is called once per frame
	void Update () 
    {
		
	}

    #region Create card functions
    /// <summary>
    /// Creates a generic the card.
    /// </summary>
    /// <returns>The card.</returns>
    private GameObject CreateCard()
    {
        GameObject cardAux;

        cardAux = Instantiate(cardPrefab);    // Creates a game object based on cardBody
        cardAux.name = "Card_Default"; // + cardNames[i_card] + "_" + suitNames[i_suit];
//        cardAux.tag = "PickableCard";
        cardAux.transform.parent = this.transform;
        cardAux.layer = this.gameObject.layer;

        cardAux.transform.position = new Vector3(0f, 0f, 0f);
        cardAux.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));

        return cardAux;
    }

    /// <summary>
    /// Creates a card based on its informations.
    /// </summary>
    /// <returns>The card created.</returns>
    /// <param name="suit">Suit of card.</param>
    /// <param name="value">Value of card.</param>
    /// <param name="color">Color of card.</param>
    private GameObject CreateCard(int suit, int value)
    {
        GameObject cardAux;
        Card cardBodyAux;

        cardAux = CreateCard();
        cardAux.name = "Card_" + valueNames[value] + "_" + suitNames[suit];
        cardBodyAux = cardAux.GetComponent<Card>();
        cardBodyAux.UpdateInfos(suit, value);
        return cardAux;
    }
    #endregion


}
