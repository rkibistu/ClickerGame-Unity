using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//Me  not proud about this code
//asa ca adaugam multe comments sa intelegem ce am vrut cand l am facut

//A doua camera trebuie sa imparta ecranul in jumatate
//jumatatea din stanga va ramane pe loc, sa se vada Playerul
//juamtatea din dreapta va urmarii tinta in sus si in jos
public class SecondCameraController : MonoBehaviour
{
    private Transform _currentTarget; // tinta pe ce focuseaza camera
    private float _currentTargetHeight; //marimea sprit-ului acesteia.( transform.pos.y - spriteHeight = partea de jos a tintei; altel obtinem mijlocul)
    private Camera _secondCamera; // camera efectiva

    [SerializeField]
    private Transform _mainCamera; //camera principala peste care se suprapune asta noua
    private Vector3 _distanceFromMainCam = new Vector3(4.755f, 0f, 0f); //distanta ca sa se suprapuna perfect pe imagine

    [SerializeField]
    private Image _middleBar; //bara despartitoare la mijloc
    [SerializeField]
    private GameObject _highBar; //bara ce arata cat de sus a ajuns tinta
    [SerializeField]
    private Transform _highBarMeter; //o linie pe _highBar ce arata unde se afla acum (se msica odata cu tinta)

    [SerializeField]
    private float _baseLerpSpeed = 0.1f; //viteza cu care camera urmareste tinta (va creste reptat pentru efect de smooth)
    private float _currentLerpSpeed;
    [SerializeField]
    private float _lerpMultiplier = 0.0075f; //viteza cu care creste reptat _baseLepSpeed;
    [SerializeField]
    private float _startFollowPosY = 3.3f; //inaltimea de la care camera incepe sa urmareasca tinta
    
    private bool _maxHeightReached = false; 
    private float _maxHeight = 0f;
    private float _oldPosY = 0f; // pozitia din frameul anterior al tintei ep axa Y
    private SecondCameraController() {; } //SINGELTON
    private static SecondCameraController _instance = null;
    public static SecondCameraController Instance {
        get {
            if (_instance == null)
                _instance = new SecondCameraController();
            return _instance;
        }
    }
    void Awake()
    {
        _instance = this;
        _currentTarget = null;
        _currentLerpSpeed = _baseLerpSpeed;

        _secondCamera = GetComponent<Camera>();
    }


    void Update()
    {
        if(_currentTarget != null) {

            Followtarget();
            _highBarMeter.position = new Vector2(_highBarMeter.position.x, _currentTarget.transform.position.y - _currentTargetHeight/2); //bara ce arata inaltimea la care este tinta
        }
    }

    private void Followtarget() {
        //urmareste tinta intr un mod mai smooth

        //vrem sa miscam camera doar daca tinta e mai sus de _startFollowPosY in cazul in care urcam
        //sau mereu in cazul in care cadem -> daca nu, ar sarii bursc de la _startToFollowPosY la 0 si ar distruge efectul smooth
        if(_currentTarget.transform.position.y >= _startFollowPosY || _maxHeightReached) {
            _secondCamera.enabled = true; 
            _middleBar.enabled = true;

            transform.position = _mainCamera.position + _distanceFromMainCam; //deplasam camera la dreapta fata de camera principala sa se alineize perfect
            

            float targetPosY = _currentTarget.transform.position.y;
            if (_maxHeightReached) {

                
                transform.position += Vector3.up * _maxHeight; //cand facem lerp la coborare, vrem sa plece de la punctul cel mai intalt si sa se tot apropie de tinta
                targetPosY = Mathf.Lerp(targetPosY, targetPosY - _currentTargetHeight * 2 / 3, _currentLerpSpeed); // vrem sa arate putin mai jos decat tinta, sa se vada pamantul mai din timp                                                                                           
            }
            float posY = Mathf.Lerp(transform.position.y, targetPosY, _currentLerpSpeed); //urmareste tinta
            _currentLerpSpeed = Mathf.Lerp(_currentLerpSpeed, 1, _lerpMultiplier); //scade distanta dintre camera si tinta
            if (posY < 0)
                posY = 0; 
            if (_oldPosY > posY && !_maxHeightReached) {
                _maxHeightReached = true;
                _maxHeight = _oldPosY;
                _currentLerpSpeed = _baseLerpSpeed; //revenit la lerp initial, sa creem efect frumos de cadera (sa ramana putin in ruma camera initial)
            }
            
            transform.position = new Vector3(transform.position.x, posY, transform.position.z); //miscam camera efectiv, pana acum doar ne am jucat cu pozitia ce o vrem
            _oldPosY = posY; //pozitia dinainte de modificare ca sa ptuem verifica daca am ajuns la inaltime max
        }
        
    }

    public void SetTarget(Transform targetTransform, float targetHeight) {


        _currentTarget = targetTransform;
        _currentTargetHeight = targetHeight; //inaltimea sprite-ului tintei

        //bara ce arata inaltimea la care a fost aruncat playerul
        _highBar.SetActive(true);
        _highBar.transform.position = new Vector2(_mainCamera.position.x + 10, _highBar.transform.position.y);

        //resetam valorile pentru viteza de urmarire a camerei si inaltime maxima
        _currentLerpSpeed = _baseLerpSpeed;
        _maxHeightReached = false;
        _oldPosY = 0;
    }
    public void ResetTarget() {

        //daca inamicul moare sau cade pe pamant vrem sa resetam datele si sa nu mai aratam pe ecran UI ce tine de intaltime
        _secondCamera.enabled = false;
        _middleBar.enabled = false;
        _currentTarget = null;
        _maxHeightReached = false;
        _currentLerpSpeed = _baseLerpSpeed;
        _oldPosY = 0;
        _highBar.SetActive(false);
    }
}
