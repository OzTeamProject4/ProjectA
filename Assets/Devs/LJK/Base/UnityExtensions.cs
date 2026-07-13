using UnityEngine;

public static class UnityExtensions
{
    public static T GetRequiredComponent<T>(this Component component) where T : Component
    {
        if (component == null)
        {
            Debug.LogError($"[ComponentUtility:GetRequiredComponent] 전달된 컴포넌트가 null입니다.");
            return null;
        }

        if (!component.TryGetComponent(out T result))
        {
            Debug.LogError($"[ComponentUtility:GetRequiredComponent] {component.name}에 {typeof(T).Name} 컴포넌트가 없습니다.");
            return null;
        }

        return result;
    }

    public static T GetRequiredComponent<T>(this GameObject gameObject) where T : Component
    {
        if (gameObject == null)
        {
            Debug.LogError($"[ComponentUtility:GetRequiredComponent] 전달된 게임 오브젝트가 null입니다.");
            return null;
        }

        if (!gameObject.TryGetComponent(out T result))
        {
            Debug.LogError($"[ComponentUtility:GetRequiredComponent] {gameObject.name}에 {typeof(T).Name} 컴포넌트가 없습니다.");
            return null;
        }

        return result;
    }
}