using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

// 어드레서블 키로 스프라이트를 받아 Image에 넣는 공통 처리.
// 키가 비면 InvalidKeyException이 나므로 반드시 걸러야 하고, 실패 시 이미지를 꺼서 이전 그림이 남지 않게 한다
public static class SpriteLoader
{
    public static async UniTask LoadIntoAsync(Image image, string spriteKey, CancellationToken cancellationToken)
    {
        if (image == null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(spriteKey))
        {
            image.enabled = false;
            return;
        }

        Sprite sprite = await GameManager.Instance.ResourceManager.LoadAssetAsync<Sprite>(spriteKey, cancellationToken);

        if (sprite == null)
        {
            image.enabled = false;
            return;
        }

        image.enabled = true;
        image.sprite = sprite;
    }
}
