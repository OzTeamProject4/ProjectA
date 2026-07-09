using UnityEngine;

public class EnemyService 
{
    // 몬스터와 관련된 전반적인 서비스를 담당
    // 관련 인스턴스 데이터들을 여기에 보관
    private EnemyViewModel _EnemyViewModel;

    public EnemyViewModel GetEnemyViewModel(string enemyDataId)
    {
        if (_EnemyViewModel == null)
        {
            CreateEnemyViewModel(enemyDataId);
        }

        return _EnemyViewModel;
    }
   
    public EnemyViewModel CreateEnemyViewModel(string enemyDataId)
    {
       

        if (_EnemyViewModel == null)
        {
            // 뷰모델을 일단 생성 한다
            var localPlayerStatVm = new EnemyViewModel();
            SetEnemyData(localPlayerStatVm, enemyDataId: "데이터 id", name : "기본 이름", totalExp: 10000, elementalType: "Fire", baseHp: 1, baseDamage: 1,PrefabAddress: "프리팹 경로");
            // 위랑 똑같은 로직이지만, 이 파라미터가 뭔지 설명해줄 수 있다(남용하진 말고, 이렇게 스탯이나 수치 초기화 하는 경우만 사용) - SetPlayerStatData(localPlayerStatVm,0,0,0,1,100);
            _EnemyViewModel = localPlayerStatVm;


           /* // 데이터를 받아오고, 성공하면 내용을 채워준다
            var characterData = DaniTechGameDataManager.Instance.GetCharacterData("character_ozbasic_01");
            if (characterData == null)
            {
                Debug.LogError($"플레이어 데이터를 찾을 수 없습니다! : DNNetworkPlayerService");
                return _localEnemyViewModel;
            }

            var baseStatDataId = characterData.BaseStatDataId;
            if (string.IsNullOrEmpty(baseStatDataId) == true)
            {
                Debug.LogError($"베이스 스탯이 없는 캐릭터 입니다! : DNNetworkPlayerService");
                return _localEnemyViewModel;
            }

            var baseStatData = DaniTechGameDataManager.Instance.GetDNBaseStatData(baseStatDataId);
            if (baseStatData == null)
            {
                Debug.LogError($"스탯 데이터를 찾을 수 없습니다! : DNNetworkPlayerService : {baseStatDataId}");
                return _localEnemyViewModel;
            }

            SetEnemyData(localPlayerStatVm
                , baseAtk: baseStatData.BaseAtk
                , baseCrit: baseStatData.BaseCrit
                , baseHp: baseStatData.BaseHp
                , baseMoveSpeed: baseStatData.BaseMoveSpeed
                , baseAtkSpeed: baseStatData.BaseAtkSpeed);*/
        }

        return _EnemyViewModel;
    }

    public void RequestAddExpToEnemy(int exp)
    {
        if (_EnemyViewModel != null)
        {
            _EnemyViewModel.TotalExp += exp;
        }
    }
    public void RequestAddStatDamageToEnemy(int addDamageValue)
    {
        if (_EnemyViewModel != null)
        {
            _EnemyViewModel.BaseDamage += addDamageValue;
        }
    }

    private void SetEnemyData(EnemyViewModel vm,string enemyDataId, string name, int totalExp, string elementalType, int baseHp, float baseDamage,string PrefabAddress)
    {
        vm.EnemyDataId = enemyDataId;
        vm.Name = name;
        vm.TotalExp = totalExp;
        vm.ElementalType = elementalType;
        vm.BaseHp = baseHp;
        vm.BaseDamage = baseDamage;
        vm.PrefabAddress = PrefabAddress;
    }

   
}
