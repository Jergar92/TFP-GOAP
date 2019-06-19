using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GOAP.Framework;

public class Inventory : MonoBehaviour {

    [SerializeField]
    private int seed;
    [SerializeField]
    private int _maxSeed;
    [SerializeField]
    private int wood;
    [SerializeField]
    private int _woodMax;
    [SerializeField]
    WorldState worldState;
    // Use this for initialization
    private bool _openInventory = false;

    public bool isOpen
    {
        get
        {
            return _openInventory;
        }
    }
    void Awake () {
        worldState.SetValue("Seeds", seed);
        worldState.SetValue("MaxSeeds", _maxSeed);
        worldState.SetValue("Wood", wood);
        worldState.SetValue("MaxWood", _woodMax);
    }
    public void OpenInventory(bool open)
    {
        _openInventory = open;
    }
    public void AddOneSeed()
    {
        seed++;
        worldState.SetValue("Seeds", seed);

    }
    public void RemoveOneSeed()
    {
        seed--;
        worldState.SetValue("Seeds", seed);

    }
    public void AddOneWood()
    {
        wood++;
        worldState.SetValue("Wood", wood);

    }
    public void RemoveOneWood()
    {
        wood--;
        worldState.SetValue("Wood", wood);

    }

}
