using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource endAuto;
    public AudioSource startTeleop;
    public AudioSource startEndGame;
    public AudioSource endMatch;

    public AudioSource eStop;

    private bool playedStartAuto = false;
    private bool playedCountDown = false;
    private bool playedCountDown2 = false;
    private bool playedEndAuto = false;
    private bool playedStartTeleop = false;
    private bool playedStartEndGame = false;
    private bool playedEndMatch = false;

    // Ring sounds
    public AudioSource ringBounce;
    public AudioSource ringImpact;

    void Start()
    {

    }

    public bool playCountDown()
    {
        if (!playedCountDown)
        {
            playedCountDown = true;
            return true;
        }
        return false;
    }

    public bool playCountDown2()
    {
        if (!playedCountDown2)
        {
            playedCountDown2 = true;
            return true;
        }
        return false;
    }

    public bool playStartAuto()
    {
        if (!playedStartAuto)
        {
            playedStartAuto = true;
            return true;
        }
        return false;
    }

    public bool playEndAuto()
    {
        if (!playedEndAuto)
        {
            endAuto.Play();
            playedEndAuto = true;
        }
        return endAuto.isPlaying;
    }

    public bool playStartTeleop()
    {
        if (!playedStartTeleop)
        {
            startTeleop.Play();
            playedStartTeleop = true;
        }
        return startTeleop.isPlaying;
    }

    public bool playStartEndGame()
    {
        if (!playedStartEndGame)
        {
            startEndGame.Play();
            playedStartEndGame = true;
        }
        return startEndGame.isPlaying;
    }

    public bool playEndMatch()
    {
        if (!playedEndMatch)
        {
            endMatch.Play();
            playedEndMatch = true;
        }
        return endMatch.isPlaying;
    }

    public void playEStop()
    {
        eStop.Play();
    }

    public void reset()
    {
        playedStartAuto = false;
        playedEndAuto = false;
        playedStartTeleop = false;
        playedStartEndGame = false;
        playedEndMatch = false;
    }

    public void playRingBounce()
    {
        ringBounce.Play();
    }

    public void playRingImpact()
    {
        ringImpact.volume = 0.1f;
        ringImpact.Play();
    }
}
