using System;

public sealed class EnemyViewModel
{
    // JSON 원본 적 정보
    public EnemyData Data { get; }

    // 전투 중 변하는 체력
    public int CurrentHp { get; private set; }
    public int MaxHp { get; }

    // JSON에서 받은 기본 공격력
    public int BaseDamage { get; }

    // 체력 변경과 사망 알림
    public event Action<int, int> onHealthChanged;
    public event Action onDead;

    // JSON 능력치로 전투 상태 생성
    public EnemyViewModel(EnemyData enemyData)
    {
        if (enemyData == null)
        {
            throw new ArgumentNullException(nameof(enemyData));
        }

        Data = enemyData;
        MaxHp = enemyData.BaseHp;
        CurrentHp = MaxHp;
        BaseDamage = enemyData.BaseDamage;
    }

    // 피해를 받아 현재 체력 변경
    public void TakeDamage(int damage)
    {
        if (CurrentHp <= 0 || damage <= 0)
        {
            return;
        }

        CurrentHp -= damage;

        if (CurrentHp < 0)
        {
            CurrentHp = 0;
        }

        // View에 체력 변경 알림
        onHealthChanged?.Invoke(CurrentHp, MaxHp);

        if (CurrentHp == 0)
        {
            // 체력이 0이면 사망 알림
            onDead?.Invoke();
        }
    }
}
