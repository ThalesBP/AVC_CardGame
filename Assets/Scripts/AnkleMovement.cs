using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnkleMovement : MonoBehaviour {

    private const float QUADRANTS = 0.70710678118654752440084436210485f;

    // Envelope do movimento
    public Vector2 max, min;            // Movement Extremes
    public Vector2 bases, origin;       // Elliptic Movement's parameters
    public float elipseScale = 1.0f;    // Scale for elipse resulting Obs: Check if is necessary

    void Start()
    {
        min = max = bases = origin = Vector2.zero;
    }

    void Calibration(Vector2 position)
    {
        if (max.y < position.y)
            max.y = position.y;
        if (max.x < position.x)
            max.x = position.x;
        if (min.y > position.y)
            min.y = position.y;
        if (min.x > position.x)
            min.x = position.x;
        bases = elipseScale * (max - min) / 2;
        origin = (max + min) / 2;
    }

    /// <summary>
    /// Transforms a position in Elliptic Space to Square Space with side size one.
    /// </summary>
    /// <returns>The position in Square Space.</returns>
    /// <param name="position">Position in Elliptic Space.</param>
    public Vector2 ElipseToSquare(Vector2 position)
    {
        Calibration(position);

        float range, r;
        float cosAng, sinAng;
        Vector2 square = Vector2.zero;

        // ATAN2(((X-OX)*BY);((Y-OY)*BX))
        float ang = Mathf.Atan2 ((position.y - origin.y) * bases.x, (position.x - origin.x)*bases.y);

        cosAng = Mathf.Cos(ang);
        sinAng = Mathf.Sin(ang);

        if (Mathf.Abs(cosAng) < Mathf.Epsilon)
            // (Y - OY)/SIN(T)/BY
            range = ((position.y - origin.y)/sinAng/bases.y);
        else
            // (X - OX)/COS(T)/BX
            range = ((position.x - origin.x)/cosAng/bases.x);

        if (Mathf.Abs(cosAng) < QUADRANTS)
        {
            r = Mathf.Abs(1f/sinAng);
            square.x = range*r*cosAng;
            square.y = range*Mathf.Sign(sinAng);
        }
        else
        {
            r = Mathf.Abs(1f/cosAng);
            square.x = range*Mathf.Sign(cosAng);
            square.y = range*r*sinAng;
        }
        return (square);
    }

    /// <summary>
    /// Transforms a position in Elliptic Space to Circle Space with radius size one.
    /// </summary>
    /// <returns>The position in Square Space.</returns>
    /// <param name="position">Position in Elliptic Space.</param>
    public Vector2 ElipseToCircle(Vector2 position)
    {
        Calibration(position);

        float range, r;
        float cosAng, sinAng;
        Vector2 circle = Vector2.zero;

        // ATAN2(((X-OX)*BY);((Y-OY)*BX))
        float ang = Mathf.Atan2 ((position.y - origin.y) * bases.x, (position.x - origin.x)*bases.y);

        cosAng = Mathf.Cos(ang);
        sinAng = Mathf.Sin(ang);

        if (Mathf.Abs(cosAng) < Mathf.Epsilon)
            // (Y - OY)/SIN(T)/BY
            range = ((position.y - origin.y)/sinAng/bases.y);
        else
            // (X - OX)/COS(T)/BX
            range = ((position.x - origin.x)/cosAng/bases.x);

        circle = new Vector2(cosAng * range, sinAng * range);

        return (circle);
    }

    /// <summary>
    /// Transforms a position in Square Space to Elliptic Space.
    /// </summary>
    /// <returns>The position in Elliptic Space.</returns>
    /// <param name="position">Position in Square Space.</param>
    Vector2 SquareToElipse(Vector2 position, float sideSize)
    {
        float range;
        float cosAng, sinAng;
        Vector2 elipse = Vector2.zero;

        // ATAN2(((X-OX)*BY);((Y-OY)*BX))
        float ang = Mathf.Atan2 (position.y, position.x);

        cosAng = Mathf.Cos(ang);
        sinAng = Mathf.Sin(ang);

        range = Mathf.Abs(position.x) > Mathf.Abs(position.y) ?
            Mathf.Abs(position.x / sideSize) :
            Mathf.Abs(position.y / sideSize);

        elipse.x = origin.x + range * cosAng * bases.x; // / elipseScale;
        elipse.y = origin.y + range * sinAng * bases.y; // / elipseScale;
        return (elipse);
    }

    /// <summary>
    /// Transforms a position in Square Space to Elliptic Space.
    /// </summary>
    /// <returns>The position in Elliptic Space.</returns>
    /// <param name="position">Position in Square Space.</param>
    Vector2 CircleToElipse(Vector2 position, float sideSize)
    {
        float range;
        float cosAng, sinAng;
        Vector2 elipse = Vector2.zero;

        // ATAN2(((X-OX)*BY);((Y-OY)*BX))
        float ang = Mathf.Atan2 (position.y, position.x);

        cosAng = Mathf.Cos(ang);
        sinAng = Mathf.Sin(ang);

        range = position.magnitude;

        elipse.x = origin.x + range * cosAng * bases.x; // / elipseScale;
        elipse.y = origin.y + range * sinAng * bases.y; // / elipseScale;
        return (elipse);
    }

    //void PlayerHelper();

    //void PlayerDisturber();
}
