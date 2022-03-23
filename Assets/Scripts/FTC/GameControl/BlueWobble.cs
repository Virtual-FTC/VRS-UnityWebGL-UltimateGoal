using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueWobble : Wobble
{
    private bool isTeleopLineScoring;
    private bool isTeleopDropScoring;
    private bool isScoring;
    private int points = 0;

    /* method for scoring in auto
     */
    public void ScoreWobble(int points)
    {
        if (!isScoring)
        {
            ScoreKeeper._Instance.addScoreBlue(points);
            isScoring = true;
            this.points = points;
        }
    }

    /* method for scoring in auto
     * the parameter points should be negative when the method is called
     */
    public void UnscoreWobble()
    {
        if (isScoring)
        {
            ScoreKeeper._Instance.addScoreBlue(-points);
            isScoring = false;
        }
    }

    /* This is for scoring in the last 30 seconds of teleop
     */
    public void ScoreWobbleTeleop(string goalType, int points)
    {
        if (!isTeleopLineScoring && goalType == "line")
        {
            ScoreKeeper._Instance.addScoreBlue(points);
            isTeleopLineScoring = true;
            this.points = points;
        }
        if (!isTeleopDropScoring && goalType == "drop")
        {
            ScoreKeeper._Instance.addScoreBlue(points);
            isTeleopDropScoring = true;
            this.points = points;
        }
    }

    /* This is for unscoring in the last 30 seconds of teleop
     * the parameter points should be negative when the method is called
     */
    public void UnscoreWobbleTeleop(string goalType)
    {
        if (isTeleopLineScoring && goalType == "line")
        {
            ScoreKeeper._Instance.addScoreBlue(-points);
            isTeleopLineScoring = false;
        }
        if (isTeleopDropScoring && goalType == "drop")
        {
            ScoreKeeper._Instance.addScoreBlue(-points);
            isTeleopDropScoring = false;
        }
    }
}
