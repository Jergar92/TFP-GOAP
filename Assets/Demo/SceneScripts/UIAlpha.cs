using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GOAP.Framework;

public class UIAlpha : MonoBehaviour {

    [SerializeField]
    private Image _checkBoxRain;
    [SerializeField]
    private Image _checkBoxNight;
    [SerializeField]
    private Text _hpText;
    [SerializeField]
    private Text _hungerText;
    [SerializeField]
    private Text _seedsText;
    [SerializeField]
    private Text _woodText;
    [SerializeField]
    private Text _coldText;
    [SerializeField]
    private Text _goalText;
    [SerializeField]
    private Text _actionText;

    [SerializeField]
    private WorldState _worldState;
    [SerializeField]
    private GoapOwner _goapOwner;
    // Use this for initialization
    void Start ()
    {
        _checkBoxRain.enabled = _worldState.GetTValue<bool>("isRaining");
        _checkBoxNight.enabled = _worldState.GetTValue<bool>("isNight");
        _coldText.text = _worldState.GetTValue<int>("Cold").ToString() + "/" + _worldState.GetTValue<int>("MaxCold").ToString();
        _hungerText.text = _worldState.GetTValue<int>("Hunger").ToString() + "/" + _worldState.GetTValue<int>("MaxHunger").ToString();
        _hpText.text = _worldState.GetTValue<int>("Health").ToString() + "/" + _worldState.GetTValue<int>("MaxHealth").ToString();
        _woodText.text = _worldState.GetTValue<int>("Wood").ToString() + "/" + _worldState.GetTValue<int>("MaxWood").ToString();
        _seedsText.text = _worldState.GetTValue<int>("Seeds").ToString() + "/" + _worldState.GetTValue<int>("MaxSeeds").ToString();

    }

    // Update is called once per frame
    void Update ()
    {
        _checkBoxRain.enabled = _worldState.GetTValue<bool>("isRaining");
        _checkBoxNight.enabled = _worldState.GetTValue<bool>("isNight");
        _coldText.text = _worldState.GetTValue<int>("Cold").ToString() + "/" + _worldState.GetTValue<int>("MaxCold").ToString();
        _hungerText.text = _worldState.GetTValue<int>("Hunger").ToString() + "/" + _worldState.GetTValue<int>("MaxHunger").ToString();
        _hpText.text = _worldState.GetTValue<int>("Health").ToString() + "/" + _worldState.GetTValue<int>("MaxHealth").ToString();
        _woodText.text = _worldState.GetTValue<int>("Wood").ToString() + "/" + _worldState.GetTValue<int>("MaxWood").ToString();
        _seedsText.text = _worldState.GetTValue<int>("Seeds").ToString() + "/" + _worldState.GetTValue<int>("MaxSeeds").ToString();

        _goalText.text = _goapOwner.goapGraph.priorityGoalName;
        _actionText.text = _goapOwner.goapGraph.currentActionName;

    }
}
