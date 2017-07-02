using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages creation and organization of cards in the game.
/// </summary>
public class Dealer : GameBase {

    [SerializeField]
    public GameMode mode = GameMode.Basic;

    [SerializeField]
    private Status gameStatus;      // interfaceManager.controls the game phases
    public GameObject cardPrefab;   // Card prefab to be instantiated
    public ControlManager player;   // Reads player's position

    [SerializeField]
    private float waitCounter;      // Counter for waiting function
    private float timeToWait;       // Aux variable for sums time to wait
    private float timeToChoose;     // Time player takes to choose
    private int packCounter;        // Counter for positioning cards in a pack
    private int challengeNumber;    // Number of card in challange - It may be useless
    private bool onCard;            // Checks if mouse is on a card

    [SerializeField]
    private Status nextStatus;      // Save the next status after wait moves
    [SerializeField]
    private Card aimedCard;
    /// <summary>
    /// Class to acess interface components.
    /// </summary>
    [SerializeField]
    private InterfaceManager interfaceManager;

    // Cards in the game    
    [SerializeField]
    private List<Card> cardsInGame;     // All card in the game
    [SerializeField]
    private List<Card> challengeCards;  // Challenge cards in the game
    [SerializeField]
    private Card objectiveCard;         // Objective card
    private List<Choice> choices;       // Saves player's choices

    private LayerMask cardMask;

    [SerializeField]
    private AudioSource soundEffect;
    public AudioClip successSound, failSound;

    public float[] mainAngles;
    public float[] subAngles;

    void Start () 
    {
        choices = new List<Choice>();
        gameStatus = Status.newTurn;
        cardMask = LayerMask.GetMask("Card");
        challengeCards = new List<Card>();
        cardsInGame = new List<Card>();
        player = gameObject.AddComponent<ControlManager>();
        player.connection = interfaceManager.control.connection;

        onCard = false;
        waitCounter = 0f;
        timeToWait = 0f;
        timeToChoose = 0f;
        packCounter = 0;
        soundEffect = gameObject.GetComponent<AudioSource>();
//        interfaceManager.control.slider.onValueChanged.AddListener(delegate {EndTurn();} );

        mainAngles = new float[] {0f, 90f, 180f, 270f};
        subAngles  = new float[] {-36f, -18f, 0f, 18f, 36f};
    }

	void Update () 
    {
        mode = (GameMode)interfaceManager.control.gameMode.value;

        switch (gameStatus)
        {
            case Status.newTurn:
                interfaceManager.control.gameStatus = Status.newTurn;

                if (interfaceManager.control.status == Status.end)
                    gameStatus = Status.endGame;

                Wait(interfaceManager.CountDownCounter + DeltaTime[VeryShort], Status.playerPlay);

                challengeNumber = Mathf.FloorToInt(interfaceManager.control.slider.value);

                challengeCards = CreateHardDeck(challengeNumber);   // Creates a pack of challenge card with n cards
                objectiveCard = CreateCard(challengeCards[0]); // Chose one card from challenge cards to be the objective card
                break;
            case Status.playerPlay:
                interfaceManager.control.gameStatus = Status.playerPlay;

                if (interfaceManager.control.status == Status.end)
                    gameStatus = Status.endGame;

                if ((FindCardPointed(cardsInGame) != null) && (player.Action))
                {
                    objectiveCard.status = Card.Highlight.free;
                    objectiveCard.timeToTwinkle = 0f;

                    switch (mode)
                    {
                        case GameMode.Memory:
                            ChangeCards(cardsInGame, Card.ValueType.doubleValue);
                            ChangeCards(cardsInGame, Card.SuitType.singleSuit);

                            if (objectiveCard.showed)
                            {
                                timeToWait = HideCard(objectiveCard, 0f);

                                timeToWait = ShowCards(challengeCards, timeToWait);   // ... and show them

                                Wait(timeToWait, Status.playerChoice);
                            }
                            else
                            {
                                timeToWait = SpreadCards(challengeCards, 90f + Random.Range(0, challengeNumber) * 360f / challengeNumber);    // Spread the cards on screen...
                                timeToWait = objectiveCard.position.MoveTo(0.5f * Vector3.back, DeltaTime[Long], timeToWait);  // Highlightes objective card in center

                                timeToWait = ShowCard(objectiveCard, timeToWait);   // Also shows objective card

                                Wait(timeToWait, Status.playerPlay);
                            }
                            break;
                        case GameMode.Basic:
                            ChangeCards(cardsInGame, Card.ValueType.doubleValue);
                            ChangeCards(cardsInGame, Card.SuitType.singleSuit);
                            goto default;
                        case GameMode.MultiSuits:
                            ChangeCards(cardsInGame, Card.ValueType.doubleValue);
                            ChangeCards(cardsInGame, Card.SuitType.complete);
                            goto default;
                        case GameMode.CountSuits:
                            objectiveCard.UpdateInfos(Card.ValueType.noValue);
                            objectiveCard.UpdateInfos(Card.SuitType.multiSuit);
                            ChangeCards(challengeCards, Card.ValueType.doubleValue);
                            ChangeCards(challengeCards, Card.SuitType.miniSuit);
                            goto default;
                        default:
                            int challenge = interfaceManager.mainChallenge.Challenge;
                            int challenge2 = interfaceManager.subChallenges[challenge].Challenge;

                            timeToWait = SpreadCards(challengeCards, mainAngles[challenge] + subAngles[challenge2]);    // Spread the cards on screen...
//                            timeToWait = SpreadCards(challengeCards, 90f + Random.Range(0, challengeNumber) * 360f / challengeNumber);    // Spread the cards on screen...
                            objectiveCard.position.MoveTo(0.5f * Vector3.back, DeltaTime[Long], timeToWait);  // Highlightes objective card in center

                            timeToWait = ShowCards(challengeCards, timeToWait);   // ... and show them

                            timeToWait = ShowCard(objectiveCard, timeToWait);   // Also shows objective card

                            Wait(timeToWait, Status.playerChoice);
                            break;
                    }
                }   // Waits player plays the game
                else
                {
                    if (Time.timeScale != 0f)
                        objectiveCard.HighlightTimer(LoadingTime[VeryLong], 1f);
                }
                break;
            case Status.playerChoice:
                interfaceManager.control.gameStatus = Status.playerChoice;

                if (interfaceManager.control.status == Status.end)
                    goto case Status.endTurn;

                if (Time.timeScale != 0f)
                {
                    timeToChoose += Time.unscaledDeltaTime;
                    if (interfaceManager.control.helpToggle.isOn)
                        challengeCards[0].HighlightTimer(LoadingTime[MostLogner], 0.75f);
                }
                gameStatus = WaitCardChoice();  // Waits player's choice
                break;
            case Status.wrong:
                interfaceManager.control.gameStatus = Status.wrong;
                aimedCard.status = Card.Highlight.wrong;   // If chosen card is wrong, highlight it first
                Wait(1.5f, Status.right);   // Waits x seconds before shows right cards
                break;
            case Status.right:
                interfaceManager.control.gameStatus = Status.right;

                if (interfaceManager.control.status == Status.end)
                    goto case Status.endTurn;

                foreach (Card card in challengeCards)
                {
                    if (card == objectiveCard)
                    {
                        card.status = Card.Highlight.right;
                    }
                    else
                    {
                        if (card.status == Card.Highlight.free)
                        {
                            HideCard(card, 0f); // Oculta cartas que nao sejam as corretas e a errada
                        }
                    }
                }   // Highlight the right cards and hides the cards not highlighted
                objectiveCard.status = Card.Highlight.right;

                Wait(2.5f, Status.endTurn);
                break;
            case Status.endTurn:
                interfaceManager.control.gameStatus = Status.endTurn;
                aimedCard = null;
                foreach (Card card in cardsInGame)
                {
                    card.status = Card.Highlight.free;
                    card.scale.MoveTo(Vector3.one);
                }   // Removes all highlights

                // Hides and packs all the cards
                packCounter = 0;
                timeToWait = HideCards(cardsInGame, 0f, 0f);
                timeToWait = PackCards(cardsInGame, DeltaTime[Short], timeToWait);
                if (interfaceManager.control.status == Status.end)
                {
                    Wait(timeToWait, Status.endGame);
                }
                else
                {
                    Wait(timeToWait, Status.destroy);
                }
                break;
            case Status.destroy:
                interfaceManager.control.gameStatus = Status.destroy;
                DestroyDeck(challengeCards);
                DestroyCard(objectiveCard);
                cardsInGame.Clear();
                packCounter = 0;

                gameStatus = Status.newTurn;
                goto case Status.newTurn;
            case Status.waitingMotion:
                if (interfaceManager.control.status == Status.end)
                {
                    interfaceManager.control.gameStatus = Status.waitingMotion;
                }
                
                if (waitCounter < timeToWait)
                    waitCounter += Time.deltaTime;
                else
                {
                    waitCounter = timeToWait = 0f;
                    gameStatus = nextStatus;

                }
                break;
            case Status.endGame:
                interfaceManager.control.gameStatus = Status.endGame;
                if (interfaceManager.control.status != Status.end)
                {
                    Debug.Log("Not end");
                    gameStatus = Status.destroy;
                }
                else
                {
                    Time.timeScale = 0f;
                    Debug.Log("End?");
                }
                break;
        }
        float orderCounter;
        if (Choice.orderCounter == 0)
            orderCounter = 1f;
        else
            orderCounter = 1f/Choice.orderCounter;

        interfaceManager.scoreValue = Choice.totalPoints;
        interfaceManager.control.metric1Value = 100f * Choice.suitCounter * orderCounter;
        interfaceManager.control.metric2Value = 100f * Choice.valueCounter * orderCounter;
        interfaceManager.control.metric3Value = 100f * Choice.colorCounter * orderCounter;
    }

    #region Generic functions
    /// <summary>
    /// Finds the card pointed by mouse in a deck or return null.
    /// </summary>
    /// <returns>The card pointed or null.</returns>
    /// <param name="deck">Deck of card to be checked.</param>
    private Card FindCardPointed(List<Card> deck)
    {
        Ray camRay = Camera.main.ScreenPointToRay (player.Position);
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
    /// Finds the card pointed by mouse in a deck or return null.
    /// </summary>
    /// <returns>The card pointed or null.</returns>
    /// <param name="deck">Deck of card to be checked.</param>
    private Vector3 FindPlacePointed()
    {
        Ray camRay = Camera.main.ScreenPointToRay (player.Position);
        RaycastHit hit;

        if (Physics.Raycast(camRay, out hit, 50f, cardMask))
            return hit.point;
        else
            return Vector3.zero;
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
            else
            {
                if (aimedCard != cardAux)
                {
                    Debug.Log(aimedCard);
                    Debug.Log(cardAux);
                    aimedCard.scale.MoveTo(Vector3.one, DeltaTime[VeryShort]);
                    aimedCard = cardAux;
                    aimedCard.scale.MoveTo(1.1f * Vector3.one, DeltaTime[Short]);
                }
            }

            if (player.Action)
            {
                challengeCards[0].status = Card.Highlight.free;
                challengeCards[0].timeToTwinkle = 0f;

                if (player.forceConnection)
                    timeToChoose -= LoadingTime[Medium] / Time.timeScale;
                
                choices.Add(new Choice(aimedCard, objectiveCard, challengeNumber, timeToChoose));

                Vector3 choicePosition = FindPlacePointed();

                interfaceManager.log.Register(interfaceManager.control.gameTime, choices[choices.Count - 1], aimedCard.position.Value, choicePosition);

                // Verifies the closest angle to a plan's angle
                float angle = Atan2(choicePosition.y, choicePosition.x) * Mathf.Rad2Deg;

                int ang = VerifyCloserAngle(mainAngles, angle);
                angle -= mainAngles[ang];
                int ang2 = VerifyCloserAngle(subAngles, angle);

                // Plan's angle
                angle = Mathf.Deg2Rad * (mainAngles[ang] + subAngles[ang2]);
                Vector3 planPosition = new Vector3(spreadRadius * Mathf.Cos(angle), spreadRadius * Mathf.Sin(angle), 0f);

                interfaceManager.mainChallenge.AddChoice(ang);
                interfaceManager.subChallenges[ang].AddChoice(ang2);

                interfaceManager.control.map.choices.Add((Vector2)
                    ControlManager.Instance.ankle.CircleToElipse(Camera.main.WorldToScreenPoint(choicePosition)
                        - new Vector3(Screen.width / 2f, Screen.height / 2f, 0f), 0.45f * Screen.height));
                interfaceManager.control.map.challenges.Add((Vector2)
                    ControlManager.Instance.ankle.CircleToElipse(Camera.main.WorldToScreenPoint(planPosition)
                        - new Vector3(Screen.width / 2f, Screen.height / 2f, 0f), 0.45f * Screen.height));

                onCard = false;
                timeToChoose = 0f;

                float gameSpeed_aux = LI(TimeChoiceLimits[0], GameSpeedLimits[1], TimeChoiceLimits[1], GameSpeedLimits[0], Choice.averageTimeToChoose) * Choice.totalMatches / Choice.orderCounter;

                if (Mathf.Abs(gameSpeed_aux - interfaceManager.control.gameSpeed) > TimeChoiceLimits[0])
                    interfaceManager.control.gameSpeed += TimeChoiceLimits[0] * Mathf.Sign(gameSpeed_aux - interfaceManager.control.gameSpeed);
                else
                    interfaceManager.control.gameSpeed = gameSpeed_aux;

                interfaceManager.control.gameSpeed = Mathf.Clamp(interfaceManager.control.gameSpeed, GameSpeedLimits[0], GameSpeedLimits[1]);

                float precisionRate;
                if ((bool)choices[choices.Count - 1])
                    precisionRate = 1f;
                else
                    precisionRate = (challengeNumber - 1) / challengeNumber;

                Choice.precision = (Choice.precision * (Choice.orderCounter - 1) + precisionRate) / Choice.orderCounter;
            //    interfaceManager.metric3Value = 100f * Choice.precision;

                ShowCard(objectiveCard, 0f);

                if (aimedCard == objectiveCard)
                {
                    interfaceManager.control.gameStatus = Status.right;
                    soundEffect.clip = successSound;
//                    soundEffect.Play();
                    return Status.right;
                }
                else
                {
                    interfaceManager.control.gameStatus = Status.wrong;
                    soundEffect.clip = failSound;
//                    soundEffect.Play();
                    return Status.wrong;
                }
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
    /// Ends the turn.
    /// </summary>
 /*   private void EndTurn()
    {
        switch (gameStatus)
        {
            case Status.waitingMotion:
                switch (nextStatus)
                {
                    case Status.endTurn:
                        break;
                    case Status.playerPlay:
                        gameStatus = Status.destroy;
                        break;
                    case Status.destroy:
                        break;
                    default:
                        nextStatus = Status.endTurn;
                        break;
                }
                break;
            case Status.playerPlay:
                gameStatus = Status.destroy;
                break;
            default:
                gameStatus = Status.endTurn;
                break;
        }
    }*/
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
        cardAux.UpdateInfos(Card.SuitType.multiSuit);

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
    /// Creates a random deck with n cards and a hard dificult.
    /// </summary>
    /// <returns>Random deck.</returns>
    /// <param name="nCards">Number of cards.</param>
    private List<Card> CreateHardDeck(int nCards)
    {
        List<Card> deckAux;
        Card cardAux;
        int[] posAux = new int[2];

        posAux[0] = Random.Range(1, nCards);
        posAux[1] = Random.Range(1, nCards);
        while (posAux[1] == posAux[0])
            posAux[1] = Random.Range(1, nCards);
        
        deckAux = new List<Card>();
        cardAux = CreateCard();
        deckAux.Add(cardAux);
        
        for (int iCard = 1; iCard < nCards; )
        {
            if (iCard == posAux[0])
            {
                switch (deckAux[0].suit)
                {
                    case (int)Suits.Club:
                        cardAux = CreateCard((int)Suits.Spades, deckAux[0].value);
                        break;
                    case (int)Suits.Spades:
                        cardAux = CreateCard((int)Suits.Club, deckAux[0].value);
                        break;
                    case (int)Suits.Heart:
                        cardAux = CreateCard((int)Suits.Diamond, deckAux[0].value);
                        break;
                    case (int)Suits.Diamond:
                        cardAux = CreateCard((int)Suits.Heart, deckAux[0].value);
                        break;
                    default:
                        Debug.Log("Something wrong is not correct");
                        break;
                }

                if (!deckAux.Contains(cardAux))
                {
                    deckAux.Add(cardAux);
                    iCard++;
                }
                else
                {
                    DestroyCard(cardAux);
                    posAux[0] = -1;
                }
            }
            else if (iCard == posAux[1])
            {
                int valueAux;

                valueAux = Random.Range(0, valueNames.Length);
                while (valueAux == deckAux[0].value)
                    valueAux = Random.Range(0, valueNames.Length);

                cardAux = CreateCard(deckAux[0].suit, valueAux);

                if (!deckAux.Contains(cardAux))
                {
                    deckAux.Add(cardAux);
                    iCard++;
                }
                else
                {
                    DestroyCard(cardAux);
                    posAux[1] = -1;
                }
            }
            else
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
        card.showed = false;
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
        card.showed = true;
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
        float timeToDo = 0f;

        angShare = 2f * Mathf.PI / deck.Count;
        foreach (Card card in deck)
        {
            timeToDo = card.position.MoveTo(new Vector3(spreadRadius * Mathf.Cos(angShare * deck.IndexOf(card) + Mathf.Deg2Rad * angle), spreadRadius * Mathf.Sin(angShare * deck.IndexOf(card) + Mathf.Deg2Rad * angle), 0f), DeltaTime[Long], deck.IndexOf(card) * DeltaTime[Short]);
        }
        return (timeToDo);
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

    /// <summary>
    /// Changes the type of the suit in each card.
    /// </summary>
    /// <param name="deck">Deck to be changed.</param>
    /// <param name="type">Type of suit.</param>
    private void ChangeCards(List<Card> deck, Card.SuitType type)
    {
        foreach (Card card in deck)
        {
            card.UpdateInfos(type);
        }
    }

    /// <summary>
    /// Changes the type of the value in each card.
    /// </summary>
    /// <param name="deck">Deck to be changed.</param>
    /// <param name="type">Type of value.</param>
    private void ChangeCards(List<Card> deck, Card.ValueType type)
    {
        foreach (Card card in deck)
        {
            card.UpdateInfos(type);
        }
    }
    #endregion

}
