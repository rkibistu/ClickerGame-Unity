using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SkillController : MonoBehaviour {
    
    [Serializable]
    struct SkillDetails {

        [HideInInspector]
        public Skills _currentSkill; //skillul curent
        [SerializeField]
        public Image _currentSprite; //sprite-ul lui
        [HideInInspector]
        public Button _currentButton; //butonul aferent (folosit cand trebuie interschimbate)
        [SerializeField]
        public Image _currentTimersImage; //iamgine neagra, transparenta folosita pentru a reprezenta cooldown-ul ramas
        [HideInInspector]
        public float _baseCooldown; //cooldownul debaza
        [HideInInspector]
        public float _timerCooldown; //timer ce scade pornind de la cooldown ul de baza
    }

    [SerializeField]
    private SkillDetails[] _currentSkills; //vector cu detalii despre skillurile curente
    public enum Skills { NoSkill = 0, UpperCut, Heal } //toate skillurile posibile

    [Header("Skills Sprites")]
    [SerializeField]
    private Sprite _noSkillSprite; //sprite-urile pt fiecare skill
    [SerializeField]
    private Sprite _upperCutSprite;
    [SerializeField]
    private Sprite _healSprite;


    private int _skillSlotsNo; //nr de skilluri, se populea automat in fucntie de cate el sunt in _curentSkills in inspector
    private Dictionary<Skills, Sprite> _spritesDictionary; //dictionar ce leaga fiecare skill de sprite-ul lui bazat pe Skills
    private Dictionary<Skills, Func<float>> _actionsDictionary;//dictionar ce lea fiecare skill de o functie

    private Skills _skillToReplaceWith = Skills.NoSkill; //acest e populat doar cand e nevoie sa se itnerschimba un skill

    private SkillController() { } //partea de singleton
    private static SkillController _instance = null;
    public static SkillController Instance {
        get {
            if (_instance == null)
                _instance = new SkillController();
            return _instance;
        }
    }
    private void Awake() {
        _instance = this;
    }
    void Start() {

        CreateDictionaries();
        InitializeSkills();

        StartCoroutine(UpdateCooldowns());
    }

    void Update() {

    }

    private void CreateDictionaries() {
        _spritesDictionary = new Dictionary<Skills, Sprite>();
        _spritesDictionary.Add(Skills.NoSkill, _noSkillSprite);
        _spritesDictionary.Add(Skills.UpperCut, _upperCutSprite);
        _spritesDictionary.Add(Skills.Heal, _healSprite);

        _actionsDictionary = new Dictionary<Skills, Func<float>>();
        _actionsDictionary.Add(Skills.NoSkill, DoNoSkill);
        _actionsDictionary.Add(Skills.UpperCut, DoUpperCut);
        _actionsDictionary.Add(Skills.Heal, DoHeal);
    }
    private void InitializeSkills() {

        _skillSlotsNo = _currentSkills.Length;

        for(int i = 0; i < _skillSlotsNo; i++) {

            SetSkill(i, Skills.NoSkill);
            _currentSkills[i]._timerCooldown = 0;
            _currentSkills[i]._baseCooldown = 0;
            _currentSkills[i]._currentButton = _currentSkills[i]._currentSprite.gameObject.GetComponent<Button>();
            int locali = i; //avem envoie de asta ca altfel se va apela ChangeSkill mereu cu utlima iteratie din for
            _currentSkills[i]._currentButton.onClick.AddListener(delegate {
                                                                            ChangeSkill(locali);
                                                                          });
            _currentSkills[i]._currentButton.enabled = false;

        }
    }

    private IEnumerator UpdateCooldowns() {
        //pentru fiecare skill, verifica daca e pe cooldown, ii scade tiemr-ul si actualizeazza iamginea ce arata cooldownul
        while (true) {
            for (int i = 0; i < _skillSlotsNo; i++) {
                if (_currentSkills[i]._timerCooldown > 0) {
                    _currentSkills[i]._timerCooldown -= 0.1f - Time.deltaTime;
                    _currentSkills[i]._currentTimersImage.fillAmount = _currentSkills[i]._timerCooldown / _currentSkills[i]._baseCooldown;
                }
                else {
                    _currentSkills[i]._currentTimersImage.fillAmount = 0;
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
    private void SetSkill(int skillNo, Skills skill) {
        //pune pe pozitia skillNo din bara de skilluri, skillul skill
        _currentSkills[skillNo]._currentSprite.sprite = _spritesDictionary[skill];
        _currentSkills[skillNo]._currentSkill = skill;
    }
    public bool AddSkill(Skills skill) {
        //adauga uin skill
        //default -> adauga pe prima pozitie unde are loc pe abra de skilluri
        //daca nu are loc, se astepta apasarea unuia dintre skillurile din bara si il va inlocui pe acela

        //functia returneaza -> true, daca a avut loc skillul si a si fost pus in bara de skilluri
        //                   -> false, daca nu a avut loc. Caz in care astepta sa se apese pe unul din skillurile din bara
        bool replace = true;
        for(int i = 0; i < _skillSlotsNo; i++) {
            if (_currentSkills[i]._currentSkill == Skills.NoSkill) {

                SetSkill(i, skill);
                replace = false;
                break;
            }
        }
        if (replace) {
            _skillToReplaceWith = skill;
            ActivateButtons();
            return false;
        }
        return true;
    }
    private void ActivateButtons() {
        //skillurile din bara au butoane aferente
        //nu el tinem active mereu ca sa nu si schimbe culoarea cand se trece mousel peste ele
        //le activam doar la nevoie
        for(int i = 0; i < _skillSlotsNo; i++) {
            _currentSkills[i]._currentButton.enabled = true;
        }

    }
    private void ChangeSkill(int skillNo) {
        //schimba skillul de pe slotul cu numarul skillNo din bara
        //va fi inlocuit cu skillul stocat in _skillToReplaceWith
        //variabile populata dde functia ce incearca sa adauge skillul si constata ca nu are loc
        if (_skillToReplaceWith != Skills.NoSkill) {
            Debug.Log("Am inlocuit skillul " + skillNo);
            SetSkill(skillNo, _skillToReplaceWith);
            _skillToReplaceWith = Skills.NoSkill;

            //mereu cand va ajunge aiic, jocul e "oprit". Playerul tre sa aleaga ce skill sa inlocuiasca
            //deci sigur suntem in idle. Si dupa ce alege, vrem sa inceapa sa fuga
            PlayerController.Instance.StartRun();
        }
        else
            Debug.Log("Butonu asta nu ar trebui sa fie valabil daca _skillToReplaceWith nu e setat");
    }

    public void DoSkill(int skillNo) {

        //daca inca e pe cooldown, iesi
        if (_currentSkills[skillNo]._timerCooldown > 0)
            return;

        //atlfel, executa skillul si actualizeaza cooldawn
        float cooldown = _actionsDictionary[_currentSkills[skillNo]._currentSkill]();
        _currentSkills[skillNo]._baseCooldown = cooldown;
        _currentSkills[skillNo]._timerCooldown = cooldown;
        if(cooldown != 0)
            _currentSkills[skillNo]._currentTimersImage.fillAmount = 1;
    }

    private float DoNoSkill() {
        //se apasa buton aferent unui skiil inexistent in abra de skilluri
        return 0;
    }
    private float DoUpperCut() {

        bool cooldown = PlayerController.Instance.DoUpperCut();
        return (cooldown == true) ? UpperCut.Instance._baseCooldown : 0;
    }
    private float DoHeal() {

        PlayerStats.Instance.Heal(100);
        return 10;
    }

    public void SetUppercutCooldown() {
        //Cooldow-ul de la skillul asta e putin mai diferit
        //daca playerul da lovituri cu succes -> nu se puen cooldown la folosire
        //se puen cooldown doar daca playerul rateaza o lovitura sau daca ianmic atinge pamantul  dupa ce a fost arucnat in aer
        //deci avem nevoie de fucntia asta ca sa putem seta cooldownul la coliziunea dintre inamic si pamant (se intampla asicron cu functionarea acestei clase, deci nu stim cand)
        for(int i = 0; i < _skillSlotsNo; i++) {
            if(_currentSkills[i]._currentSkill == Skills.UpperCut) {
                if (_currentSkills[i]._timerCooldown > 0)
                    return; //daca e deja pe cd -> nu l reseta
                _currentSkills[i]._baseCooldown = UpperCut.Instance._baseCooldown;
                _currentSkills[i]._timerCooldown = _currentSkills[i]._baseCooldown;
            }
        }
    }
}
