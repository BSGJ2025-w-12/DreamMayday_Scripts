using UnityEngine;
using System.Collections.Generic;

public class DreamStateManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static DreamStateManager Instance { get; private set; }

    // 夢ごとの状態を保持（必要に応じて型変更）
    public int Dream0State = 0;
    public int Dream1State = 0;
    public int Dream2State = 0;
    public int Dream3State = 0;
    public int Dream4State = 0;
    public int Dream5State = 0;


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
