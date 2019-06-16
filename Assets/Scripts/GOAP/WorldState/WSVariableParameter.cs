using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GOAP.Framework;
public class WSVariableParameter :MonoBehaviour{
 
    public bool Test()
    {
        return true;
    }
    public bool CloseToTarget(Transform target)
    {
        float distance = 1.0f;
        if ((target.position-transform.position).magnitude < distance)
        {
            return true;
        }
        return false;
    }
    public bool UseItem(Transform target)
    {
        Item item = target.GetComponent<Item>();
        if(item!=null)
        {
            if (item.OnUse(gameObject))
                return true;
        }       
        return false;
    }
    public bool isItemActive(Transform target)
    {
        Item item = target.GetComponent<Item>();
        if (item != null)
        {
            return item.isActive;
        }
        return false;
    }
    public bool UseInventoryItem(string item)
    {
        if (GetComponent<Inventory>().isOpen)
        {
            if (item == "fruit")
            {
                return true;
            }
        }
        return false;
    }
}
