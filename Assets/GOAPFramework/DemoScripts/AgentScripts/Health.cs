using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GOAP.Framework;
using UnityEditor.SceneManagement;
public class Health : MonoBehaviour {
    [SerializeField]
    private int health;
    [SerializeField]
    private int max_health;

    [SerializeField]
    private int cold;
    [SerializeField]
    private int maxCold;

    [SerializeField]
    private int hunger;
    [SerializeField]
    private int maxHunger;

    [SerializeField]
    WorldState worldState;
    // Use this for initialization
    float timer = 15.0f;
    float coldTimer = 5.0f;

    float _currentTime = 0.0f;
    float _coldTime = 0.0f;

    void Awake() {
        worldState.SetValue("Health", health);
        worldState.SetValue("Cold", cold);
        worldState.SetValue("Hunger", hunger);

    }
    private void Update()
    {
        _currentTime += Time.deltaTime;
        _coldTime += Time.deltaTime;
        if (_currentTime>timer)
        {
            IncreaseHunger();
            _currentTime = 0.0f;
        }
        if (_coldTime > coldTimer)
        {
            IncreaseCold();
            _coldTime = 0.0f;
        }

    }
    void IncreaseHunger()
    {
        hunger++;

        if (hunger>=maxHunger)
        {
            DecreaseHealth();
        }
        worldState.SetValue("Hunger", hunger);
    }
    void IncreaseCold()
    {
        if (worldState.GetTValue<bool>("isNight"))
        {
            cold += 1;
        }
        if (worldState.GetTValue<bool>("isRaining"))
        {
            cold += 2;
        }      
        if (cold >= maxCold)
        {
            cold = maxCold;
            DecreaseHealth();
        }
        worldState.SetValue("Cold", cold);
    }
    void DecreaseHealth()
    {
        health--;
        worldState.SetValue("Health", health);

        if (health < 0)
        {
            EditorSceneManager.LoadScene("LostScene");
           //EndGame
        }
       
    }
    public void RestoreHealth(int value)
    {
        health = Mathf.Min(health + value, max_health);

        worldState.SetValue("Health", health);

    }
    public void DecreaseHunger(int value)
    {
        hunger = Mathf.Max(hunger - value, 0);

        worldState.SetValue("Hunger", hunger);

    }
    public void DecreaseCold(int value)
    {
        cold = Mathf.Max(cold - value, 0);

        worldState.SetValue("Cold", cold);

    }
}
