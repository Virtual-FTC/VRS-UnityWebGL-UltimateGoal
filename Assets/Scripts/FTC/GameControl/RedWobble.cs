using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedWobble : MonoBehaviour
{
    public bool isScoring;

    public void ScoreWobble(int points)
    {
        if (!isScoring)
        {
            ScoreKeeper._Instance.addScoreRed(points);
            isScoring = true;
        }
    }

    /* the parameter points should be negative when the method is called
     */
    public void UnscoreWobble(int points)
    {
        if (isScoring)
        {
            ScoreKeeper._Instance.addScoreRed(points);
            isScoring = false;
        }
    }
}
