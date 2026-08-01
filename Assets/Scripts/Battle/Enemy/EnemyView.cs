using System.ComponentModel;
using UnityEngine;

public class EnemyView : ViewBase
{
    [SerializeField] private Transform _headAnchor;
    public Transform HeadAnchor => _headAnchor != null ? _headAnchor : transform;

    private EnemyViewModel _vm;


   
    public void BindEnemyViewModel(EnemyViewModel vm)
    {
        _vm = vm;
        _vm.PropertyChanged += OnPropChagned_View;
        _vm.InvokeOnceOnInit();
    }

    public void SetHead(Transform targetHeadTransform) {
        _headAnchor = targetHeadTransform;
    }


    private void OnDestroy()
    {
        if (_vm != null)
        {
            _vm.PropertyChanged -= OnPropChagned_View;
        }
    }
    private void OnPropChagned_View(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(EnemyViewModel.CurrentLevel):
                {
                }
                break;
        }
    }
}
