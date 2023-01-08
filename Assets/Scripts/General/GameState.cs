using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameState", menuName = "General/GameState")]
public class GameState : ScriptableObject
{

    public GameObject silo;
    
    public int numberOfEnemiesAlive = 0;
    public bool waveIsActive = false;



}