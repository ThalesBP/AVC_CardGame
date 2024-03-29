﻿using System.Collections;
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
 /*   public static float averageTimeToChoose = 0f;
    public static float averageTimeToPlay = 0f;
    public static float averageTimeToMemorize = 0f;
    public static float[] rangeOfTimeToChoose = { 0f, 0f };
    public static float[] rangeOfTimeToPlay = { 0f, 0f };
    public static float[] rangeOfTimeToMemorize = { 0f, 0f };*/

    public int[] objectiveCard = new int[3], choiceCard = new int[3];
    public bool suitMatch, valueMatch, colorMatch, match;
    public int pointMatch;
    public int order;
    public int numOptions;
/*    public float timeToChoose;
    public float timeToPlay;
    public float timeToMemorize;*/

    public static float AverageTimeToChoose = 0f;
    public static float AverageTimeToPlay = 0f;
    public static float AverageTimeToMemorize = 0f;
    public static float TimeToRead = 0f;

    public static float[] RangeTimeToChoose = {float.PositiveInfinity, float.NegativeInfinity};
    public static float[] RangeTimeToPlay = {float.PositiveInfinity, float.NegativeInfinity};
    public static float[] RangeTimeToMemorize = {float.PositiveInfinity, float.NegativeInfinity};

    public float TimeToChoose = 0f;
    public float TimeToPlay = 0f;
    public float TimeToMemorize = 0f;
    public float choiceChanged = 0f;

    private const int suitScore = 5;
    private const int colorScore = 5;
    private const int valueScore = 10;
    private const int extraScore = 10;
    private const int timeScore = 10;

    /// <summary>
    /// Initializes a new instance of the <see cref="Choice"/> class comparing the card chosen with objective card and the number of options.
    /// </summary>
    /// <param name="objective">Objective card.</param>
    /// <param name="choice">Chosen card.</param>
    /// <param name="nOptions">Number of card options.</param>
    public Choice(Card objective, Card choice, int nOptions, float timeToChoose, float timeToPlay, float timeToMemorize)
    {
        pointMatch = 0;
        numOptions = nOptions;
        objectiveCard[0] = objective.value;
        objectiveCard[1] = objective.suit;
        objectiveCard[2] = objective.color;
        choiceCard[0] = choice.value;
        choiceCard[1] = choice.suit;
        choiceCard[2] = choice.color;

        suitMatch = objective.suit == choice.suit;
        valueMatch = objective.value == choice.value;
        colorMatch = objective.color == choice.color;

        if (suitMatch)
        {
            pointMatch += suitScore;
            suitCounter++;
        } 
        if (colorMatch)
        {
            pointMatch += colorScore;
            colorCounter++;
        }
        if (valueMatch)
        {
            pointMatch += valueScore;
            valueCounter++;
        }

        // Extra point for exact answer
        if (pointMatch == suitScore + colorScore + valueScore)
        {
            match = true;
            pointMatch += extraScore;
            totalMatches++;
        
            // Extra point for faster answer
            pointMatch += Mathf.Clamp(Mathf.FloorToInt((timeScore + 1f - 2f * timeToChoose)), 0, timeScore);
        }
        else
            match = false;
        /*
        this.timeToChoose = timeToChoose;
        this.timeToPlay = timeToPlay;
        this.timeToMemorize = timeToMemorize;

        rangeOfTimeToChoose = CheckExtremes(rangeOfTimeToChoose, timeToChoose);
        rangeOfTimeToPlay = CheckExtremes(rangeOfTimeToPlay, timeToPlay);
        rangeOfTimeToMemorize = CheckExtremes(rangeOfTimeToMemorize, timeToPlay);

        averageTimeToChoose = (averageTimeToChoose * orderCounter + timeToChoose) / (orderCounter + 1);
        averageTimeToPlay = (averageTimeToPlay * orderCounter + timeToPlay) / (orderCounter + 1);
        averageTimeToMemorize = (averageTimeToMemorize * orderCounter + timeToPlay) / (orderCounter + 1);
        */

        TimeToChoose = timeToChoose;
        TimeToPlay = timeToPlay;
        TimeToMemorize = timeToMemorize;

        AverageTimeToChoose = GameBase.Average(AverageTimeToChoose, timeToChoose, orderCounter);
        AverageTimeToPlay = GameBase.Average(AverageTimeToPlay, timeToPlay, orderCounter);
        AverageTimeToMemorize = GameBase.Average(AverageTimeToMemorize, timeToMemorize, orderCounter);

        RangeTimeToChoose = GameBase.CheckExtremes(timeToChoose, RangeTimeToChoose);
        RangeTimeToPlay = GameBase.CheckExtremes(timeToPlay, RangeTimeToPlay);
        RangeTimeToMemorize = GameBase.CheckExtremes(timeToMemorize, RangeTimeToMemorize);

        order = orderCounter;
        orderCounter++;
        totalPoints += pointMatch;
    }

    public Choice () 
    {
        objectiveCard = new int[3];
        choiceCard = new int[3];
        pointMatch = order = numOptions = 0;

        suitMatch = false;
        valueMatch = false;
        colorMatch = false;
        match = false;

        TimeToChoose = 0f;
        TimeToPlay = 0f;
        TimeToMemorize = 0f;
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

        AverageTimeToChoose = 0f;
        AverageTimeToPlay = 0f;
        AverageTimeToMemorize = 0f;
        TimeToRead = 0f;

        RangeTimeToChoose = new float[] {float.PositiveInfinity, float.NegativeInfinity};
        RangeTimeToPlay = new float[] {float.PositiveInfinity, float.NegativeInfinity};
        RangeTimeToMemorize = new float[] {float.PositiveInfinity, float.NegativeInfinity};
    }

    public static int CheckPoints(Card a, Card b)
    {
        int match = 0;

        if (a.suit == b.suit)
            match += suitScore;
        if (a.value == b.value)
            match += valueScore;
        if (a.color == b.color)
            match += colorScore;

        if (match == suitScore + valueScore + colorScore)
            match += extraScore + Mathf.RoundToInt(timeScore/2f);

        return match;
    }
}

