using UnityEngine;

public class StageSelectPlayer
{
    private readonly StagePlayerPartyRoot _prefab;
    private readonly Transform _parent;

    private StagePlayerParty _party;

    public StagePlayerParty Party
    {
        get { return _party; }
    }

    public bool IsSpawned
    {
        get { return null != _party; }
    }

    public Vector3 Position
    {
        get { return null != _party ? _party.transform.position : Vector3.zero; }
    }

    public StageSelectPlayer(StagePlayerPartyRoot prefab, Transform parent)
    {
        _prefab = prefab;
        _parent = parent;
    }

    public bool Spawn(Transform spawnPoint)
    {
        if (null == _prefab)
        {
            Debug.LogError("[StageSelectPlayer] _prefab 이 null 입니다.");
            return false;
        }

        if (null == spawnPoint)
        {
            Debug.LogError("[StageSelectPlayer] spawnPoint 가 null 입니다.");
            return false;
        }

        StagePlayerPartyRoot root = Object.Instantiate(_prefab, spawnPoint.position, spawnPoint.rotation, _parent);
        _party = root.PlayerParty;

        return null != _party;
    }

    public void Bind(StagePlayerPartyViewModel viewModel)
    {
        if (null == _party)
        {
            Debug.LogError("[StageSelectPlayer] Bind: _party 가 null 입니다.");
            return;
        }

        _party.Bind(viewModel);
    }

    public void WarpTo(Vector3 position)
    {
        if (null == _party)
        {
            return;
        }

        _party.WarpTo(position);
    }

}
