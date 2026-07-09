using UnityEngine;

public class Test_GameManager : MonoBehaviour
{
    public static Test_GameManager Inst { get; set; }

    public EnemyService EnemyService { get; private set; }

    private void Awake()
    {
        Inst = this;
        InitService();

    }
    private void OnEnable()
    {
        RequestCreateEnemy("데이터 ID가 들어갈 곳");

    }

    private void InitService()
    {
        // 앞으로 매니저에서 사용할 다양한 서비스를 생성
        EnemyService = new EnemyService();

    }
    public void RequestCreateEnemy(string enemyDataId)
    {
        // 게임 시작이나, 맵 진입 시 로컬 플레이어를 서버에 생성하는 요청
        EnemyViewModel localEnemyVm = EnemyService.GetEnemyViewModel(enemyDataId);

        // 응답 받았다고 가정한다 = 추후 실제 서버 통신시에는 람다나 비동기 로직으로 받아온다
        Test_GameObjectManager.inst.CreateEnemy(localEnemyVm);
    }

}