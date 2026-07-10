using UnityEngine;

public class GameManager : BaseManager<GameManager>
{
    public static GameManager Instance { get; private set; }

    public ResourceManager ResourceManager { get; private set; }

    public DataManager DataManager { get; private set; }

    private void Awake()
    {
        EnsureSingleton();
        SetupManagers();
        InitializeManagers();
        
    }

    public override void Initialize()
    {
        Debug.Log("게임 매니저 초기화");
    }

    private void EnsureSingleton()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"[GameManager:EnsureSingleton] 중복된 인스턴스가 발견되어 {gameObject.name} 오브젝트를 파괴합니다.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void SetupManagers()
    {
        ResourceManager = this.GetRequiredComponent<ResourceManager>();
        DataManager = this.GetRequiredComponent<DataManager>();
    }

    private void InitializeManagers()
    {
        Initialize();
        ResourceManager.Initialize();
        DataManager.Initialize();
    }
}