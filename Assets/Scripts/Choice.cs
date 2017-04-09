using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Choice represents a unique player's choice.
/// </summary>
public class Choice {

    public static int suitCounter, valueCounter, colorCounter = 0;
    public static int totalPoints = 0;
    public static int orderCounter = 0;
    public static int totalMatches = 0;
    public static float precision = 0f;
    public static float averageTimeToChoose = 0f;
    public static float[] rangeOfTime = { 6000f, 0f };

    public string objectiveCard = "";
    public string choiceCard = "";
    public bool suitMatch, valueMatch, colorMatch;
    public int pointMatch;
    public int order;
    public int numOptions;
    public float timeToChoose;

    /// <summary>
    /// Initializes a new instance of the <see cref="Choice"/> class comparing the card chosen with objective card and the number of options.
    /// </summary>
    /// <param name="objective">Objective card.</param>
    /// <param name="choice">Chosen card.</param>
    /// <param name="nOptions">Number of card options.</param>
    public Choice(Card objective, Card choice, int nOptions, float timeToChoose)
    {
        pointMatch = 0;
        objectiveCard = objective.ToString(0);
        choiceCard = choice.ToString(0);

        suitMatch = objective.suit == choice.suit;
        valueMatch = objective.value == choice.value;
        colorMatch = objective.color == choice.color;

        if (suitMatch)
        {
            pointMatch += 10;
            suitCounter++;
        }
        if (valueMatch)
        {
            pointMatch += 15;
            valueCounter++;
        }
        if (colorMatch)
        {
            pointMatch += 5;
            colorCounter++;
        }

        if (pointMatch == 30)
            totalMatches++;

        this.timeToChoose = timeToChoose;
        if (timeToChoose < rangeOfTime[0])
            rangeOfTime[0] = timeToChoose;
        if (timeToChoose > rangeOfTime[1])
            rangeOfTime[1] = timeToChoose;

        averageTimeToChoose = (averageTimeToChoose * orderCounter + timeToChoose) / (orderCounter + 1);
        order = orderCounter;
        orderCounter++;
        totalPoints += pointMatch;
    }

    public Choice () 
    {
        objectiveCard = choiceCard = "null";
        pointMatch = 0;

        suitMatch = false;
        valueMatch = false;
        colorMatch = false;
    }

    public static bool operator ==(Choice A, Choice B)
    {
        if ((A.suitMatch == B.suitMatch) && (A.valueMatch == B.valueMatch) && (A.colorMatch == B.colorMatch))
            return true;
        else
            return false;
    }

    public static bool operator !=(Choice A, Choice B)
    {
        if ((A.suitMatch != B.suitMatch) || (A.valueMatch != B.valueMatch) || (A.colorMatch != B.colorMatch))
            return true;
        else
            return false;
    }

    public static implicit operator int(Choice choice)
    {
        return choice.pointMatch;
    }

    public static implicit operator bool(Choice choice)
    {
        if (choice.pointMatch == 30)
            return true;
        else
            return false;
    }

    public override bool Equals(object choice)
    {
        try 
        {
            return (this == (Choice)choice);
        }
        catch
        {
            return false;
        }
    }

    public override int GetHashCode()
    {
        return (pointMatch);
    }

    public static int CountMatches(List<Choice> choices)
    {
        int numOfMatches = 0;
        foreach (Choice choice in choices)
        {
            if (choice)
                numOfMatches++;
        }
        return numOfMatches;
    }

    public static void ResetChoice()
    {
        suitCounter = valueCounter = colorCounter = 0;
        totalPoints = 0;
        orderCounter = 0;
        totalMatches = 0;
        precision = 0f;
        averageTimeToChoose = 0f;
        rangeOfTime = new float[] { 6000f, 0f };
    }
}
