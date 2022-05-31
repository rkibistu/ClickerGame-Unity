using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    
    [SerializeField]
    private float _speed;
    [SerializeField]
    private float _distanceToStop = 10;

    private enum State { Idle = 0, Run, Attack, Dead};
    private State _currentState;
    private Animator _animator;

    [HideInInspector]
    public EnemyController _currentEnemy;

    private PlayerController() { }
    private static PlayerController _instance = null;
    public static PlayerController Instance {
        get {
            if (_instance == null)
                _instance = new PlayerController();
            return _instance;
        }
    }

    private void Awake() {
        _instance = this;
    }

    void Start()
    {
        _animator = GetComponent<Animator>();
        
        _currentState = State.Run;
        _animator.Play("Run");
    } 
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) {

            PlayerStats.Instance.Damaged(5);
            PlayerStats.Instance.ConsumeMana(17);
            PlayerStats.Instance.GainXP(150);
        }
    }

    private void FixedUpdate() {

        if (_currentState == State.Run) {

            gameObject.transform.position = new Vector2(transform.position.x + _speed * 0.01f, transform.position.y);
            CheckForEnemy();
        }
    }

    private void CheckForEnemy() {
        //raycast; Daca intalnesti inamic, treci in starea de atac
        // -> auto attack la fiecare animatie si la fiecare click

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, _distanceToStop);
        if(hit.collider != null) {

            _currentEnemy = hit.collider.GetComponent<EnemyController>();

            _currentEnemy.StartAttack();
            _currentState = State.Attack;
            _animator.Play("Attack");
        }
    }

    void OnFire() {
        //la fiecare apasare de click incepe iar aniamtia de atack si aplica damage la inamicu curent

        if(_currentState != State.Attack) {
            return;
        }

        _animator.Play("Attack");
        AttackCurrentEnemy();
    }
    private void OnSkill1() {
        
        SkillController.Instance.DoSkill(0);
    }
    private void OnSkill2() {
        SkillController.Instance.DoSkill(1);
    }
    private void OnSkill3() {
        SkillController.Instance.DoSkill(2);
    }
    private void AttackCurrentEnemy() {
        //funcia asta e apelata automat de fiecare data cand se termina aniamtia de Attack (sa exista si un atac automat, nu doar la click)
        //si e apelata si la fiecare click
        //daca inamicul maore, se revine la starea de Run

        float healthLeft = _currentEnemy.Damaged(PlayerStats.Instance.CurrentDamage);
        
        if (healthLeft <= 0) {
            EnemyKilled();
        }
    }

    private void EnemyKilled() {
        //toate setarile de schimbat dupa ce omori un inamic

        _currentState = State.Run;
        _animator.Play("Run");

        PlayerStats.Instance.GainXP(100);//aici va trebui ceva formula

        EnemySpawner.Instance.SpawnEnemy(transform.position, true);
    }
    private void OnDrawGizmos() {

        Gizmos.DrawLine(transform.position, (Vector2)transform.position + Vector2.right * _distanceToStop);
    }

    public bool DoUpperCut() {

        if (_currentState != State.Attack) {
            return false;
        }
        _animator.Play("Attack");

        bool cooldown = UpperCut.Instance.UseUpperCut(_currentEnemy);
        
        if (_currentEnemy.IsAlive() == false)
            EnemyKilled();

        return cooldown;
    }
    public void Die() {

        _currentState = State.Dead;
    }
    public void StartIdle() {

        _currentState = State.Idle;
        _animator.Play("Idle");
    }
    public void StartRun() {
        _currentState = State.Run;
        _animator.Play("Run");
    }

}
