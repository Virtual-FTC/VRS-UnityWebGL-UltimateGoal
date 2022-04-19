using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugUI : MonoBehaviour
{
    public static DebugUI instance;

    public bool DebugMode = false;
    [SerializeField] private TMP_Text console;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void Display(string message)
    {
        if(!DebugMode) { return; }
        console.text = message;
    }
}
