using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueWobble : MonoBehaviour
{
    public bool isScoring;

    public void ScoreWobble(int points)
    {
        if (!isScoring)
        {
            ScoreKeeper._Instance.addScoreBlue(points);
            isScoring = true;
        }
    }

    /* the parameter points should be negative when the method is called
     */
    public void UnscoreWobble(int points)
    {
        if (isScoring)
        {
            ScoreKeeper._Instance.addScoreBlue(points);
            isScoring = false;
        }
    }
}
