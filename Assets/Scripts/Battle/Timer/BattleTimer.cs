using UnityEngine;
using TMPro;
using UnityEngine.Events;

// 해당 파일을 EnemySpawnManager의 OnBattleEnd에 넣어주세요

public class BattleTimer : MonoBehaviour
{
    [SerializeField] private float battleTime = 120f;
    [SerializeField] private TMP_Text timerText;  // 텍스트로 시간 표시 테스트
    [SerializeField] private UnityEvent onTimeOver; 

    private float remainTime;

    private bool isBattleRunning;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartTimer();
        isBattleRunning = true;
        Debug.Log("타이머 시작");
    }

    // Update is called once per frame
    void Update()
    {
        if(!isBattleRunning)
        {
            return;
        }

        remainTime -= Time.deltaTime;

        if(remainTime <= 0f)
        {
            remainTime = 0f;
            isBattleRunning = false;
            UpdateTimerText();


            Debug.Log("Time Over - Lose");
            onTimeOver.Invoke();
            // UI 먼저 띄우고 Time.timeScale = 0f가 되어야 합니다!!!!
            Time.timeScale = 0f;
            return;
        }

        UpdateTimerText();
    }

    private void StartTimer()
    {
        remainTime = battleTime;
        isBattleRunning = true;
        UpdateTimerText();
    }


    // 제한 시간 만료 전 전투 종료시
    public void StopTimer()
    {
        isBattleRunning = false;
    }

    private void UpdateTimerText()
    {
        int minute = Mathf.FloorToInt(remainTime / 60f);
        int second = Mathf.FloorToInt(remainTime % 60f);

        timerText.text = $"{minute:00}:{second:00}";
    }
}
