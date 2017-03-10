using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
///  Works as common base for all classes in the game.
/// </summary>
/// <remarks>
/// Here contents all information that are going to be used along the development and can make the changes easier.
/// </remarks>
public class GameBase : MonoBehaviour {

    #region Suit of cards, cards and player's position
    protected static readonly string[] suitNames = {"Diamond", "Spades", "Heart", "Club"};
    protected static readonly string[] suitSymbols = {"♦", "♠", "♥", "♣"};
    protected static readonly string[] valueNames = {"A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K"};
    protected static readonly string[] colorNames = { "Red", "Black"};

    public enum Suits {Diamond, Spades, Heart, Club};
    public enum Values {A = 0, J = 11, Q = 12, K = 13};
    public enum Colors {Red, Black};
    #endregion

    #region Card design informations
    protected static readonly float refSize = 3f;   // Ratio between charSize and fontSize to keep global size
    protected static int fontSize_M = 100;          // fontSize and charSize for Medium size
    protected static float charSize_M = refSize / fontSize_M;
    protected static int fontSize_S = 60;           // fontSize and charSize for Small size
    protected static float charSize_S =  refSize / fontSize_S;
    #endregion

    #region Time informations
    protected static readonly float[] DeltaTime = { 0.1f, 0.25f, 0.5f, 0.75f, 1f, 1.5f };
    protected static readonly int VeryShort = 0;
    protected static readonly int Short = 1;
    protected static readonly int Medium = 2;
    protected static readonly int Long = 3;
    protected static readonly int VeryLong = 4;
    protected static readonly int MuchLonger = 4;

    protected static readonly int CountDown = 3;    // 3 seconds
    #endregion

    #region Position, rotation and scale of cards on screen
    static protected float backgndDist = 0.4f;      // Background distance
    static protected float cardThick = 0.01f;       // Card's thickness
    static protected float spreadRadius = 3f;       // The radius that cards are spread
    static protected float highlightScale = 1.2f;   // How much it grows to hightlight somethings
    #endregion

    #region Interface
    protected static readonly Color ConnectColor = new Color(16f/255f, 204f/255f, 0f);
    protected static readonly Color StartColor = new Color(0f, 192f/255f, 1f);
    protected static readonly Color StopColor = Color.red;
    protected static readonly Color YellowText = new Color(1f, 206f/255f, 0f);
    #endregion

    #region Texts
    public enum Languages {English, Portuguese};
    public static Languages chosenLanguage = Languages.English;
    protected static readonly string[] readyText = { "READY?", "PRONTO?" };
    protected static readonly string[] goText = { "Go!", "Vai!" };
    protected static readonly string[] hitRateText = { "Hit Rate", "Taxa de Acerto" };
    protected static readonly string[] timeRateText = { "Time Rate", "Taxa de Tempo" };
    protected static readonly string[] info1Text = { "Plantar/Dorsiflexion", "Plantar/Dorsiflexão" };
    protected static readonly string[] info2Text = { "Inver/Eversion", "Inver/Eversão" };
    protected static readonly string[] scorePointsText = { "Score", "Pontos" };
    protected static readonly string[] timeText = { "Time", "Tempo" };
    protected static readonly string[] metric1Text = { "Suit", "Naipe" };
    protected static readonly string[] metric2Text = { "Value", "Valor" };
    protected static readonly string[] metric3Text = { "Color", "Cor" };
    protected static readonly string[] panelText = { "Panel", "Painel" };
    protected static readonly string[] connectText = { "Connect", "Conectar" };
    protected static readonly string[] disconnectText = { "Disconnect", "Desconectar" };
    protected static readonly string[] startText = { "Start", "Iniciar" };
    protected static readonly string[] pausedText = { "Paused", "Pausado" };
    protected static readonly string[] pauseText = { "Pause", "Pausar" };
    protected static readonly string[] stopText = { "Stop", "Parar" };
    protected static readonly string[] playTimeText = { "Play time in minutes", "Tempo de jogo em minutos" };
    protected static readonly string[] infTimeText = { "Infinite game time", "Tempo de jogo infinito" };
    protected static readonly string[] helpText = { "Visual Help", "Ajuda Visual" };
    protected static readonly string[] numOfCardsText = { "Number of Cards", "Número de Cartas" };

    public enum Messages {newGame, playerPlay, playerChoice, rightCard, wrongCard, endGame, waitingMotion, Connecting, Disconnecting};
    protected static readonly string[,] gameMessageTexts =
    {
            {"Creating new game", "Criando novo jogo"},
            {"Waiting player starts", "Esperando jogador iniciar"},
            {"Choice done", "Jogada realizada"},
            {"Right card", "Carta certa"},
            {"Wrong card", "Carta errada"},
            {"Ending game", "Jogo terminando"},
            {"Waiting motion", "Esperando movimento"},
            {"Connecting", "Conectando"},
            {"Disconecting", "Desconectando"}
    };

    #endregion

    #region Other informations
    #endregion

    #region Public Variables    // Variables to be changed realtime
    public int fontSizeAdjust_S = 60;
    public int fontSizeAdjust_M = 100;
    public Languages languageAdjust = Languages.English;
    #endregion

    /// <summary>
    /// It is only used to update static variable from Unity's Inspector
    /// </summary>
    void Update()
    {
        fontSize_M = fontSizeAdjust_M;
        charSize_M = refSize / fontSize_M;
        fontSize_S = fontSizeAdjust_S;
        charSize_S = refSize / fontSize_S;
        chosenLanguage = languageAdjust;
    }

    /// <summary>
    /// Sorts a deck.
    /// </summary>
    /// <returns>The deck sorted.</returns>
    /// <param name="cardlist">The deck to be sorted.</param>
    private List<GameObject> SortCards(List<GameObject> cardlist)
    {
        GameObject aux;
        List<GameObject> finaldeck = new List<GameObject>();

        for (int j = 0; j < suitNames.Length; j++)
        {
            for (int i = 0; i < valueNames.Length; i++)
            {
                if ((aux = FindCard(cardlist, "Card_" + valueNames[i] + "_" + suitNames[j])) != null)
                    finaldeck.Add(aux);
            }
        }
        return finaldeck;
    }

    /// <summary>
    /// Finds a card in a deck.
    /// </summary>
    /// <returns>The card.</returns>
    /// <param name="pack">The deck where to find.</param>
    /// <param name="name">The card to be find.</param>
    protected GameObject FindCard(GameObject[] pack, string name)
    {
        for (int i = 0; i < pack.Length; i++) 
        {
            if (string.Compare (pack [i].name, name) == 0)
                return pack [i];
        }
        return null;
    }

    /// <summary>
    /// Finds a card in a deck.
    /// </summary>
    /// <returns>The card.</returns>
    /// <param name="pack">The deck where to find.</param>
    /// <param name="name">The card to be find.</param>
    protected GameObject FindCard(List<GameObject> pack, string name)
    {
        for (int i = 0; i < pack.Count; i++) 
        {
            if (string.Compare (pack [i].name, name) == 0)
                return pack [i];
        }
        return null;
    }


    protected Color SetAlpha(Color color, float alpha)
    {
        return new Color(color.r, color.g, color.b, alpha);
    }
}
