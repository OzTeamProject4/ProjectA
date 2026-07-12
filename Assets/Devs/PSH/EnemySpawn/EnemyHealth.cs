using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    // 적 사망 시 스포너에 알림
    public event Action<GameObject> onDead;

    // 체력 변경 시 UI에 전달
    public event Action<int, int> onHealthChanged;

    // 현재 적의 전투 상태
    private EnemyViewModel enemyViewModel;

    // 스폰된 적의 ViewModel 연결
    public void Bind(EnemyViewModel viewModel)
    {
        if (viewModel == null)
        {
            Debug.LogError("EnemyViewModel is missing.");
            return;
        }

        Unbind();

        enemyViewModel = viewModel;
        enemyViewModel.onHealthChanged += HandleHealthChanged;
        enemyViewModel.onDead += HandleDead;

        HandleHealthChanged(enemyViewModel.CurrentHp, enemyViewModel.MaxHp);
    }

    // 외부 피해를 ViewModel에 전달
    public void TakeDamage(int damage)
    {
        if (enemyViewModel == null)
        {
            Debug.LogError("EnemyHealth is not bound to an EnemyViewModel.");
            return;
        }

        enemyViewModel.TakeDamage(damage);
    }

    // ViewModel의 체력 변경 전달
    private void HandleHealthChanged(int currentHp, int maxHp)
    {
        onHealthChanged?.Invoke(currentHp, maxHp);
    }

    // ViewModel의 사망 이벤트 전달
    private void HandleDead()
    {
        onDead?.Invoke(gameObject);
    }

    // 오브젝트 제거 전 이벤트 해제
    private void OnDestroy()
    {
        Unbind();
    }

    // 이전 ViewModel 이벤트 해제
    private void Unbind()
    {
        if (enemyViewModel == null)
        {
            return;
        }

        enemyViewModel.onHealthChanged -= HandleHealthChanged;
        enemyViewModel.onDead -= HandleDead;
        enemyViewModel = null;
    }
}
