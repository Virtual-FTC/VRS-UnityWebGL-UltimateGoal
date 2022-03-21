using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class ScoreKeeper : MonoBehaviour
{
    public static ScoreKeeper _Instance;
    
    public Text redScoreText;
    public Text blueScoreText;

    [Header("Score Values")]
    public int WobbleAuto = 15;
    public int WobbleLine = 5;
    public int WobbleDrop = 20;
    public int PowerGoal = 15;
    public int FreeplayRingHigh = 6;
    public int FreeplayRingMid = 4;
    public int FreeplayRingLow = 2;
    public int AutoRingHigh = 12;
    public int AutoRingMid = 6;
    public int AutoRingLow = 3;
    public int WhiteLineGoal = 5;

    [HideInInspector]
    public PhotonView thisView;

    private int redScore = 0;
    private int blueScore = 0;

    private bool freeze = false;

    public Light[] lights;

    private void Awake()
    {
        thisView = GetComponent<PhotonView>();
        if (_Instance == null)
            _Instance = this;
        else
            Destroy(this);
    }

    public void addScoreRed(int points)
    {
        if (!PhotonNetwork.IsConnected)
            addScoreRedHelper(points);
        else
            thisView.RPC("addScoreRedHelper", RpcTarget.AllBuffered, points);
    }

    [PunRPC]
    public void addScoreRedHelper(int points)
    {
        if (freeze)
            return;
        redScore += points;
        updateRedScore();
        if (redScore > blueScore)
            setLightsRed();
        else if (blueScore > redScore)
            setLightsBlue();
        else
            setLightsNorm();
    }

    public void addScoreBlue(int points)
    {
        if (!PhotonNetwork.IsConnected)
        {
            addScoreBlueHelper(points);
        }
        else
        {
            thisView.RPC("addScoreBlueHelper", RpcTarget.AllBuffered, points);
        } 
    }

    [PunRPC]
    public void addScoreBlueHelper(int points)
    {
        if (freeze)
            return;
        blueScore += points;
        updateBlueScore();
        if (redScore > blueScore)
            setLightsRed();
        else if (blueScore > redScore)
            setLightsBlue();
        else
            setLightsNorm();
    }

    public int getScoreRed()
    {
        return redScore;
    }

    public int getScoreBlue()
    {
        return blueScore;
    }

    void updateRedScore()
    {
        redScoreText.text = "" + redScore;
    }

    void updateBlueScore()
    {
        blueScoreText.text = "" + blueScore;
    }

    public void resetScore()
    {
        if (PhotonNetwork.IsConnected)
            thisView.RPC("resetScoreHelper", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void resetScoreHelper()
    {
        freeze = false;
        redScore = 0;
        blueScore = 0;
        updateRedScore();
        updateBlueScore();

        setLightsNorm();
    }

    [PunRPC]
    public void freezeScore()
    {
        freeze = true;
    }

    public void setLightsNorm()
    {
        /*
        Color myColor = new Color();
        var color = "#846032";
        ColorUtility.TryParseHtmlString(color, out myColor);
        for (int x = 0; x < lights.Length; x++)
        {
            lights[x].color = myColor;
        }
        */
    }

    public void setLightsBlue()
    {
        /*
        Color myColor = new Color();
        var color = "#323E84";
        ColorUtility.TryParseHtmlString(color, out myColor);
        for (int x = 0; x < lights.Length; x++)
        {
            lights[x].color = myColor;
        }
        */
    }

    public void setLightsRed()
    {
        /*
        Color myColor = new Color();
        var color = "#A93C4E";
        ColorUtility.TryParseHtmlString(color, out myColor);
        for (int x = 0; x < lights.Length; x++)
        {
            lights[x].color = myColor;
        }
        */
    }

    public void setLightsGreen()
    {
        /*
        Color myColor = new Color();
        var color = "#41A83C";
        ColorUtility.TryParseHtmlString(color, out myColor);
        for (int x = 0; x < lights.Length; x++)
        {
            lights[x].color = myColor;
        }
        */
    }
}
