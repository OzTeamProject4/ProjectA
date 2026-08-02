using UnityEngine;

public class StagePlayerPartyRoot : MonoBehaviour
{
    [SerializeField] private StagePlayerParty _playerParty;

    public StagePlayerParty PlayerParty
    {
        get { return _playerParty; }
    }
}