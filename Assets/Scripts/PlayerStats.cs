using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerStats : MonoBehaviour {
    [SerializeField]
    private float _maxHealth = 100;
    [HideInInspector]
    public float _currentHealth;
    public event EventHandler<StatsChangedEventArgs> OnHealthChanged;
    public float _currentHealthRegen;
    public event EventHandler<StatsChangedEventArgs> OnHealthRegenChanged;

    [SerializeField]
    private float _maxMana = 80;
    [HideInInspector]
    public float _currentMana;
    public event EventHandler<StatsChangedEventArgs> OnManaChanged;
    public float _currentManaRegen;
    public event EventHandler<StatsChangedEventArgs> OnManaRegenChanged;

    [SerializeField]
    private float _maxXp = 1000;
    [HideInInspector]
    public float _currentXp;
    private int _currentLevel;
    public event EventHandler<StatsChangedEventArgs> OnXpChanged;

    [SerializeField]
    private float _currentDamage;
    public event EventHandler<StatsChangedEventArgs> OnDamageChanged;

    public float _currentCritChange;
    public event EventHandler<StatsChangedEventArgs> OnCritChangeChanged;
    public float _currentCritMultiplier;
    public event EventHandler<StatsChangedEventArgs> OnCritMultiplierChanged;



    private PlayerStats() {; }
    private static PlayerStats _instance = null;
    public static PlayerStats Instance {
        get {
            if (_instance == null)
                _instance = new PlayerStats();
            return _instance;
        }
    }

    private void Awake() {

        _instance = this;
    }

    void Start() {

        _currentHealth = _maxHealth;
        _currentMana = _maxMana;
        _currentXp = 0f;
        _currentLevel = 0;

        StartCoroutine(Regenerate());
    }

    IEnumerator Regenerate() {

        while (true) {
            Heal(_currentHealthRegen);
            RegainMana(_currentManaRegen);
            yield return new WaitForSeconds(1);
        }
        
    }

    private void Die() {

        Debug.Log("AI MUUUURIT FRA!!" + _currentHealth);
        StopAllCoroutines();
        PlayerController.Instance.Die();
    }
    private void LevelUP() {

        _currentXp = _currentXp - _maxXp;
        _maxXp += 0.1f * _maxXp;
        _currentLevel++;

        //la fiecare level alegi upgreade nou
        UpgradesController.Instance.SetActive(true);
        PlayerController.Instance.StartIdle();
    }
    public float Damaged(float value) {

        _currentHealth = Mathf.Max(_currentHealth - value, 0f);
        if (OnHealthChanged != null) {

            StatsChangedEventArgs args = new StatsChangedEventArgs();
            args._currentValue = _currentHealth;
            args._maxValue = _maxHealth;
            OnHealthChanged(this, args);
        }
        if (_currentHealth <= 0)
            Die();

            return _currentHealth;
    }
    public float Heal(float value) {

        _currentHealth = Mathf.Min(_currentHealth + value, _maxHealth);
        if (OnHealthChanged != null) {

            StatsChangedEventArgs args = new StatsChangedEventArgs();
            args._currentValue = _currentHealth;
            args._maxValue = _maxHealth;
            OnHealthChanged(this, args);
        }

        return _currentHealth;
    }
    public float AddHealthRegen(int value) {

        _currentHealthRegen += value;
        if (OnHealthRegenChanged != null) {

            StatsChangedEventArgs args = new StatsChangedEventArgs();
            args._currentValue = _currentHealthRegen;
            OnHealthRegenChanged(this, args);
        }

        return _currentHealthRegen;
    }
    public float AddMaxHealth(int value) {

        _maxHealth += value;
        _currentHealth = Mathf.Min(_maxHealth, _currentHealth + value);
        if (OnHealthChanged != null) {

            StatsChangedEventArgs args = new StatsChangedEventArgs();
            args._currentValue = _currentHealth;
            args._maxValue = _maxHealth;
            OnHealthChanged(this, args);
        }

        return _maxHealth;
    }
    public float RegainMana(float value) {

        _currentMana = Math.Min(_currentMana + value, _maxMana);
        if (OnManaChanged != null) {

            StatsChangedEventArgs args = new StatsChangedEventArgs();
            args._currentValue = _currentMana;
            args._maxValue = _maxMana;
            OnManaChanged(this, args);
        }
        return _currentMana;
    }
    public float ConsumeMana(float value) {

        _currentMana = Math.Max(_currentMana - value, 0f);
        if (OnManaChanged != null) {

            StatsChangedEventArgs args = new StatsChangedEventArgs();
            args._currentValue = _currentMana;
            args._maxValue = _maxMana;
            OnManaChanged(this, args);
        }
        return _currentMana;
    }
    public float AddMaxMana(int value) {

        _maxMana += value;
        _currentMana = Mathf.Min(_maxMana, _currentMana + value);
        if (OnHealthChanged != null) {

            StatsChangedEventArgs args = new StatsChangedEventArgs();
            args._currentValue = _currentMana;
            args._maxValue = _maxMana;
            OnManaChanged(this, args);
        }

        return _maxMana;
    }
    public float AddManaRegen(int value) {

        _currentManaRegen += value;
        if (OnManaRegenChanged != null) {

            StatsChangedEventArgs args = new StatsChangedEventArgs();
            args._currentValue = _currentManaRegen;
            OnHealthRegenChanged(this, args);
        }

        return _currentManaRegen;
    }
    public int GainXP(float value) {

        _currentXp += value;
        while (_currentXp >= _maxXp) {
            LevelUP();
        }
        if (OnXpChanged != null) {
            StatsChangedEventArgs args = new StatsChangedEventArgs();
            args._currentValue = _currentXp;
            args._maxValue = _maxXp;
            OnXpChanged(this, args);
        }
        return _currentLevel;
    }

    public float GainDamage(float value) {
        _currentDamage += value;
        if (OnDamageChanged != null) {

            StatsChangedEventArgs args = new StatsChangedEventArgs();
            args._currentValue = _currentDamage;
            OnDamageChanged(this, args);
        }
        return _currentDamage;
    }
    public float LoseDamage(float value) {
        _currentDamage = Mathf.Max(0f, _currentDamage - value);
        if (OnDamageChanged != null) {

            StatsChangedEventArgs args = new StatsChangedEventArgs();
            args._currentValue = _currentDamage;
            OnDamageChanged(this, args);
        }
        return _currentDamage;
    }
    public float CurrentDamage {
        get {
            return _currentDamage;
        }
        set {
            _currentDamage = Mathf.Max(0f, value);
            if (OnDamageChanged != null) {

                StatsChangedEventArgs args = new StatsChangedEventArgs();
                args._currentValue = _currentDamage;
                OnDamageChanged(this, args);
            }
        }
    }
}


public class StatsChangedEventArgs : EventArgs {

    public float _maxValue { get; set; } //valoarea (maxima, avem nevoie la calculul de procentaj) a statusului afectat
    public float _currentValue { get; set; } //valoarea (dupa modificare) a statusului afectat
}