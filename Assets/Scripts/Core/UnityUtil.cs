using UnityEngine;

public static class UnityUtil
{
    public static void ValidateReference(Object target, string className, string fieldName)
    {
        if (target != null)
        {
            return;
        }

        Debug.LogError($"[{nameof(UnityUtil)}:{nameof(ValidateReference)}] '{className}' 클래스의 '{fieldName}' 필드가 할당되지 않았습니다.");
    }
}
