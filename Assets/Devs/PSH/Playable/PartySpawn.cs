using UnityEngine;

public class SpawnPlayer : MonoBehaviour
{
    [SerializeField] private PartySpawnData[] _partyMembers;

    [SerializeField] private Transform[] _spawnPoints;

    [SerializeField] private float _spawnHeightOffset = 1f;

    [SerializeField] private Transform _spawnParent;

    private void Start()
    {
        SpawnPartyMembers();
    }

    // 파티원 스폰 시작
    private void SpawnPartyMembers()
    {
        // 파티 멤버가 없다면?
        if (_partyMembers == null || _partyMembers.Length == 0)
        {
            Debug.LogWarning("No party members are assigned.");
            return;
        }


        // 스폰포인트가 없다면?
        if (_spawnPoints == null || _spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points are assigned.");
            return;
        }


        // 둘다 있다면? 파티원 배열을 끝까지 한번씩 검사
        for (int i = 0; i < _partyMembers.Length; i++)
        {
            PartySpawnData partyMember = _partyMembers[i];

            // 스폰이 가능한 지 검사
            if (!CanSpawnPartyMember(partyMember, i))
            {
                continue;
            }

            // 스폰 시작
            SpawnPartyMember(partyMember);
        }
    }

    private bool CanSpawnPartyMember(PartySpawnData partyMember, int index)
    {
        // 파티 멤버 미지정
        if (partyMember == null)
        {
            Debug.LogError($"Party member data at index {index} is missing.");
            return false;
        }

        // 파티 멤버 프리팹 없음
        if (partyMember.CharacterPrefab == null)
        {
            Debug.LogWarning($"Character prefab at party index {index} is missing.");
            return false;
        }

        // 파티 멤버의 스폰 포인트 번호의 범위 이상
        if (partyMember.SpawnPointIndex < 0 || partyMember.SpawnPointIndex >= _spawnPoints.Length)
        {
            Debug.LogError($"Spawn point index {partyMember.SpawnPointIndex} at party index {index} is invalid.");
            return false;
        }

        // 해당 번호의 스폰 위치가 실제로 Inspector에 연결되어 있지 않음
        if (_spawnPoints[partyMember.SpawnPointIndex] == null)
        {
            Debug.LogError($"Spawn point at index {partyMember.SpawnPointIndex} is missing.");
            return false;
        }

        return true;
    }

    private void SpawnPartyMember(PartySpawnData partyMember)
    {
        Transform spawnPoint = _spawnPoints[partyMember.SpawnPointIndex];

        Instantiate(
            partyMember.CharacterPrefab,
            spawnPoint.position + Vector3.up * _spawnHeightOffset,
            spawnPoint.rotation,
            _spawnParent
        );
    }
}

[System.Serializable]
public class PartySpawnData
{
    [SerializeField] private GameObject _characterPrefab;
    [SerializeField] private int _spawnPointIndex;

    public GameObject CharacterPrefab
    {
        get { return _characterPrefab; }
    }

    public int SpawnPointIndex
    {
        get
        { return _spawnPointIndex; }
    }
}

/*
여기는 파티 편성 창에서 데이터를 받는다는 상황을 가정한 코드입니다

using UnityEngine;

public class SpawnPlayer : MonoBehaviour
{
    // 씬에 배치된 실제 스폰 위치들
    // Element 0 = 왼쪽, Element 1 = 가운데, Element 2 = 오른쪽 같은 식으로 약속해서 사용
    [SerializeField] private Transform[] _spawnPoints;

    // 캐릭터를 스폰 위치보다 살짝 위에 생성하기 위한 높이 보정값
    [SerializeField] private float _spawnHeightOffset = 1f;

    // 생성된 캐릭터들을 이 Transform의 자식으로 묶고 싶을 때 사용
    [SerializeField] private Transform _spawnParent;

    // 파티 편성 쪽에서 완성된 파티 데이터를 넘겨주면, 그 데이터를 기준으로 캐릭터들을 스폰한다
    public void SpawnPartyMembers(PartySpawnData[] partyMembers)
    {
        // 파티 데이터 배열 자체가 없거나, 안에 아무 데이터도 없다면 스폰할 수 없다
        if (partyMembers == null || partyMembers.Length == 0)
        {
            Debug.LogWarning("No party members are assigned.");
            return;
        }

        // 스폰 위치 배열 자체가 없거나, 안에 아무 위치도 없다면 캐릭터를 배치할 수 없다
        if (_spawnPoints == null || _spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points are assigned.");
            return;
        }

        // 파티 멤버 데이터를 하나씩 확인하면서 스폰한다
        for (int i = 0; i < partyMembers.Length; i++)
        {
            PartySpawnData partyMember = partyMembers[i];

            // 현재 파티 멤버가 스폰 가능한 데이터인지 검사한다
            // 문제가 있으면 continue로 아래 SpawnPartyMember 호출을 건너뛴다
            if (!CanSpawnPartyMember(partyMember, i))
            {
                continue;
            }

            // 검사를 통과한 파티 멤버만 실제로 생성한다
            SpawnPartyMember(partyMember);
        }
    }

    // 파티 멤버 한 명이 실제로 스폰 가능한 상태인지 검사한다
    private bool CanSpawnPartyMember(PartySpawnData partyMember, int index)
    {
        // 파티 멤버 데이터 자체가 비어 있으면 스폰할 수 없다
        if (partyMember == null)
        {
            Debug.LogWarning($"Party member data at index {index} is missing.");
            return false;
        }

        // 생성할 캐릭터 프리팹이 비어 있으면 스폰할 수 없다
        if (partyMember.CharacterPrefab == null)
        {
            Debug.LogWarning($"Character prefab at party index {index} is missing.");
            return false;
        }

        // 파티 편성 쪽에서 넘긴 스폰 위치 번호가 배열 범위 밖이면 스폰할 수 없다
        if (partyMember.SpawnPointIndex < 0 || partyMember.SpawnPointIndex >= _spawnPoints.Length)
        {
            Debug.LogWarning($"Spawn point index {partyMember.SpawnPointIndex} at party index {index} is invalid.");
            return false;
        }

        // 스폰 위치 번호는 맞지만, 해당 위치 Transform이 Inspector에 연결되어 있지 않으면 스폰할 수 없다
        if (_spawnPoints[partyMember.SpawnPointIndex] == null)
        {
            Debug.LogWarning($"Spawn point at index {partyMember.SpawnPointIndex} is missing.");
            return false;
        }

        // 위 검사를 전부 통과했으므로 스폰 가능
        return true;
    }

    // 검사를 통과한 파티 멤버를 실제 씬에 생성한다
    private void SpawnPartyMember(PartySpawnData partyMember)
    {
        // 파티 멤버가 가지고 있는 스폰 위치 번호로 실제 Transform을 찾는다
        Transform spawnPoint = _spawnPoints[partyMember.SpawnPointIndex];

        // 캐릭터 프리팹을 지정된 위치와 회전값으로 생성한다
        Instantiate(
            partyMember.CharacterPrefab,
            spawnPoint.position + Vector3.up * _spawnHeightOffset,
            spawnPoint.rotation,
            _spawnParent
        );
    }
}

[System.Serializable]
public class PartySpawnData
{
    // 생성할 캐릭터 프리팹
    [SerializeField] private GameObject _characterPrefab;

    // 어느 스폰 위치에 생성할지 나타내는 번호
    // 예: 0 = 왼쪽, 1 = 가운데, 2 = 오른쪽
    [SerializeField] private int _spawnPointIndex;

    // 외부에서 캐릭터 프리팹을 읽을 수 있게 하는 프로퍼티
    public GameObject CharacterPrefab
    {
        get { return _characterPrefab; }
    }

    // 외부에서 스폰 위치 번호를 읽을 수 있게 하는 프로퍼티
    public int SpawnPointIndex
    {
        get { return _spawnPointIndex; }
    }

    // 파티 편성 쪽에서 코드로 PartySpawnData를 만들 때 사용하는 생성자
    public PartySpawnData(GameObject characterPrefab, int spawnPointIndex)
    {
        _characterPrefab = characterPrefab;
        _spawnPointIndex = spawnPointIndex;
    }
}


*/