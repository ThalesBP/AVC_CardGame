using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnkleMovement : MonoBehaviour {

    private const float QUADRANTS = 0.70710678118654752440084436210485f;

    // Envelope do movimento
    public Vector2 max, min;            // Movement Extremes
    public Vector2 bases, origin;       // Elliptic Movement's parameters
    public float elipseScale = 1.0f;    // Scale for elipse resulting Obs: Check if is necessary

    public Vector3 Max
    {
        get
        {
            return Mathf.Rad2Deg * max;
        }
    }

    public Vector3 Min
    {
        get
        {
            return Mathf.Rad2Deg * min;
        }
    }

    public Vector3 Bases
    {
        get
        {
            return 2.0f * Mathf.Rad2Deg * bases;
        }
    }

    void Start()
    {
        Reset();
    }

    void Calibration(Vector2 position)
    {
  /*      if (max.y < position.y)
            max.y = position.y;
        if (max.x < position.x)
            max.x = position.x;
        if (min.y > position.y)
            min.y = position.y;
        if (min.x > position.x)
            min.x = position.x;
        bases = elipseScale * (max - min) / 2;
        origin = (max + min) / 2;
*/

        float range;
        float cosAng, sinAng;

        // ATAN2(((X-OX)*BY);((Y-OY)*BX))
        float ang = GameBase.Atan2 ((position.y - origin.y) * bases.x, (position.x - origin.x) * bases.y);
        float rangeLin;

        cosAng = Mathf.Cos(ang);
        sinAng = Mathf.Sin(ang);

        float basesLinX, basesLinY;
        float cosAngLin, sinAngLin;
        Vector2 originLin;

        Vector2 opposite = origin - new Vector2(bases.x * cosAng, bases.y * sinAng);

        if (Mathf.Abs(cosAng) < Mathf.Abs(sinAng))
        {
            // (Y - OY)/SIN(T)/BY
            range = ((position.y - origin.y) / sinAng / bases.y);
            rangeLin = (position.y - opposite.y) / Mathf.Pow(sinAng, 3) - 2f * bases.y / Mathf.Pow(sinAng, 2);

   /*         basesLinX = rangeLin * cosAng * cosAng / 2f + bases.x;
            cosAngLin = (position.x - opposite.x) / (2 * basesLinX);
            sinAngLin = Mathf.Sqrt(1f - cosAngLin * cosAngLin);
            basesLinY = (position.y - opposite.y) / (2 * sinAng);*/
            originLin = (position + opposite) / 2f;

            basesLinX = bases.x * (2f * (range - 1) * cosAng + 1);
            basesLinY = Mathf.Abs((position.y - originLin.y) * basesLinX / Mathf.Sqrt(basesLinX * basesLinX - Mathf.Pow(position.x - originLin.x, 2f)));
        }
        else
        {
            // (X - OX)/COS(T)/BX
            range = ((position.x - origin.x) / cosAng / bases.x);
            rangeLin = (position.x - opposite.x) / Mathf.Pow(cosAng, 3) - 2f * bases.x / Mathf.Pow(cosAng, 2);

      /*      basesLinY = rangeLin * sinAng * sinAng / 2f + bases.y;
            sinAngLin = (position.y - opposite.y) / (2 * basesLinY);
            cosAngLin = Mathf.Sqrt(1f - sinAngLin * sinAngLin);
            basesLinX = (position.x - opposite.x) / (2 * cosAng);*/
            originLin = (position + opposite) / 2f;

            basesLinY = bases.y * (2f * (range - 1) * sinAng + 1);
            basesLinX = Mathf.Abs((position.x - originLin.x) * basesLinY / Mathf.Sqrt(basesLinY * basesLinY - Mathf.Pow(position.y - originLin.y, 2f)));
        }

        if (range > 1f)
        {
            origin = (position + opposite) / 2f;

//            bases = new Vector2(rangeLin * cosAng * cosAng / 2f + bases.x, rangeLin * sinAng * sinAng / 2f + bases.y);
            bases = new Vector2(basesLinX, basesLinY);
        }
    }

    public void SetRadius(float radius)
    {
        float absRadius = Mathf.Abs(radius);

        min = new Vector2(-absRadius, -absRadius);
        max = new Vector2(absRadius, absRadius);
        bases = max;
        origin = Vector2.zero;
    }

    public void Reset()
    {
        SetRadius(Mathf.Epsilon);
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
        float ang = GameBase.Atan2 ((position.y - origin.y) * bases.x, (position.x - origin.x)*bases.y);

        cosAng = Mathf.Cos(ang);
        sinAng = Mathf.Sin(ang);

        if (Mathf.Abs(cosAng) < Mathf.Abs(sinAng))
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

        float range;
        float cosAng, sinAng;
        Vector2 circle = Vector2.zero;

        // ATAN2(((X-OX)*BY);((Y-OY)*BX))
        float ang = GameBase.Atan2 ((position.y - origin.y) * bases.x, (position.x - origin.x)*bases.y);

        cosAng = Mathf.Cos(ang);
        sinAng = Mathf.Sin(ang);

        if (Mathf.Abs(cosAng) < Mathf.Abs(sinAng))
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
    public Vector2 SquareToElipse(Vector2 position, float sideSize)
    {
        float range;
//        float cosAng, sinAng;
        Vector2 elipse = Vector2.zero;

        // ATAN2(((X-OX)*BY);((Y-OY)*BX))
   //     float ang = GameBase.Atan2 (position.y, position.x);

  //      cosAng = Mathf.Cos(ang);
  //      sinAng = Mathf.Sin(ang);

        range = Mathf.Abs(position.x) > Mathf.Abs(position.y) ?
            Mathf.Abs(position.x / sideSize) :
            Mathf.Abs(position.y / sideSize);

        elipse = origin + range * Vector2.Scale(position.normalized, bases);

  //      elipse.x = origin.x + range * cosAng * bases.x; // / elipseScale;
  //      elipse.y = origin.y + range * sinAng * bases.y; // / elipseScale;
        return (elipse);
    }

    /// <summary>
    /// Transforms a position in Square Space to Elliptic Space.
    /// </summary>
    /// <returns>The position in Elliptic Space.</returns>
    /// <param name="position">Position in Square Space.</param>
    public Vector2 CircleToElipse(Vector2 position, float sideSize)
    {
        float range;
//        float cosAng, sinAng;
        Vector2 elipse = Vector2.zero;


        // ATAN2(((X-OX)*BY);((Y-OY)*BX))
    //    float ang = GameBase.Atan2 (position.y, position.x);

  //      cosAng = Mathf.Cos(ang);
//        sinAng = Mathf.Sin(ang);

        range = position.magnitude / sideSize;
        elipse = origin + range * Vector2.Scale(position.normalized, bases);

        //elipse = new Vector2(origin.x + range * cosAng * bases.x, elipse.y = origin.y + range * sinAng * bases.y); // / elipseScale;
        return (elipse);
    }

    //void PlayerHelper();

    //void PlayerDisturber();
}
