using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListItem : MonoBehaviour
{
    [SerializeField] private Text nameLabel;
    public Button btn_up, btn_down;

    public void Setup(string Name)
    {
        nameLabel.text = Name;
    }
}
