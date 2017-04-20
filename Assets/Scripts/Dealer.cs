using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages creation and organization of cards in the game.
/// </summary>
public class Dealer : GameBase {

    /// <summary>
    /// Game phases from new game to end game.
    /// </summary>
    public enum Status {newTurn, playerPlay, playerChoice, rightCard, wrongCard, endTurn, waitingMotion, endGame};

    [SerializeField]
    private Status gameStatus;      // Controls the game phases
    public GameObject cardPrefab;   // Card prefab to be instantiated
    public ControlManager player;   // Reads player's position

    private float waitCounter;      // Counter for waiting function
    private float timeToWait;       // Aux variable for sums time to wait
    private float timeToChoose;     // Time player takes to choose
    private int packCounter;        // Counter for positioning cards in a pack
    private int challengeNumber;    // Number of card in challange - It may be useless
    private bool onCard;            // Checks if mouse is on a card
    private Status nextStatus;      // Save the next status after wait moves
    private Card aimedCard;
    /// <summary>
    /// Class to acess interface components.
    /// </summary>
    [SerializeField]
    private InterfaceManager gameInterface;

    // Cards in the game    
    [SerializeField]
    private List<Card> cardsInGame;     // All card in the game
    [SerializeField]
    private List<Card> challengeCards;  // Challenge cards in the game
    [SerializeField]
    private Card objectiveCard;         // Objective card
    private List<Choice> choices;       // Saves player's choices

    private LayerMask cardMask;

    // Start variables
    void Start () 
    {
        choices = new List<Choice>();
        gameStatus = Status.newTurn;
        cardMask = LayerMask.GetMask("Card");
        challengeCards = new List<Card>();
        cardsInGame = new List<Card>();
        player = gameObject.AddComponent<ControlManager>();
        onCard = false;
        waitCounter = 0f;
        timeToWait = 0f;
        timeToChoose = 0f;
        packCounter = 0;
        gameInterface.numOfCardsSlider.onValueChanged.AddListener(delegate {SliderChanged();});
        gameInterface.stopButton.onClick.AddListener(delegate {GameEnd();});
        challengeNumber = Mathf.FloorToInt(gameInterface.numOfCardsSlider.value);
    }
    // Update is called once per frame
	void Update () 
    {
        switch (gameStatus)
        {
            case Status.newTurn:
                gameInterface.dealerMessage = Messages.newTurn;
                if (challengeCards.Count > 0)
                {
                    gameStatus = Status.playerPlay;
                    DestroyDeck(challengeCards);
                    DestroyCard(objectiveCard);
                    cardsInGame.Clear();
                    packCounter = 0;
                }   // Cards must be cleared after frist phase
                else
                {
                    gameInterface.scoreValue = 0;
                    gameInterface.metric1Value = 0f;
                    gameInterface.metric2Value = 0f;
                    gameInterface.metric3Value = 0f;
                    Wait(CountDown + 1, Status.playerPlay);
                }

                challengeCards = CreateDeck(challengeNumber);   // Creates a pack of challenge card with n cards
                objectiveCard = CreateCard(challengeCards[0]); // Chose one card from challenge cards to be the objective card
                break;
            case Status.playerPlay:
                gameInterface.dealerMessage = Messages.waitingPlayer;
                if ((FindCardPointed(cardsInGame) != null) && (player.GetAction()))
                    {
                    timeToWait = SpreadCards(challengeCards, 90f + Random.Range(0, challengeNumber - 1) * 360f / challengeNumber);    // Spread the cards on screen...
                    timeToWait = ShowCards(challengeCards, timeToWait);// DeltaTime[Short], DeltaTime[Long]);   // ... and show them

                    objectiveCard.position.MoveTo(0.5f * Vector3.back, DeltaTime[Long], challengeCards.Count * DeltaTime[Short]);   // Highlightes objective card in center

                    timeToWait = ShowCard(objectiveCard, timeToWait);// challengeCards.Count * DeltaTime[Short] + DeltaTime[Long]);   // Also shows objective card
                    Wait(timeToWait, Status.playerChoice);
                    }   // Waits player plays the game
                break;
            case Status.playerChoice:
                gameInterface.dealerMessage = Messages.waitingChoice;
                timeToChoose += Time.unscaledDeltaTime;
                gameStatus = WaitCardChoice();  // Waits player's choice
                break;
            case Status.wrongCard:
                gameInterface.dealerMessage = Messages.wrongCard;
                aimedCard.status = Card.Status.wrong;   // If chosen card is wrong, highlight it first
                Wait(1.5f, Status.rightCard);
                break;
            case Status.rightCard:
                gameInterface.dealerMessage = Messages.rightCard;
                int num = 0;
                foreach (Card card in challengeCards)
                {
                    if (card == objectiveCard)
                    {
                        card.status = Card.Status.right;
                    }
                    else
                    {
                        if (card.status == Card.Status.free)
                        {
                            HideCard(card, 0f);
                        }
                    }
                    num++;
                }   // Highlight the right cards and hides the cards not highlighted
                objectiveCard.status = Card.Status.right;

                Wait(2.5f, Status.endTurn);
                break;
            case Status.endTurn:
                gameInterface.dealerMessage = Messages.endTurn;
                aimedCard = null;
                foreach (Card card in cardsInGame)
                {
                    card.status = Card.Status.free;
                    card.scale.MoveTo(Vector3.one);
                }   // Removes all highlights

                // Hides and packs all the cards
                packCounter = 0;
                timeToWait = HideCards(cardsInGame, 0f, 0f);
                timeToWait = PackCards(cardsInGame, DeltaTime[Short], timeToWait);

                if (gameInterface.currentStatus == InterfaceManager.GameStatus.end)
                    Wait(timeToWait, Status.endGame);
                else 
                    Wait(timeToWait, Status.newTurn);
                break;
            case Status.waitingMotion:
                gameInterface.dealerMessage = Messages.waitingMotion;
                if (waitCounter < timeToWait)
                    waitCounter += Time.deltaTime;
                else
                {
                    waitCounter = timeToWait = 0f;
                    gameStatus = nextStatus;

                }
                break;
            case Status.endGame:
                if (gameInterface.currentStatus != InterfaceManager.GameStatus.end)
                {
                    gameStatus = Status.newTurn;
                    DestroyDeck(challengeCards);
                    DestroyCard(objectiveCard);
                    cardsInGame.Clear();
                    packCounter = 0;
                    Choice.ResetChoice();
                }
                else
                {
                    Time.timeScale = 0f;
                }
                break;
        }
    }

    #region Generic functions
    /// <summary>
    /// Finds the card pointed by mouse in a deck or return null.
    /// </summary>
    /// <returns>The card pointed or null.</returns>
    /// <param name="deck">Deck of card to be checked.</param>
    private Card FindCardPointed(List<Card> deck)
    {
        Ray camRay = Camera.main.ScreenPointToRay (player.GetPosition());
        RaycastHit cardHit;

        if (Physics.Raycast(camRay, out cardHit, 50f, cardMask))
        {
            foreach (Card card in deck)
            {
                if (cardHit.transform.parent.transform == card.transform)
                {
                    return card;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Waits the card choise.
    /// </summary>
    /// <returns>WaitingPlayer if have no choice or the result: right / wrong card</returns>
    private Status WaitCardChoice()
    {
        Card cardAux = FindCardPointed(challengeCards);

        if (cardAux != null)
        {
            if (!onCard)
            {
                onCard = true;
                aimedCard = cardAux;
                aimedCard.scale.MoveTo(1.1f * Vector3.one, DeltaTime[Short]);

            }
            if (player.GetAction())
            {
                choices.Add(new Choice(aimedCard, objectiveCard, challengeNumber, timeToChoose));
                gameInterface.scoreValue = Choice.totalPoints;

                onCard = false;
                timeToChoose = 0f;

                float gameSpeed_aux = LI(TimeChoiceLimits[0], GameSpeedLimits[1], TimeChoiceLimits[1], GameSpeedLimits[0], Choice.averageTimeToChoose) * Choice.totalMatches / Choice.orderCounter;

                if (Mathf.Abs(gameSpeed_aux - gameInterface.gameSpeed) > TimeChoiceLimits[0])
                    gameInterface.gameSpeed += TimeChoiceLimits[0] * Mathf.Sign(gameSpeed_aux - gameInterface.gameSpeed);
                else
                    gameInterface.gameSpeed = Mathf.Clamp(gameSpeed_aux, GameSpeedLimits[0], GameSpeedLimits[1]);

                gameInterface.metric1Value = 100f * Choice.suitCounter / Choice.orderCounter;
                gameInterface.metric2Value = 100f * Choice.valueCounter / Choice.orderCounter;
                gameInterface.metric3Value = 100f * Choice.colorCounter / Choice.orderCounter;

                float precisionRate;
                if ((bool)choices[choices.Count - 1])
                    precisionRate = 1f;
                else
                    precisionRate = (challengeNumber - 1) / challengeNumber;

                Choice.precision = (Choice.precision * (Choice.orderCounter - 1) + precisionRate) / Choice.orderCounter;
            //    gameInterface.metric3Value = 100f * Choice.precision;

                if (aimedCard == objectiveCard)
                    return Status.rightCard;
                else
                    return Status.wrongCard;
            }
        }
        else
        {
            onCard = false;
            if (aimedCard != null)
            {
                aimedCard.scale.MoveTo(Vector3.one, DeltaTime[VeryShort]);
                aimedCard = null;
            }
        }
        return Status.playerChoice;
    }

    /// <summary>
    /// Waits the specified time and set status game after that.
    /// </summary>
    /// <param name="time">Time to wait.</param>
    /// <param name="after">After status.</param>
    private void Wait(float time, Status after)
    {
        timeToWait = time;
        nextStatus = after;
        gameStatus = Status.waitingMotion;
    }

    /// <summary>
    /// Is called when slider is changed
    /// </summary>
    private void SliderChanged()
    {
        challengeNumber = Mathf.FloorToInt(gameInterface.numOfCardsSlider.value);
        switch (gameStatus)
        {
            case Status.playerPlay:
                gameStatus = Status.newTurn;
                break;
            case Status.playerChoice:
                gameStatus = Status.endTurn;
                break;
            default:
                break;
        }

    }

    private void GameEnd()
    {
        if (gameStatus == Status.waitingMotion)
            nextStatus = Status.endTurn;
        else
            gameStatus = Status.endTurn;
    }

    #endregion

    #region Create and Destroy card functions
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

        cardObjAux = Instantiate(cardPrefab, PackPosition(), Quaternion.Euler(180f * Vector3.up));    // Creates a game object based on cardBody
        transform.SetParent(this.transform);

        cardObjAux.name = "Card_" + valueNames[value] + "_" + suitNames[suit];
        cardObjAux.transform.SetParent(this.transform);
        cardAux = cardObjAux.GetComponent<Card>();
        cardAux.UpdateInfos(suit, value);

        cardsInGame.Add(cardAux);

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

    /// <summary>
    /// Destroies the card.
    /// </summary>
    /// <param name="card">Card to be destroyed.</param>
    private void DestroyCard(Card card)
    {
        cardsInGame.RemoveAt(cardsInGame.FindLastIndex(x => x == card));
        Destroy(card.gameObject);
    }

    #endregion

    #region Create and Destroy deck functions
    /// <summary>
    /// Creates the deck based in a number of suits and values.
    /// </summary>
    /// <returns>The deck.</returns>
    /// <param name="nSuits">Number of suits.</param>
    /// <param name="nValues">Number of values.</param>
    private List<Card> CreateDeck (int nSuits, int nValues)
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
    private List<Card> CreateDeck(int nCards)
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
                DestroyCard(cardAux);
        }
        return deckAux;

    }

    /// <summary>
    /// Destroys the deck.
    /// </summary>
    /// <param name="deck">Deck to be destroyed.</param>
    private void DestroyDeck(List<Card> deck)
    {
        foreach (Card card in deck)
        {
            DestroyCard(card);
        }
        deck.Clear();
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
        return DeltaTime[Long] + delay;
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
        return DeltaTime[Long] + delay;
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
    /// Shows a pack of cards eachone delayed by delayStep after wait delay time.
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
    /// Shows a pack of cards at one after wait delay time.
    /// </summary>
    /// <returns>Total time to execute</returns>
    /// <param name="deck">Deck to be showed.</param>
    /// <param name="delay">Delay before start to showed.</param>
    float ShowCards(List<Card> deck, float delay)
    {
        return ShowCards(deck, 0f, delay);
    }

    /// <summary>
    /// Returns current packs the position.
    /// </summary>
    Vector3 PackPosition()
    {
        packCounter++;
        return (backgndDist - cardThick * packCounter) * Vector3.forward;
    }

    /// <summary>
    /// Adds a card to the pack.
    /// </summary>
    /// <returns>Total time to execute</returns>
    /// <param name="card">Card to be added.</param>
    float PackCard(Card card, float delay)
    {
        card.position.MoveTo(PackPosition(), DeltaTime[Long], delay);
        return DeltaTime[Long] + delay;
    }

    /// <summary>
    /// Packs the cards of a deck.
    /// </summary>
    /// <returns>Total time to execute.</returns>
    /// <param name="deck">Deck to be packed.</param>
    float PackCards(List<Card> deck, float delayStep, float delay)
    {
        foreach (Card card in deck)
        {
            PackCard(card, deck.IndexOf(card) * delayStep + delay);
        }
        return (DeltaTime[Long] + (deck.Count - 1) * delayStep + delay);
    }

    /// <summary>
    /// Spreads the cards in a circle around screen starting from a angle.
    /// </summary>
    /// <param name="deck">Deck to be spread.</param>
    /// <param name="Angle of first card in degrees."> 
    /// <returns>Total time to execute</returns>
    float SpreadCards(List<Card> deck, float angle)
    {
        float angShare;
       
        angShare = 2f * Mathf.PI / deck.Count;
        foreach (Card card in deck)
        {
            card.position.MoveTo(new Vector3(spreadRadius * Mathf.Cos(angShare * deck.IndexOf(card) + Mathf.Deg2Rad * angle), spreadRadius * Mathf.Sin(angShare * deck.IndexOf(card) + Mathf.Deg2Rad * angle), 0), DeltaTime[Long], deck.IndexOf(card) * DeltaTime[Short]);
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
    private Card ChooseCard(List<Card> deck)
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
    private Card PickCard(List<Card> deck)
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
    private List<Card> PickCards(List<Card> deck, int nCards)
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
