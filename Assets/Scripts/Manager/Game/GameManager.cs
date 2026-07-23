using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameManager : BaseManager<GameManager>
{
    public static GameManager Instance { get; private set; }

    public ResourceManager ResourceManager { get; private set; }

    public DataManager DataManager { get; private set; }

    public AudioManager AudioManager { get; private set; }

    public UIManager UIManager { get; private set; }
    public Inventory Inventory { get; private set; }

    public InputManager InputManager { get; private set; }

    public ObjectManager ObjectManager { get; private set; }

    public BattleManager BattleManager { get; private set; }


    private void Awake()
    {
        EnsureSingleton();
        SetupManagers();
    }

    public override UniTask InitializeAsync()
    {
        return UniTask.CompletedTask;
    }

    public async UniTask InitializeManagersAsync()
    {
        await InitializeAsync();
        await ResourceManager.InitializeAsync();
        await DataManager.InitializeAsync();
        await AudioManager.InitializeAsync();
        await UIManager.InitializeAsync();
        await InputManager.InitializeAsync();
        await ObjectManager.InitializeAsync();
        await BattleManager.InitializeAsync();
    }

    private void EnsureSingleton()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"[{nameof(GameManager)}:{nameof(EnsureSingleton)}] 중복된 인스턴스가 발견되어 {gameObject.name} 오브젝트를 파괴합니다.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void SetupManagers()
    {
        ResourceManager = this.GetRequiredComponent<ResourceManager>();
        DataManager = this.GetRequiredComponent<DataManager>();
        AudioManager = this.GetRequiredComponent<AudioManager>();
        UIManager = this.GetRequiredComponent<UIManager>();
        InputManager = this.GetRequiredComponent<InputManager>();
        ObjectManager = this.GetRequiredComponent<ObjectManager>();
        BattleManager = this.GetRequiredComponent<BattleManager>();

        Inventory = new Inventory();
    }
}