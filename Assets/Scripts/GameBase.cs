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
    protected static readonly string[] valueNames = {"A", "2", "3", "4", "5", "6", "7", "8", "9", "10"}; //, "J", "Q", "K"};
    protected static readonly string[] colorNames = { "Red", "Black"};

    public enum Suits {Diamond, Spades, Heart, Club};
    public enum Values {A = 0, J = 11, Q = 12, K = 13}; // Obs: Necessary?
    public enum Colors {Red, Black};
    #endregion

    #region Card design informations
    protected static readonly float refSize = 3f;   // Ratio between charSize and fontSize to keep global size
    protected static int fontSize_M = 100;          // fontSize and charSize for Medium size
    protected static float charSize_M = refSize / fontSize_M;
    protected static int fontSize_S = 60;           // fontSize and charSize for Small size
    protected static float charSize_S =  refSize / fontSize_S;
    protected static float scaleSuit = 0.5f, xPos = 0.00365f, yPos = 0.0055f;   // Multisuits references
    protected static Vector3 centralScale = new Vector3(0.04f, 0.03f, 0.4f);
    protected static Vector3 valuePos = new Vector3(0f, -0.007f, -0.008f);
    protected static Vector3 valueScale = new Vector3(0.015f, 0.01f, 0.4f);
    #endregion

    #region Game Modes
    protected static readonly int numOfGameModes = 4; // Number of game modes
    public enum GameMode {Basic, Memory, MultiSuits, CountSuits};
    protected static readonly string[,] gameModeTexts =
        {
                {"Basic", "Básico"},
                {"Memory", "Memória"},
                {"Multi Suits", "Múltiplos Naipes"},
                {"Count Suits", "Contar Naipes"}
        };
    #endregion

    #region Time informations
    protected static readonly float[] LoadingTime = {0.1f, 0.5f, 1.25f, 2.5f, 3.5f, 5f, 8f};
    protected static readonly float[] DeltaTime = { 0.1f, 0.25f, 0.5f, 0.75f, 1f, 1.5f };
    protected static readonly int VeryShort = 0;
    protected static readonly int Short = 1;
    protected static readonly int Medium = 2;
    protected static readonly int Long = 3;
    protected static readonly int VeryLong = 4;
    protected static readonly int MuchLonger = 5;
    protected static readonly int MostLogner = 6;
    protected static readonly int CountDown = 3;    // 3 seconds
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
    protected static readonly Color Nude1 = new Color(232f/255f, 202f/255f, 179f/255f, 1f);
    protected static readonly Color Nude2 = new Color(196f/255f, 144f/255f, 124f/255f, 1f);
    protected static readonly Color Nude3 = new Color(111f/255f, 86f/255f, 84f/255f, 1f);
    #endregion

    #region Texts
    public enum Languages {English, Portuguese};
    public static Languages chosenLanguage = Languages.English;
    // Central Message
    protected static readonly string[] readyText = { "READY?", "PRONTO?" };
    protected static readonly string[] goText = { "Go!", "Vai!" };
    protected static readonly string[] endOfGameText = { "End of game", "Fim de jogo" };
    protected static readonly string[] moveCirclesText = { "Move in circles", "Mova em círculos" };
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
    protected static readonly string[] connectingText = { "Connecting", "Conectando" };
    protected static readonly string[] disconnectText = { "Disconnect", "Desconectar" };
    protected static readonly string[] calibrateText = { "Calibrate", "Calibrar" };
    protected static readonly string[] calibratingText = { "Calibrating", "Calibrando" };
    protected static readonly string[] startText = { "Start", "Iniciar" };
    protected static readonly string[] restartText = { "Restart", "Reiniciar" };
    protected static readonly string[] pausedText = { "Paused", "Pausado" };
    protected static readonly string[] pauseText = { "Pause", "Pausar" };
    protected static readonly string[] stopText = { "Stop", "Parar" };
    protected static readonly string[] playTimeText = { "Play time in minutes", "Tempo de jogo em minutos" };
    protected static readonly string[] infTimeText = { "Infinite game time", "Tempo de jogo infinito" };
    protected static readonly string[] gameModeText = { "Game Mode", "Modo de Jogo" };
    protected static readonly string[] cardsText = { "Cards", "Cartas" };
    protected static readonly string[] helpText = { "Visual Help", "Ajuda Visual" };
    // User panel
    protected static readonly string[] userPanelText = { "User", "Usuário" };
    protected static readonly string[] insertUserText = { "Insert user", "Insira usuário" };
    protected static readonly string[] userWrongText = { "Name invalid or repeated", "Nome inválido ou repetido" };
    protected static readonly string[] insertInfoText = { "Insert any information (optional)", "Insira alguma informação (opcional)" };
    protected static readonly string[] addText = { "Add", "Adicionar" };
    protected static readonly string[] doneText = { "Done", "Feito" };
    protected static readonly string[] managerLoginText = { "Manager", "Administrador" };
    protected static readonly string[] enterPasswordText = { "Enter password...", "Insira a senha..." }; 
    protected static readonly string[] passwordWrongText = { "Password wrong!", "Senha errada!" }; 
    protected static readonly string[] loginText = { "Log In", "Entrar" };
    protected static readonly string[] logoutText = { "Log Out", "Sair" };
    protected static readonly string[] playerSelectText = { "Player Select", "Selecionar Jogador" };
    protected static readonly string[] sessionPlanText = { "Session Plan", "Plano da Sessão" };
    protected static readonly string[] chooseText = { "Choose", "Escolher" };
    protected static readonly string[] changeText = { "Change", "Mudar" };

    protected static readonly int numOfMembers = 2; // The N first member in the list
    public enum Limbs { rightHand, leftHand, rightFoot, leftFoot };
    protected static readonly string[,] limbTexts = 
        {
            {"Left foot", "Pé esquerdo" },
            {"Right foot", "Pé direito" },
            {"Left hand", "Mão esquerda" },
            {"Right hand", "Mão direita" }
        };

    public enum Status {
        begin,              // Begin of the game
        playing,            // Game running
        paused,             // Game paused
        end,                // End of the game
        newTurn,            // Starting new turn
        playerPlay,         // Waiting player start
        playerChoice,       // Waiting player choose
        right,              // Player choose the right
        wrong,               // Player choose the wrong
        endTurn,            // Turn has ended
        destroy,            // Destroy objects
        waitingMotion,      // Waiting current motion
        idle,               // No motion
        endGame,            // End current match
        connecting,         // Trying to connect to robot
        connected,          // Connected to robot
        disconnecting,      // Disconnecting to robot
        disconnected,       // Disconnected to robot
        calibrating         // Calibrating movement
    };

    protected static readonly string[,] gameMessageTexts =
    {
            {"New game", "Novo jogo"},
            {"Game running", "Jogo executando"},
            {"Game paused", "Jogo pausado"},
            {"Game ended", "Jogo encerrado"},
            {"New turn", "Novo turno"},
            {"Waiting player starts", "Esperando jogador iniciar"},
            {"Waiting player chooses", "Esperando jogador escolher"},
            {"Right choose", "Escolha certa"},
            {"Wrong choose", "Escolha errada"},
            {"Turn ended", "Turno terminando"},
            {"Destroying elements", "Destruindo elementos"},
            {"Moving", "Movendo"},
            {"Not Moving", "Não Movendo"},
            {"Match ended", "Partida encerrada"},
            {"Connecting", "Conectando"},
            {"Connected", "Conectado"},
            {"Disconnecting", "Desconectando"},
            {"Disconnected", "Desconectado"},
            {"Calibrating", "Calibrando"}
    };

    #endregion

    #region Other informations
    #endregion

    #region Public Variables    // Variables to be changed realtime
    public int fontSizeAdjust_S = 60;
    public int fontSizeAdjust_M = 100;
 //   [SerializeField]
//    private Languages languageAdjust = Languages.Portuguese;
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
 //       chosenLanguage = languageAdjust;
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
    /// Writes a time in a time format.
    /// </summary>
    /// <returns>The format.</returns>
    /// <param name="time">Time.</param>
    /// <param name="format">Format.</param>
    /// <param name="round">Round.</param>
    protected string TimeFormat(float time, string format, int round)
    {
        return Mathf.FloorToInt(time / 60).ToString ("D2") + ":" + (Round(time, round) % 60).ToString (format);
    }

    /// <summary>
    /// Rounds the specified numbers do certain number of decimal places.
    /// </summary>
    /// <param name="number">Real number.</param>
    /// <param name="decPlaces">Number of decimal places.</param>
    protected float Round (float number, int decPlaces)
    {
        int aux = Mathf.FloorToInt(number* Mathf.Pow (10f, (float)decPlaces));
        return aux / Mathf.Pow (10f, decPlaces);
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
    public static float LI (float x1, float y1, float x2, float y2, float x)
    {
        return ((y2 - y1) * x - x1 * y2 + x2 * y1) / (x2 - x1);
    }

    /// <summary>
    /// Retunrs a vector with [mean, standard deviation].
    /// </summary>
    /// <param name="list">List of values.</param>
    public Vector2 Statistics (List<float> list)
    {
        float avg = 0f;
        float sd = 0f;

        foreach (float number in list)
        {
            avg += number;
        }
        avg /= list.Count;

        foreach (float number in list)
        {
            sd += (number - avg) * (number - avg);
        }
        sd /= list.Count;
        sd = Mathf.Sqrt(sd);
        return new Vector2(avg, sd);
    }

    public List<Vector3> SplitVertical(float top, float offset, int number)
    {
        List<Vector3> positions = new List<Vector3>();
        if (number == 1)
        {
            positions.Add(new Vector3(0f, offset, 0f));
            return positions;
        }

        float step = Mathf.Abs(top) * 2f / (number - 1f);

        for (int iStep = 0; iStep < number; iStep++)
        {
            positions.Add(new Vector3(0f, offset, -Mathf.Abs(top) + step * iStep));
        }
        return positions;
    }

    /// <summary>
    /// Returns the array's values as string.
    /// </summary>
    static public string ArrayToString(float [] array)
    {
        string text = array[0].ToString();

        for (int i = 1; i < array.Length; i++)
        {
            text = text + ", " + array[i].ToString();
        }

        return text;
    }

    /// <summary>
    /// Verifies the closer angle among a array of angles.
    /// </summary>
    /// <returns>The closer angle index.</returns>
    /// <param name="angles">Array of Angles.</param>
    /// <param name="angle">Angle target.</param>
    static public int VerifyCloserAngle(float[] angles, float angle)
    {
        float diff = Mathf.Infinity;
        int index = Random.Range(0, angles.Length);

        for (int i = 0; i < angles.Length; i++)
        {
            if (Mathf.Abs(Mathf.DeltaAngle(angle, angles[i])) < diff)
            {
                diff = Mathf.Abs(Mathf.DeltaAngle(angle, angles[i]));
                index = i;
            }
        }

        return index;
    }

    static public float Atan2(float y, float x)
    {
        if (x == 0f)
        if (y > 0f)
            return Mathf.PI / 2f;
        else
            return Mathf.PI * 3f / 2f;
        else
            return Mathf.Atan2(y, x);
    }
}
