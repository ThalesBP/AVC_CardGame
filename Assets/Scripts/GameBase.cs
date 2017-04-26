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
    protected static readonly int CountDown = 1;    // 3 seconds
    protected static readonly float[] GameSpeedLimits = { 1f, 3f };
    protected static readonly float[] TimeChoiceLimits = { 0.5f, 4.5f };
    #endregion

    #region Position, rotation and scale of cards on screen
    static protected float backgndDist = 0.7f;      // Background distance
    static protected float cardThick = 0.01f;       // Card's thickness
    static protected float spreadRadius = 3f;       // The radius that cards are spread
    static protected float highlightScale = 1.2f;   // How much it grows to hightlight somethings
    #endregion

    #region Interface
    protected static readonly Color GreenColor = new Color(16f/255f, 204f/255f, 0f);
    protected static readonly Color BlueColor = new Color(0f, 192f/255f, 1f);
    protected static readonly Color RedColor = Color.red;
    protected static readonly Color YellowText = new Color(1f, 206f/255f, 0f);
    protected static readonly Color BlackMatteColor = new Color(50f/255f, 50f/255f, 50f/255f, 0.5f);
    #endregion

    #region Texts
    public enum Languages {English, Portuguese};
    public static Languages chosenLanguage = Languages.English;
    // Central Message
    protected static readonly string[] readyText = { "READY?", "PRONTO?" };
    protected static readonly string[] goText = { "Go!", "Vai!" };
    protected static readonly string[] endOfGameText = { "End of game", "Fim de jogo" };
    // Results
    protected static readonly string[] hitRateText = { "Hit Rate", "Taxa de Acerto" };
    protected static readonly string[] timeRateText = { "Time to Choose", "Tempo de Escolha" };
    protected static readonly string[] info1Text = { "Plantar/Dorsiflexion", "Plantar/Dorsiflexão" };
    protected static readonly string[] info2Text = { "Inver/Eversion", "Inver/Eversão" };
    protected static readonly string[] ofText = { " of ", " de " };
    protected static readonly string[] fromText = { " from ", " de " };
    protected static readonly string[] toText = { " to ", " até " };
    // Realtime Infos
    protected static readonly string[] scorePointsText = { "Score", "Pontos" };
    protected static readonly string[] timeText = { "Time", "Tempo" };
    protected static readonly string[] metric1Text = { "Suit", "Naipe" };
    protected static readonly string[] metric2Text = { "Value", "Valor" };
    protected static readonly string[] metric3Text = { "Color", "Cor" };
    // Control panel
    protected static readonly string[] controlPanelText = { "Control", "Controle" };
    protected static readonly string[] connectText = { "Connect", "Conectar" };
    protected static readonly string[] disconnectText = { "Disconnect", "Desconectar" };
    protected static readonly string[] startText = { "Start", "Iniciar" };
    protected static readonly string[] restartText = { "Restart", "Reiniciar" };
    protected static readonly string[] pausedText = { "Paused", "Pausado" };
    protected static readonly string[] pauseText = { "Pause", "Pausar" };
    protected static readonly string[] stopText = { "Stop", "Parar" };
    protected static readonly string[] playTimeText = { "Play time in minutes", "Tempo de jogo em minutos" };
    protected static readonly string[] infTimeText = { "Infinite game time", "Tempo de jogo infinito" };
    protected static readonly string[] helpText = { "Visual Help", "Ajuda Visual" };
    protected static readonly string[] cardsText = { "Cards", "Cartas" };
    // User panel
    protected static readonly string[] userPanelText = { "User", "Usuário" };
    protected static readonly string[] insertUserText = { "Insert user", "Insira usuário" };
    protected static readonly string[] userWrongText = { "Name invalid or repeated", "Nome inválido ou repetido" };
    protected static readonly string[] insertInfoText = { "Insert any information (optional)", "Insira alguma informação (opcional)" };
    protected static readonly string[] addText = { "Add", "Adicionar" };
    protected static readonly string[] doneText = { "Done", "Feito" };
    protected static readonly string[] managerLoginText = { "Manager Login", "Entrar Administrador" };
    protected static readonly string[] enterPasswordText = { "Enter password...", "Insira a senha..." }; 
    protected static readonly string[] passwordWrongText = { "Password wrong!", "Senha errada!" }; 
    protected static readonly string[] loginText = { "Log In", "Entrar" };
    protected static readonly string[] logoutText = { "Log Out", "Sair" };
    protected static readonly string[] playerSelectText = { "Player Select", "Selecionar Jogador" };
    protected static readonly string[] chooseText = { "Choose", "Escolher" };
    protected static readonly string[] changeText = { "Change", "Mudar" };

    public enum Limbs { rightHand, leftHand, rightFoot, leftFoot };
    protected static readonly string[,] limbTexts = 
        {
            {"Right hand", "Mão direita" },
            {"Left hand", "Mão esquerda" },
            {"Right foot", "Pé direito" },
            {"Left foot", "Pé esquerdo" }
        };

    // Obs: Comment later
    public enum Status {
        begin, 
        playing, 
        counting,
        paused, 
        end, 
        newTurn, 
        playerPlay, 
        playerChoice, 
        rightCard, 
        wrongCard, 
        endTurn, 
        waitingMotion, 
        endGame,
        connecting,
        connected,
        disconnecting,
        disconnected
    };

    protected static readonly string[,] gameMessageTexts =
    {
            {"Creating new game", "Criando novo jogo"},
            {"Waiting player starts", "Esperando jogador iniciar"},
            {"Waiting player chooses", "Esperando jogador escolher"},
            {"Choice done", "Jogada realizada"},
            {"Right card", "Carta certa"},
            {"Wrong card", "Carta errada"},
            {"Turn ended", "Turno terminando"},
            {"Waiting motion", "Esperando movimento"},
            {"Waiting to start", "Esperando iniciar"},
            {"Game running", "Jogo executando"},
            {"Game paused", "Jogo pausado"},
            {"Showing results", "Mostrando resultados"},
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

    /// <summary>
    /// Returns the color with alpha changed.
    /// </summary>
    /// <returns>Color with alpha changed.</returns>
    /// <param name="color">Color to have alpha changed.</param>
    /// <param name="alpha">Alpha desired.</param>
    protected Color SetAlpha(Color color, float alpha)
    {
        return new Color(color.r, color.g, color.b, alpha);
    }

    /// <summary>
    /// Linear interpolation:
    /// Returns a <value>y</value> of a <value>x</value> in a line defined by <value>(x1, y1)</value> and <value>(x2, y2)</value>.
    /// </summary>
    /// <returns>Returns a <value>y</value> of a <value>x</value> in a line.</returns>
    /// <param name="x1">The first x value.</param>
    /// <param name="y1">The first y value.</param>
    /// <param name="x2">The second x value.</param>
    /// <param name="y2">The second y value.</param>
    /// <param name="x">The x coordinate of y desired value.</param>
    protected float LI (float x1, float y1, float x2, float y2, float x)
    {
        return ((y2 - y1) * x - x1 * y2 + x2 * y1) / (x2 - x1);
    }
}
