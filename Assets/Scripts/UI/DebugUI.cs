using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugUI : MonoBehaviour
{
    public static DebugUI instance;

    private int counter;
    public int lineCount = 22;

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

    private void Start()
    {
        counter = 0;
    }

    public void Display(string message)
    {
        if(!DebugMode) { return; }
        counter++;
        if(counter>lineCount)
        {
            console.text = "";
            counter = 0;
        }
        console.text += $"\n{message}";
    }

    private void Update()
    {
        //console.text = "";
    }
}
