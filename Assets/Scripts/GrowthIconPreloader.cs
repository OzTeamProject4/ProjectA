using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GrowthIconPreloader
{
    private readonly IGameDataProvider _dataProvider;
    private readonly List<string> _loadedSpriteKeys = new();

    public GrowthIconPreloader(IGameDataProvider dataProvider)
    {
        if (null == dataProvider)
        {
            Debug.LogError("[GrowthIconPreloader] dataProvider 가 null 입니다.");
        }

        _dataProvider = dataProvider;
    }

    public async UniTask PreloadAsync(CancellationToken cancellationToken)
    {
        foreach (EquipmentData data in _dataProvider.GetAllEquipment())
        {
            await LoadSpriteAsync(data.SpritePath, cancellationToken);
        }

        foreach (ItemData item in _dataProvider.GetAllItems())
        {
            await LoadSpriteAsync(item.SpritePath, cancellationToken);
        }
    }

    public void Release()
    {
        foreach (string key in _loadedSpriteKeys)
        {
            GameManager.Instance.ResourceManager.ReleaseAsset(key);
        }

        _loadedSpriteKeys.Clear();
    }

    private async UniTask LoadSpriteAsync(string spritePath, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(spritePath))
        {
            return;
        }

        try
        {
            await GameManager.Instance.ResourceManager.LoadAssetAsync<Sprite>(spritePath, cancellationToken);
            _loadedSpriteKeys.Add(spritePath);
        }
        catch (OperationCanceledException)
        {
            // 화면 파괴로 취소됨, 무시
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"[GrowthIconPreloader] 아이콘 프리로드 실패. spritePath={spritePath}\n{exception}");
        }
    }
}