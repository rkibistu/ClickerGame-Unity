using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour {
    

    private Animator _animator;
    private Rigidbody2D _rigidBody;

    private Image _healthBar;

    private bool _inAir = false;

    //private float _rotationSpeed = 0.5f;
    private struct Stats {

        public Stats(float multiplier) {

            maxHealth = 100 * multiplier;
            currentHealth = maxHealth;
            damage = 10 * multiplier;
        }

        public float maxHealth;
        public float currentHealth;
        public float damage;
    }
    private Stats _stats;
    private float _multiplier;

    

    void Start() {
        _animator = GetComponent<Animator>();
        _rigidBody = GetComponent<Rigidbody2D>();

        _healthBar = GameObject.FindGameObjectWithTag("EnemyHealthBar").GetComponent<Image>();
        _healthBar.fillAmount = 1;
    }

    private void Update() {

        if (_inAir) {

           // _rigidBody.MoveRotation(_rotationSpeed++);
        }
    }

    public float Multiplier {
        get { return _multiplier; }
        set {
            _multiplier = value;
            _stats = new Stats(_multiplier);
        }
    }

    private void DoDamageToPlayer() {

        PlayerStats.Instance.Damaged(1);
    }
    public void StartAttack() {

        _animator.Play("Attack");
    }
    public float Damaged(float value) {

        _stats.currentHealth -= value;
        if(_stats.currentHealth <= 0) {
            SecondCameraController.Instance.ResetTarget();
            UpperCut.Instance.ResetForce();
            Destroy(gameObject);
            return -1;
        }

        _healthBar.fillAmount = _stats.currentHealth / _stats.maxHealth;
        return _stats.currentHealth;
    }
    public bool ThrowInTheAir(float throwForce, bool moveCamera = true) {

        if(moveCamera)
            SecondCameraController.Instance.SetTarget(transform, GetComponent<SpriteRenderer>().bounds.size.y);

        _rigidBody.velocity = new Vector2(0f, 0f);
        _rigidBody.AddForce(Vector2.up * throwForce, ForceMode2D.Impulse);

        _inAir = true;

        return true;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        
        if(collision.collider.gameObject.tag == "Ground") {

            if (_inAir) {

                UpperCut.Instance.FallOnTheGround();
                SecondCameraController.Instance.ResetTarget();
            }
            _inAir = false;
        }
    }
    public bool IsGoingUp() {

        return _rigidBody.velocity.y > 0;
    }
    public bool IsAlive() {
        return _stats.currentHealth > 0;
    }
}
