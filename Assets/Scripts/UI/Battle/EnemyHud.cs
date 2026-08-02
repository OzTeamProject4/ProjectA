using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHud : BaseUI
{
    [SerializeField] private string _enemyHudSlotId;
    private List<GameObject> _hudSlotList = new List<GameObject>();
    public async UniTask AddEnemyHudSlot(EnemyViewModel enemyViewModel, Transform headAnchor)
    {
        if (enemyViewModel == null)
        {
            return;
        }

        GameObject hudSlot = await GameManager.Instance.ObjectManager.SpawnAsync(_enemyHudSlotId, gameObject.transform, Vector3.zero, Quaternion.identity);

        if (hudSlot != null)
        {
            hudSlot.transform.localScale = Vector3.one;
            _hudSlotList.Add(hudSlot);

            if (hudSlot.TryGetComponent(out EnemyHudSlot enemyHudSlot))
            {
                enemyHudSlot.BindEnemyViewModel(enemyViewModel, headAnchor);
            }
        }

    }
    public void RemoveHudSlot(GameObject hudSlot)
    {
        if (_hudSlotList.Remove(hudSlot))
        {
            GameManager.Instance.ObjectManager.Despawn(hudSlot);
        }
    }


}
