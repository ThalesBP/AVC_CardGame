using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages creation and organization of cards in the game.
/// </summary>
public class Dealer : GameBase {

    public GameObject cardPrefab;   // Card prefab to be instantiated
    public int challengeNumber;     // Number of card in challange - It may be useless

    // Aux variables
    private Card cardAux;
    private int packCounter;
    private Card aimedCard;
    [SerializeField]
    private bool onCard;

    // Cards in the game    
    [SerializeField]
    private List<Card> cardsInGame, challengeCards;
    [SerializeField]
    private Card objectiveCard;

    private LayerMask cardMask;


    // Use this for initialization
	void Start () 
    {
        cardMask = LayerMask.GetMask("Card");

        challengeCards = CreateDeck(challengeNumber);
        onCard = false;

        PackCards(challengeCards, 0f, 0f);
        HideCards(challengeCards, 0f, 0f);

        cardAux = ChooseCard(challengeCards);
        objectiveCard = CreateCard(cardAux);

        PackCard(objectiveCard, 0f);
        HideCard(objectiveCard, 0f);

        SpreadCards(challengeCards);
        ShowCards(challengeCards, delay_M, move_M);

        objectiveCard.position.MoveTo(1.5f * Vector3.back, move_M, challengeCards.Count * delay_M);
        ShowCard(objectiveCard, challengeCards.Count * delay_M + move_M);
    }
	
	// Update is called once per frame
	void Update () 
    {
        ControlMouseInteraction();
	}

    /// <summary>
    /// Controls mouse interaction
    /// </summary>
    private void ControlMouseInteraction()
    {
        Ray camRay = Camera.main.ScreenPointToRay (Input.mousePosition);

        RaycastHit cardHit;

        if (Physics.Raycast(camRay, out cardHit, 50f, cardMask))
        {
            if (!onCard)
            {
                onCard = true;
                foreach (Card challange in challengeCards)
                {
                    if (cardHit.transform.parent.transform == challange.transform)
                    {
                        aimedCard = challange;
                        aimedCard.scale.MoveTo(Vector3.one * 1.1f, 0.2f);

                        // Only for test
                        objectiveCard.UpdateInfos(aimedCard);
                    }
  //                  else
 //                       challange.scale.MoveTo(Vector3.one, 0.1f);
                }

                //          if (underMouse != null)
                //                objective.UpdateInfos(underMouse.suit, underMouse.value);
                //          if (!Input.GetMouseButton(0))
            }
        }
        else
        {
            onCard = false;
            if (aimedCard != null)
            {
                aimedCard.scale.MoveTo(Vector3.one, 0.1f);
                aimedCard = null;
            }
        }
    }

    #region Create card functions
    /// <summary>
    /// Creates a random card.
    /// </summary>
    /// <returns>The card.</returns>
    private Card CreateCard()
    {
        return CreateCard(Random.Range(0, suitNames.Length), Random.Range(0, valueNames.Length));
    }

    /// <summary>
    /// Creates a card based on its informations.
    /// </summary>
    /// <returns>The card created.</returns>
    /// <param name="suit">Suit of card.</param>
    /// <param name="value">Value of card.</param>
    /// <param name="color">Color of card.</param>
    private Card CreateCard(int suit, int value)
    {
        GameObject cardObjAux;
        Card cardAux;

        cardObjAux = Instantiate(cardPrefab);    // Creates a game object based on cardBody
        cardObjAux.transform.parent = this.transform;
//        cardObjAux.layer = this.gameObject.layer;

        cardObjAux.transform.position = new Vector3(0f, 0f, 0f);
        cardObjAux.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));

        cardObjAux.name = "Card_" + valueNames[value] + "_" + suitNames[suit];
//        cardObjAux.tag = "Card";
        cardAux = cardObjAux.GetComponent<Card>();
        cardAux.UpdateInfos(suit, value);
        return cardAux;
    }

    /// <summary>
    /// Creates the card based on another card information.
    /// </summary>
    /// <returns>The card copied.</returns>
    /// <param name="card">The card to be copied.</param>
    private Card CreateCard(Card card)
    {
        return CreateCard(card.suit, card.value);
    }

    #endregion

    #region Create deck functions
    /// <summary>
    /// Creates the deck based in a number of suits and values.
    /// </summary>
    /// <returns>The deck.</returns>
    /// <param name="nSuits">Number of suits.</param>
    /// <param name="nValues">Number of values.</param>
    List<Card> CreateDeck (int nSuits, int nValues)
    {
        List<Card> deckAux;

        deckAux = new List<Card>();

        for (int iSuit = 0; iSuit < nSuits; iSuit++)
        {
            for (int iValue = 0; iValue < nValues; iValue++)
            {
                deckAux.Add(CreateCard(iSuit, iValue));

            }
        }
        return deckAux;
    }

    /// <summary>
    /// Creates a random deck with n cards.
    /// </summary>
    /// <returns>Random deck.</returns>
    /// <param name="nCards">Number of cards.</param>
    List<Card> CreateDeck(int nCards)
    {
        List<Card> deckAux;
        Card cardAux;

        deckAux = new List<Card>();

        for (int iCard = 0; iCard < nCards; )
        {
            cardAux = CreateCard();

            if (!deckAux.Contains(cardAux))
            {
                deckAux.Add(cardAux);
                iCard++;
            }
        }
        return deckAux;

    }

   /* List<Card> CreateDeck (List<int> suitsList, List<int> valuesList)
    {
        List<Card> deckAux;

        deckAux = new List<Card>();

        foreach (int iSuit in suitsList)
        {
            foreach (int iValue in valuesList)
            {
                deckAux.Add(CreateCard(iSuit, iValue));
                deckAux[deckAux.Count - 1].position.MoveTo(new Vector3(-0.01f * iValue - 0.13f * iSuit, 0f, 0f));
                deckAux[deckAux.Count - 1].rotation.MoveTo(new Vector3(0f, 180f, 0f));
            }
        }

        for (int iSuit = 0; iSuit < nSuits; iSuit++)
        {
            for (int iValue = 0; iValue < nValues; iValue++)
            {
                deckAux.Add(CreateCard(iSuit, iValue));
                deckAux[deckAux.Count - 1].position.MoveTo(new Vector3(-0.01f * iValue - 0.13f * iSuit, 0f, 0f));
                deckAux[deckAux.Count - 1].rotation.MoveTo(new Vector3(0f, 180f, 0f));
            }
        }
        return deckAux;
    }*/
    #endregion

    #region Movements manager
    /// <summary>
    /// Hides the card after delay time.
    /// </summary>
    /// <param name="card">Card to be hidden.</param>
    /// <param name="delay">Delay time to be hidden.</param>
    void HideCard(Card card, float delay)
    {
        card.rotation.MoveTo(new Vector3(0f, 180f, 0f), delay);
    }

    /// <summary>
    /// Shows the card after delay time.
    /// </summary>
    /// <param name="card">Card to be showed.</param>
    /// <param name="delay">Delay time to be showed.</param>
    void ShowCard(Card card, float delay)
    {
        card.rotation.MoveTo(Vector3.zero, move_M, delay);
    }

    /// <summary>
    /// Hides a pack of cards.
    /// </summary>
    /// <param name="deck">Deck to be hidden.</param>
    /// <param name="delayStep">Delay step between each hidden movement.</param>
    /// <param name="delay">Delay before start to hidden.</param>
    void HideCards(List<Card> deck, float delayStep, float delay)
    {
        foreach (Card card in deck)
        {
            HideCard(card, deck.IndexOf(card) * delayStep + delay);
        }
    }

    /// <summary>
    /// Shows a pack of cards.
    /// </summary>
    /// <param name="deck">Deck to be showed.</param>
    /// <param name="delayStep">Delay step between each showed movement.</param>
    /// <param name="delay">Delay before start to showed.</param>
    void ShowCards(List<Card> deck, float delayStep, float delay)
    {
        foreach (Card card in deck)
        {
            ShowCard(card, deck.IndexOf(card) * delayStep + delay);
        }
    }


    /// <summary>
    /// Adds a card to the pack.
    /// </summary>
    /// <param name="card">Card to be added.</param>
    void PackCard(Card card, float delay)
    {
        card.position.MoveTo(cardThick * packCounter * Vector3.back, delay);
        packCounter++;
    }

    /// <summary>
    /// Packs the cards of a deck.
    /// </summary>
    /// <param name="deck">Deck to be packed.</param>
    void PackCards(List<Card> deck, float delayStep, float delay)
    {
        foreach (Card card in deck)
            PackCard(card, deck.IndexOf(card) * delayStep + delay);
    }

    /// <summary>
    /// Spreads the cards in a circle around screen.
    /// </summary>
    /// <param name="deck">Deck to be spread</param>
    void SpreadCards(List<Card> deck)
    {
        float angShare;

        angShare = 2f * Mathf.PI / deck.Count;
        foreach (Card card in deck)
        {
            card.position.MoveTo(new Vector3(spreadRadius * Mathf.Sin(angShare * deck.IndexOf(card)), spreadRadius * Mathf.Cos(angShare * deck.IndexOf(card)), 0), move_M, deck.IndexOf(card) * delay_M);
        }
    }
    #endregion

    #region Deck manager
    /// <summary>
    /// Chooses a random card from a deck (does not remove from it).
    /// </summary>
    /// <returns>The random card.</returns>
    /// <param name="deck">Deck to have a random card picked.</param>
    Card ChooseCard(List<Card> deck)
    {
        Card randomCard;

        randomCard = deck[Random.Range(0, deck.Count - 1)];

        return randomCard;
    }

    /// <summary>
    /// Picks a random card from a deck.
    /// </summary>
    /// <returns>The random card.</returns>
    /// <param name="deck">Deck to have a random card picked.</param>
    Card PickCard(List<Card> deck)
    {
        Card randomCard;

        randomCard = ChooseCard(deck);
        deck.Remove(randomCard);

        return randomCard;
    }

    /// <summary>
    /// Picks n random cards from a deck.
    /// </summary>
    /// <returns>The random cards picked.</returns>
    /// <param name="deck">Deck to have random cards picked.</param>
    /// <param name="nCards">Number cards picked.</param>
    List<Card> PickCards(List<Card> deck, int nCards)
    {
        List<Card> pickedCards = new List<Card>();

        if (nCards > deck.Count)
        {
            Debug.Log("nCards is bigger than number of cards in the deck");
            nCards = deck.Count;
        }

        for (int iCard = 0; iCard < nCards; iCard++)
        {
            pickedCards.Add(PickCard(deck));
        }
        return pickedCards;
    }
    #endregion

}
