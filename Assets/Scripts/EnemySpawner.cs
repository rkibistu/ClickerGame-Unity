using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {
    [SerializeField]
    private List<GameObject> _enemiesPrefab;
    [SerializeField]
    private float distanceFromPlayer = 25f;

    private float _multiplier = 1;

    private EnemySpawner() {; }
    private static EnemySpawner _instance = null;
    public static EnemySpawner Instance {
        get {
            if (_instance == null)
                _instance = new EnemySpawner();
            return _instance;
        }
    }
    void Start() {

        _instance = this;
        SpawnEnemy(transform.position);
    }

    public void SpawnEnemy(Vector2 position, bool addOffset = false) {
        //spawneaza inamic la positia data de position
        //daca addOffset este TRUE -> adauga si distanta tinunta in distanceFromPlayer intern

        int index = Random.Range(0, _enemiesPrefab.Count - 1); //alege random mosntru
        if (addOffset)
            position += Vector2.right * distanceFromPlayer;

        GameObject enemy = Instantiate(_enemiesPrefab[index], position, Quaternion.identity);
        enemy.GetComponent<EnemyController>().Multiplier = _multiplier; //multiplayer pentru statsurile inamicului -> sa devina tot mai puternic

        _multiplier += 0.1f; 
    }

}
