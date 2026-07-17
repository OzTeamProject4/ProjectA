using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class BaseButton : MonoBehaviour
{
    [SerializeField] protected Button _button;

    protected virtual void Awake()
    {
        UnityUtil.ValidateReference(_button, nameof(BaseButton), nameof(_button));
    }

    protected virtual void OnEnable()
    {
        AddOnClickListener(OnButtonClick);
    }

    protected virtual void OnDisable()
    {
        RemoveOnClickListeners();
    }

    public virtual void SetButtonInteractable(bool interactable)
    {
        _button.interactable = interactable;
    }

    private void AddOnClickListener(UnityAction onClick)
    {
        _button.onClick.AddListener(onClick);
    }

    private void RemoveOnClickListeners()
    {
        _button.onClick.RemoveAllListeners();
    }

    protected abstract void OnButtonClick();
}