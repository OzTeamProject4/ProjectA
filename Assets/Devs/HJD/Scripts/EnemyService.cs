using System.Collections.Generic;
using UnityEngine;

public class EnemyService
{
    // 관련 인스턴스 데이터들을 여기에 보관
    private EnemyViewModel _enemyViewModel;

    

    public void RequestAddExpToEnemy(int exp)
    {
        if (_enemyViewModel != null)
        {
            _enemyViewModel.TotalExp += exp;
        }
    }
    public void RequestAddStatDamageToEnemy(int addDamageValue)
    {
        if (_enemyViewModel != null)
        {
            _enemyViewModel.BaseDamage += addDamageValue;
        }
    }

   


}
