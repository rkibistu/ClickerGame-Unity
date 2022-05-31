using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
public class PlayerStatsCanvasController : MonoBehaviour
{

    enum Types { Health = 0, Mana, XP}

    [SerializeField]
    private Types _type;
    private Image _image;
    
    void Start()
    {
        _image = GetComponent<Image>();

        switch (_type) {
            case Types.Health:
                PlayerStats.Instance.OnHealthChanged += Handle_OnStatChanged;
                break;
            case Types.Mana:
                PlayerStats.Instance.OnManaChanged += Handle_OnStatChanged;
                break;
            case Types.XP:
                PlayerStats.Instance.OnXpChanged += Handle_OnStatChanged;
                break;
        }
        
    }

    private void Handle_OnStatChanged(object sender, StatsChangedEventArgs args) {

        _image.fillAmount = args._currentValue / args._maxValue;
    }
 
}
