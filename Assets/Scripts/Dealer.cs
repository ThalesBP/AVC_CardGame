using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages creation and organization of cards in the game.
/// </summary>
public class Dealer : GameBase {

    public enum Status {newGame, waitingPlayer, endGame, waitingMotion};

    private Status gameStatus;      // Controls the game status
    public GameObject cardPrefab;   // Card prefab to be instantiated
    public int challengeNumber;     // Number of card in challange - It may be useless

    // Aux variables
    private Card cardAux;
    private int packCounter;
    private float waitCounter, timeToWait;
    private Status nextStatus;
    private Card aimedCard;
    [SerializeField]
    private bool onCard;        // This variable may be useless

    // Cards in the game    
    [SerializeField]
    private List<Card> cardsInGame, challengeCards;
    [SerializeField]
    private Card objectiveCard;

    private LayerMask cardMask;


    // Use this for initialization
	void Start () 
    {
        gameStatus = Status.newGame;
        cardMask = LayerMask.GetMask("Card");
        onCard = false;
        waitCounter = 0f;
        timeToWait = 0f;
        }
	
	// Update is called once per frame
	void Update () 
    {
        switch (gameStatus)
        {
            case Status.newGame:
                if (challengeCards.Count > 0)
                {
                    foreach (Card card in challengeCards)
                    {
                        //challengeCards.Remove(card);
                        Destroy(card.gameObject);
                    }
                    Destroy(objectiveCard.gameObject);
                }
                packCounter = 0;

                challengeCards = CreateDeck(challengeNumber);

                //PackCards(challengeCards, 0f, 0f);

                cardAux = ChooseCard(challengeCards);
                objectiveCard = CreateCard(cardAux);

                //PackCard(objectiveCard, 0f);


                SpreadCards(challengeCards);
                ShowCards(challengeCards, DeltaTime[Short], DeltaTime[Long]);

                objectiveCard.position.MoveTo(0.5f * Vector3.back, DeltaTime[Long], challengeCards.Count * DeltaTime[Short]);

                timeToWait = ShowCard(objectiveCard, challengeCards.Count * DeltaTime[Short] + DeltaTime[Long]);
                Wait(timeToWait, Status.waitingPlayer);

                break;
            case Status.waitingPlayer:
                if (!ControlMouseInteraction())
                {
                    Wait(DeltaTime[VeryLong], Status.endGame);
                }
                break;
            case Status.endGame:
                if (Input.GetMouseButton(0))
                {
                    packCounter = 0;

                    objectiveCard.status = Card.Status.free;
                    foreach (Card card in challengeCards)
                    {
                        card.status = Card.Status.free;
                        card.scale.MoveTo(Vector3.one);
                    }

                    HideCard(objectiveCard, 0f);
                    timeToWait = HideCards(challengeCards, 0f, 0f);

                    PackCard(objectiveCard, DeltaTime[Long]);

                    timeToWait += PackCards(challengeCards, DeltaTime[Short], timeToWait);
                    Wait(timeToWait, Status.newGame);
                    challengeNumber++;
                }
                break;
            case Status.waitingMotion:
                if (waitCounter < timeToWait)
                    waitCounter += Time.deltaTime;
                else
                {
                    waitCounter = timeToWait = 0f;
                    gameStatus = nextStatus;
                }
                break;
        }
    }

    /// <summary>
    /// Controls mouse interaction
    /// </summary>
    private bool ControlMouseInteraction()
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
                        aimedCard.scale.MoveTo(1.1f * Vector3.one, DeltaTime[Short]);
                    }
                }

            }
            if (Input.GetMouseButton(0))
            {
                objectiveCard.status = Card.Status.right;
                if (aimedCard == objectiveCard)
                {
                    aimedCard.status = Card.Status.right;
                }
                else
                {
                    aimedCard.status = Card.Status.wrong;

                    foreach (Card challange in challengeCards)
                    {
                        if (challange == objectiveCard)
                        {
                            challange.status = Card.Status.right;
                            break;
                        }
                    }
                }
                return false;
            }
        }
        else
        {
            onCard = false;
            if (aimedCard != null)
            {
//                aimedCard.status = Card.Status.free;
                aimedCard.scale.MoveTo(Vector3.one, DeltaTime[VeryShort]);
                aimedCard = null;
            }
        }
        return true;
    }

    /// <summary>
    /// Wait the specified time and set status game after that.
    /// </summary>
    /// <param name="time">Time to wait.</param>
    /// <param name="after">After status.</param>
    private void Wait(float time, Status after)
    {
        timeToWait = time;
        nextStatus = after;
        gameStatus = Status.waitingMotion;
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
        transform.SetParent(this.transform);

        cardObjAux.name = "Card_" + valueNames[value] + "_" + suitNames[suit];
        cardAux = cardObjAux.GetComponent<Card>();
        cardAux.UpdateInfos(suit, value);
        cardAux.position.MoveTo((backgndDist - (challengeNumber - packCounter) * cardThick) * Vector3.forward);
        cardAux.rotation.MoveTo(180f * Vector3.up);
        cardAux.scale.MoveTo(Vector3.one);
        packCounter++;

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
            else
                Destroy(cardAux.gameObject);
        }
        return deckAux;

    }
    #endregion

    #region Movements manager
    /// <summary>
    /// Hides the card after delay time.
    /// </summary>
    /// <returns>Total time to execute</returns>
    /// <param name="card">Card to be hidden.</param>
    /// <param name="delay">Delay time to be hidden.</param>
    float HideCard(Card card, float delay)
    {
        card.rotation.MoveTo(new Vector3(0f, 180f, 0f), DeltaTime[Long], delay);
        return delay;
    }

    /// <summary>
    /// Shows the card after delay time.
    /// </summary>
    /// <returns>Total time to execute</returns>
    /// <param name="card">Card to be showed.</param>
    /// <param name="delay">Delay time to be showed.</param>
    float ShowCard(Card card, float delay)
    {
        card.rotation.MoveTo(Vector3.zero, DeltaTime[Long], delay);
        return delay;
    }

    /// <summary>
    /// Hides a pack of cards.
    /// </summary>
    /// <returns>Total time to execute</returns>
    /// <param name="deck">Deck to be hidden.</param>
    /// <param name="delayStep">Delay step between each hidden movement.</param>
    /// <param name="delay">Delay before start to hidden.</param>
    float HideCards(List<Card> deck, float delayStep, float delay)
    {
        foreach (Card card in deck)
        {
            HideCard(card, deck.IndexOf(card) * delayStep + delay);
        }
        return (DeltaTime[Long] + deck.Count * delayStep + delay);
    }

    /// <summary>
    /// Shows a pack of cards.
    /// </summary>
    /// <returns>Total time to execute</returns>
    /// <param name="deck">Deck to be showed.</param>
    /// <param name="delayStep">Delay step between each showed movement.</param>
    /// <param name="delay">Delay before start to showed.</param>
    float ShowCards(List<Card> deck, float delayStep, float delay)
    {
        foreach (Card card in deck)
        {
            ShowCard(card, deck.IndexOf(card) * delayStep + delay);
        }
        return (DeltaTime[Long] + deck.Count * delayStep + delay);
    }


    /// <summary>
    /// Adds a card to the pack.
    /// </summary>
    /// <returns>Total time to execute</returns>
    /// <param name="card">Card to be added.</param>
    float PackCard(Card card, float delay)
    {
        card.position.MoveTo((backgndDist - cardThick * packCounter) * Vector3.forward, DeltaTime[Long], delay);
        packCounter++;
        return delay;
    }

    /// <summary>
    /// Packs the cards of a deck.
    /// </summary>
    /// <returns>Total time to execute</returns>
    /// <param name="deck">Deck to be packed.</param>
    float PackCards(List<Card> deck, float delayStep, float delay)
    {
        foreach (Card card in deck)
        {
            PackCard(card, deck.IndexOf(card) * delayStep + delay);
        }
        return (DeltaTime[Long] + deck.Count * delayStep + delay);
    }

    /// <summary>
    /// Spreads the cards in a circle around screen.
    /// </summary>
    /// <param name="deck">Deck to be spread</param>
    /// <returns>Total time to execute</returns>
    float SpreadCards(List<Card> deck)
    {
        float angShare;

        angShare = 2f * Mathf.PI / deck.Count;
        foreach (Card card in deck)
        {
            card.position.MoveTo(new Vector3(spreadRadius * Mathf.Sin(angShare * deck.IndexOf(card)), spreadRadius * Mathf.Cos(angShare * deck.IndexOf(card)), 0), DeltaTime[Long], deck.IndexOf(card) * DeltaTime[Short]);
        }
        return (deck.Count * DeltaTime[Short]);
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
