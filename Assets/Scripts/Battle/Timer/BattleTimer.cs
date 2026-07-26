using System;

public class BattleTimer
{
    private float _battleTime;
    private float _remainTime;
    private bool _isBattleRunning;

    public event Action OnTimeOver;

    public float RemainTime
    {
        get
        {
             return _remainTime; 
        }
    }

    public BattleTimer(float battleTime)
    {
        _battleTime = battleTime;
    }

    public void Tick(float deltaTime)
    {
        if (_isBattleRunning == false)
        {
            return;
        }

        _remainTime -= deltaTime;

        if (_remainTime <= 0f)
        {
            _remainTime = 0f;
            _isBattleRunning = false;
            OnTimeOver?.Invoke();
        }
    }
    public void StartTimer()
    {
        _remainTime = _battleTime;
        _isBattleRunning = true;
    }

    public void StopTimer()
    {
        _isBattleRunning = false;
    }
    
}
