using UnityEngine;

public class UILayer : MonoBehaviour
{
    [SerializeField] private RectTransform _a;
    [SerializeField] private RectTransform _b;
    [SerializeField] private RectTransform _c;
    [SerializeField] private RectTransform _d;
    [SerializeField] private RectTransform _e;

    public RectTransform A
    { 
        get { return _a; } 
    }

    public RectTransform B 
    {
        get { return _b; } 
    }

    public RectTransform C
    { 
        get { return _c; }
    }

    public RectTransform D 
    { 
        get { return _d; }
    }

    public RectTransform E 
    {
        get { return _e; } 
    }

    private void Awake()
    {
        ValidateReferences();
    }

    private void ValidateReferences()
    {
        ValidateReference(_a, nameof(_a));
        ValidateReference(_b, nameof(_b));
        ValidateReference(_c, nameof(_c));
        ValidateReference(_d, nameof(_d));
        ValidateReference(_e, nameof(_e));
    }

    private void ValidateReference(RectTransform rectTransform, string fieldName)
    {
        if (rectTransform == null)
        {
            Debug.LogError($"[UILayer:ValidateReference] {fieldName} 필드 레퍼런스가 할당되지 않았습니다.");
        }
    }
}