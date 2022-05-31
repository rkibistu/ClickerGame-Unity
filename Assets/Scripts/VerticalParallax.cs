using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalParallax : MonoBehaviour
{
    [SerializeField]
    private GameObject _camera;
    [SerializeField]
    private float parallaxEffect = 0;

    private float _length, _startpos;
    private float _minimPos;
    void Start()
    {
        _startpos = transform.position.y;
        _minimPos = transform.position.y;

        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        _length =  sprite.bounds.size.y;
    }

    
    void Update()
    {
        float temp = _camera.transform.position.y * (1 - parallaxEffect);
        float dist = _camera.transform.position.y * parallaxEffect;

        transform.position = new Vector3(transform.position.x, _startpos + dist, transform.position.z);

        if (temp > _startpos + _length)
            _startpos += _length;
        if (temp < _startpos - _length)
            _startpos -= _length;

        if (_startpos < _minimPos)
            _startpos = _minimPos;
    }
}
