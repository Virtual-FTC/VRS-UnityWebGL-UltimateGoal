using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vrs_messenger : MonoBehaviour
{
    public static vrs_messenger instance;
    private string playmode = "null";

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    public void SetPlaymode(string playmode)
    {
        Debug.Log("playmode = " + playmode);
        this.playmode = playmode;
    }

    public string GetPlaymode()
    {
        return playmode;
    }
}
