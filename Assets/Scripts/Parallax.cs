using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Tilemaps;

public class Parallax : MonoBehaviour
{
    [SerializeField]
    private GameObject _camera;
    [SerializeField]
    private float _parallaxEffect = 0;

    private float _length, _startpos;
    void Start()
    {
        _startpos = transform.position.x;

        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        _length = (sprite != null) ? sprite.bounds.size.x : GetComponent<TilemapRenderer>().bounds.size.x;

        //Debug.Log(this.name + "  Length: " + _length);
    }

    
    void Update()
    {
        float temp = _camera.transform.position.x * (1 - _parallaxEffect);
        float dist = _camera.transform.position.x * _parallaxEffect;

        transform.position = new Vector3(_startpos + dist, transform.position.y, transform.position.z);

        if (temp > _startpos + _length)
            _startpos += _length;
        if (temp < _startpos - _length)
            _startpos -= _length;
    }
}
