using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chimney : Item {

    private float currentTime = 0.0f;
    public float addTime = 5.0f;
    public void AddWood()
    {
        _isActive = true;
        currentTime += addTime;
    }
    private void Update()
    {
        if (isActive)
        {
            currentTime -= Time.deltaTime;
        }
        if(currentTime < 0.0f)
        {
            _isActive = false;
            currentTime = 0.0f;
        }
        
    }
   

}
