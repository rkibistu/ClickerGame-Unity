using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDamageStats : MonoBehaviour
{
    enum Types { Damage = 0, PoisonDamage, BurnDamage, ManaRegen, HealthRegen, Mana, Health}

    [SerializeField]
    private Types _type;
    private Text _text;
    void Start()
    {
        _text = GetComponent<Text>();

        switch (_type) {
            case Types.Damage:
                PlayerStats.Instance.OnDamageChanged += Handle_OnDamageChanged;
                _text.text = "Damage: " + PlayerStats.Instance.CurrentDamage;
                break;
            case Types.PoisonDamage:
                break;
            case Types.BurnDamage:
                break;
            case Types.ManaRegen:
                PlayerStats.Instance.OnManaRegenChanged += Handle_OnRegenChanged;
                _text.text = PlayerStats.Instance._currentManaRegen + "/s";
                break;
            case Types.HealthRegen:
                PlayerStats.Instance.OnHealthRegenChanged += Handle_OnRegenChanged;
                _text.text = PlayerStats.Instance._currentHealthRegen + "/s";
                break;
            case Types.Mana:
                PlayerStats.Instance.OnManaChanged += Handle_OnManaOrHealthChanged;
                _text.text = PlayerStats.Instance._currentMana.ToString();
                break;
            case Types.Health:
                PlayerStats.Instance.OnHealthChanged += Handle_OnManaOrHealthChanged;
                _text.text = PlayerStats.Instance._currentHealth.ToString();
                break;

        }
    }

    private void Handle_OnDamageChanged(object sender, StatsChangedEventArgs args) {

        _text.text = "Damage: " + args._currentValue;
    }
    private void Handle_OnRegenChanged(object sender, StatsChangedEventArgs args) {

        _text.text = args._currentValue + "/s";
    }
    private void Handle_OnManaOrHealthChanged(object sender, StatsChangedEventArgs args) {
        _text.text = args._currentValue.ToString();
    }

}
