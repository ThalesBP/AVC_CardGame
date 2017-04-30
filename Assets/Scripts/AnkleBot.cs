using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnkleBot : MonoBehaviour {

    private const float QUADRANTS = 0.70710678118654752440084436210485f;

    // Envelope do movimento
    public Vector2 max, min;        // Input for elipse
    public Vector2 bases, origin;   // Elipse's parameters
    public float elipseScale;       // Scale for fitting the moves
    public float squareScale = 1f;
    public float boundaryDist;


	void Start () 
    {
		
	}
	
	void Update () 
    {
		
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

    Vector2 ElipseToSquare(Vector2 elipse)
    {
        float range, r;
        float cosAng, sinAng;
        Vector2 square = Vector2.zero;

        // ATAN2(((X-OX)*BY);((Y-OY)*BX))
        float ang = Mathf.Atan2 ((elipse.y - origin.y) * bases.x, (elipse.x - origin.x)*bases.y);

        cosAng = Mathf.Cos(ang);
        sinAng = Mathf.Sin(ang);

        if (Mathf.Abs(cosAng) < Mathf.Epsilon)
            // (Y - OY)/SIN(T)/BY
            range = ((elipse.y - origin.y)/sinAng/bases.y);
        else
            // (X - OX)/COS(T)/BX
            range = ((elipse.x - origin.x)/cosAng/bases.x);

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

    Vector2 SquareToElipse(Vector2 square)
    {
        float range;
        float cosAng, sinAng;
        Vector2 elipse = Vector2.zero;

        // ATAN2(((X-OX)*BY);((Y-OY)*BX))
        float ang = Mathf.Atan2 (square.y, square.x);

        cosAng = Mathf.Cos(ang);
        sinAng = Mathf.Sin(ang);

        range = Mathf.Abs(square.x) > Mathf.Abs(square.y) ?
            Mathf.Abs(square.x / boundaryDist) :
            Mathf.Abs(square.y / boundaryDist);

        elipse.x = origin.x + range * cosAng * bases.x; // / elipseScale;
        elipse.y = origin.y + range * sinAng * bases.y; // / elipseScale;
        return (elipse);
    }


    //void PlayerHelper();

    //void PlayerDisturber();
}
