using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Counter : MonoBehaviour
{
    [SerializeField]
    private Movement indicatorScript;

    public int playerScore;
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void FixedUpdate()
    {
        playerScore = indicatorScript.pickedUpObjects;
    }
}
