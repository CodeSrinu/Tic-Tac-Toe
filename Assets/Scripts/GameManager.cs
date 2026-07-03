using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public event EventHandler<OnClickedOnGridPosEventArgs> OnClickedOnGridPos;
    public class OnClickedOnGridPosEventArgs{
        public int x;
        public int y;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void CheckGridposition(int x, int y)
    {
        OnClickedOnGridPos?.Invoke(this, new OnClickedOnGridPosEventArgs
        {
            x = x,
            y = y
        }); 
    }
}
