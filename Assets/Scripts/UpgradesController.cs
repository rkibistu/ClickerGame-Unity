using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
public class UpgradesController : MonoBehaviour
{
    [SerializeField]
    private Button[] _buttons;
    private TextMeshProUGUI[] _texts;
    private GameObject _buttonsContainer;

    private List<Action<int>> _upgrades;


    //cand tre sa alegem un upgrade, playerul e in starea IDLE. Si exista 3 cazuri:
    // 1). alege un skill de stats. nu mai tre sa faca nimic -> start run
    // 2). alege un skill, este loc in bara de skiluri, nu mai tre sa faca nimic -> start run
    // 3) alege un skilll, nu e loc in bara, tre sa aleaga un skill -> ramane in idle
    private bool _startRunAfterSelection = true;

    private UpgradesController() { }
    private static UpgradesController _instance = null;
    public static UpgradesController Instance {
        get {
            if (_instance == null)
                _instance = new UpgradesController();
            return _instance;
        }
    }
    void Start()
    {
        _instance = this;
        _buttonsContainer = _buttons[0].transform.parent.gameObject;

        PopulateUpgradesList();
        GetTextsFromButtons();
    }
    private void PopulateUpgradesList() {

        _upgrades = new List<Action<int>>();
        _upgrades.Add(GenerateAddUppercutSkill);
        _upgrades.Add(GenerateAddHealth);
        _upgrades.Add(GenerateAddMana);
        _upgrades.Add(GenerateAddHealSkill);
    }
    private void GetTextsFromButtons() {
        _texts = new TextMeshProUGUI[_buttons.Length];
        for (int i = 0; i < _buttons.Length; i++) {
            _texts[i] = _buttons[i].GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    public void SetActive(bool active) {

        _buttonsContainer.SetActive(active);
        for (int i = 0; i < _buttons.Length; i++)
            _buttons[i].onClick.RemoveAllListeners();
        if (active = true)
            GenerateUpgrades();
    }

    private void GenerateUpgrades() {

        int choice;
        List<int> choices = new List<int>(_buttons.Length);
        for(int i = 0; i < _buttons.Length; i++) {

            do {
                //sa nu se repete acelasi upgrade
                choice = UnityEngine.Random.Range(0, _upgrades.Count);
            } while (choices.Contains(choice));
            choices.Add(choice);
            _upgrades[choice](i);
        }
    }
    private void GenerateAddUppercutSkill(int buttonNo) {
        //asta se apeleaza random si populeaza butoane (la fel toate ce incep cu Generate
        _texts[buttonNo].text = "Add uppercut";
        _buttons[buttonNo].onClick.AddListener(AddUppercutSkill);
    }
    private void AddUppercutSkill() {
        //asta se intampla la apasarea de buton. Fiecare fucntie de generate are asociata o functie ca asta (de tip actiune)
        _startRunAfterSelection = SkillController.Instance.AddSkill(SkillController.Skills.UpperCut);
        SetActive(false);
        if(_startRunAfterSelection)
            PlayerController.Instance.StartRun();
    }

    private void GenerateAddHealth(int buttonNo) {

        _texts[buttonNo].text = "Add health";
        _buttons[buttonNo].onClick.AddListener(AddHealth);
    }
    private void AddHealth() {

        Debug.Log("add health");
        PlayerStats.Instance.AddMaxHealth(20);
        SetActive(false);
        PlayerController.Instance.StartRun();
    }

    private void GenerateAddMana(int buttonNo) {

        _texts[buttonNo].text = "Add mana";
        _buttons[buttonNo].onClick.AddListener(AddMana);
    }
    private void AddMana() {
        PlayerStats.Instance.AddMaxMana(10);
        SetActive(false);
        PlayerController.Instance.StartRun();
    }

    private void GenerateAddHealSkill(int buttonNo) {
        _texts[buttonNo].text = "Add HEAL skill";
        _buttons[buttonNo].onClick.AddListener(AddHealSkill);
    }
    private void AddHealSkill() {
        SkillController.Instance.AddSkill(SkillController.Skills.Heal);
        SetActive(false);
        PlayerController.Instance.StartRun();
    }
}
