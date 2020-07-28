using UnityEngine;
using System;
using System.Collections;
using System.Threading;

public class GameContext
{
    public static GameContext GetInstance
    {
        get
        { return Singleton<GameContext>.GetInstance; }
    }

    public Stage stage
    {
        get { return _stage; }
        set { lock (lockThis) { _stage = value; } }
    }

    public float GameSpeed
    {
        get { return _gameSpeed; }
        set
        {
            //TowerPool towerPool = TowerPool.GetInstance;
            //UnitPool unitPool = UnitPool.GetInstance;
            lock (lockThis)
            {
                if (value > 0)
                    Time.timeScale = _gameSpeed = value;
                else
                    Time.timeScale = _gameSpeed = 0;
                //towerPool.SetGameSpeed(_gameSpeed);
                //unitPool.SetGameSpeed(_gameSpeed);
            }
        }
    }

    public int Money
    {
        get { return _money; }
    }

    public float StageTime
    {
        get { return _stageTime; }
    }

    public int IncreaseMoney(int amount)
    {
        lock (lockThis)
        {
            _money += amount;
            return _money;
        }
    }

    public int DecreaseMoney(int amount)
    {
        lock (lockThis)
        {
            _money -= amount;
            return _money;
        }
    }

    public void ResetStageTime()
    {
        _stageTime = 0.0f;
    }

    public bool ShowBuildMenu
    {
        get { return _showBuildMenu; }
        set
        {
            lock (lockThis)
            {
                _showBuildMenu = value;
            }
        }
    }

    public bool ShowDebugInfo
    {
        get { return _showDebugInfo; }
        set
        {
            lock (lockThis)
            {
                _showDebugInfo = value;
            }
        }
    }

    GameContext()
    {
        //GameObject worker = new GameObject("GameContextWorker");
        //worker.AddComponent<GameContextWorker>();
    }

    ~GameContext()
    {
    }


    public void Update(float delta)
    {
        if (stage)
        {
            stage.Update(delta);
            _stageTime += delta;
        }
    }


    object lockThis = new object();

    Stage _stage = null;

    float _gameSpeed = 1;

    int _money = 0;

    bool _showBuildMenu = false;

    bool _showDebugInfo = true;

    float _stageTime = 0.0f;
}