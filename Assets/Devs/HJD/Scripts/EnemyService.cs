using System.Collections.Generic;
using UnityEngine;

public class EnemyService
{
    // 관련 인스턴스 데이터들을 여기에 보관
    private EnemyViewModel _enemyViewModel;
    private Dictionary<int, EnemyViewModel> _enemyViewModelList = new Dictionary<int, EnemyViewModel>();
    private int _enemyViewModelInstanceId = 0;

    public EnemyViewModel CreateEnemyViewModel(string enemyDataId)
    {



        // 뷰모델을 일단 생성 한다
        var enemyVm = new EnemyViewModel();
        SetEnemyData(enemyVm, enemyInstanceId: 0, enemyDataId: "데이터 id", name: "기본 이름", totalExp: 10000, elementalType: "Fire", baseHp: 1, baseDamage: 1, PrefabAddress: "프리팹 경로");
        // 위랑 똑같은 로직이지만, 이 파라미터가 뭔지 설명해줄 수 있다(남용하진 말고, 이렇게 스탯이나 수치 초기화 하는 경우만 사용) - SetPlayerStatData(localPlayerStatVm,0,0,0,1,100);
        _enemyViewModel = enemyVm;


        // 데이터를 받아오고, 성공하면 내용을 채워준다
        if (GameManager.Instance.DataManager.TryGetData<EnemyData>(enemyDataId, out EnemyData enemyData)) { }

        if (enemyData == null)
        {
            Debug.LogError($"적 데이터를 찾을 수 없습니다! : EnemyService");
            return _enemyViewModel;
        }

        SetEnemyData(enemyVm
            , _enemyViewModelInstanceId
            , enemyData.DataId
            , enemyData.Name
            , enemyData.TotalExp
            , enemyData.ElementalType
            , enemyData.BaseHp
            , enemyData.BaseDamage
            , enemyData.PrefabAddress);

        _enemyViewModelList.Add(_enemyViewModelInstanceId, enemyVm);
        _enemyViewModelInstanceId++;

        return _enemyViewModel;
    }

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

    private void SetEnemyData(EnemyViewModel vm, int enemyInstanceId, string enemyDataId, string name, int totalExp, string elementalType, float baseHp, float baseDamage, string PrefabAddress)
    {
        vm.EnemyInstanceId = enemyInstanceId;
        vm.EnemyDataId = enemyDataId;
        vm.Name = name;
        vm.TotalExp = totalExp;
        vm.ElementalType = elementalType;
        vm.BaseHp = baseHp;
        vm.BaseDamage = baseDamage;
        vm.PrefabAddress = PrefabAddress;
    }


}
