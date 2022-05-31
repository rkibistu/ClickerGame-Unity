using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


class UpperCut : MonoBehaviour {

    [SerializeField]
    private float _baseForce;
    private float _currentForce;
    [SerializeField]
    private float _multiplier = 2f;
    [SerializeField]
    private int _maxConsecutiveHits;
    private int _consecutiveHitsCount = 0;
    [SerializeField]
    public float _baseCooldown;


    [SerializeField]
    private Transform _perfectHitMaxHeight;
    [SerializeField]
    private Transform _goodHitMaxHeight;

    //temp
    // public SpriteRenderer _cdIcon;

    private UpperCut() { }
    private static UpperCut _instance = null;
    public static UpperCut Instance {
        get {
            if (_instance == null)
                _instance = new UpperCut();
            return _instance;
        }
    }

    private void Start() {

        _instance = this;

        _currentForce = _baseForce;
    }

    private void Update() {

    }
    public bool UseUpperCut(EnemyController target) {
        // Debug.Log("b: " + _baseForce);
        //uppercut the current enemy -> throw it in air
        //limita de folosire consecutiva
        //cu fiecare folosire conseutiva de succes -> arunca si mai sus

        //va returna true -> daca trebuie aplciat cooldown
        //           false -> daca nu trebuie 


        if (target.IsGoingUp())
            return false;


        //calculam noua forta
        float targetPosY = target.transform.position.y - target.GetComponent<SpriteRenderer>().size.y / 2;
        if (targetPosY < _perfectHitMaxHeight.position.y) {
            //Super
            _currentForce = _currentForce + _multiplier;
        }
        else if (targetPosY < _goodHitMaxHeight.position.y) {
            //OK
            _currentForce = _currentForce + _multiplier / 2;
        }
        else {
            //nah
            _currentForce = 0;
        }

        if (_currentForce == 0) {

            target.ThrowInTheAir(_currentForce, false);
            ResetForce();
            return true;
        }
        else {

            target.ThrowInTheAir(_currentForce);
        }

        target.Damaged(PlayerStats.Instance.CurrentDamage);

        _consecutiveHitsCount++;
        if (_consecutiveHitsCount >= _maxConsecutiveHits) {
            ResetForce();
            return true;
        }
        return false;
    }

    public void ResetForce() {

        _currentForce = _baseForce;
        _consecutiveHitsCount = 0;
    }
    public void FallOnTheGround() {

        ResetForce();
        SkillController.Instance.SetUppercutCooldown();
    }



}

