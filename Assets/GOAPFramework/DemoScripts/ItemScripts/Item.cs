using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

    GameObject _target;
    [SerializeField]
    protected bool _isActive = true;

    public bool isActive
    {
        get
        {
            return _isActive;
        }
    }
    public void Use(GameObject target)
    {
        _target = target;
    }
    public bool OnUse(GameObject target)
    {
        return _target == target;
    }
    public void Detach()
    {
        _target = null;
    }
}
