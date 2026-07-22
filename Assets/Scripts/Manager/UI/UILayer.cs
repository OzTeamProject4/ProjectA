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
        UnityUtil.ValidateReference(_a, nameof(UILayer), nameof(_a));
        UnityUtil.ValidateReference(_b, nameof(UILayer), nameof(_b));
        UnityUtil.ValidateReference(_c, nameof(UILayer), nameof(_c));
        UnityUtil.ValidateReference(_d, nameof(UILayer), nameof(_d));
        UnityUtil.ValidateReference(_e, nameof(UILayer), nameof(_e));
    }
}