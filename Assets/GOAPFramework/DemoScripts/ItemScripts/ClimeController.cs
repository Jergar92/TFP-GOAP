using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GOAP.Framework;
public class ClimeController : MonoBehaviour {

    [SerializeField]
    private WorldState _worldState;

    [SerializeField]
    private float rainMinTime;
    [SerializeField]
    private float rainMaxTime;
    [SerializeField]
    private float rainMinSpawnTime;
    [SerializeField]
    private float rainMaxSpawnTime;
    [SerializeField]
    private float nightTime;
    [SerializeField]
    private float nightSpawnTime;
    // Use this for initialization
    void Start () {
        Invoke("NightStart", nightSpawnTime);
        Invoke("RainStart", Random.Range(rainMinSpawnTime, rainMaxSpawnTime));

    }

    void NightStart()
    {
        _worldState.SetValue("isNight", true);
        Invoke("NightStop", nightTime);
    }
    void NightStop()
    {
        _worldState.SetValue("isNight", false);
        Invoke("NightStart", nightSpawnTime);
    }
    void RainStart()
    {
        _worldState.SetValue("isRaining", true);
        Invoke("RainStop", Random.Range(rainMinTime, rainMaxTime));
    }
    void RainStop()
    {
        _worldState.SetValue("isRaining", false);
        Invoke("RainStart", Random.Range(rainMinSpawnTime, rainMaxSpawnTime));
    }
}
