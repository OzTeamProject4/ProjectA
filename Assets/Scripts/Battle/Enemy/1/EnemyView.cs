using System.ComponentModel;
using UnityEngine;

public class EnemyView : ViewBase
{
    // 플레이어 3D 모델, 애니메이션, 물리에 관한 뷰
    [SerializeField] private TextMesh _textMesh_Name;
    [SerializeField] private TextMesh _textMesh_Level;
    [SerializeField] private TextMesh _textMesh_CurrentHp;

    // 뷰에서 절대 new로 VewModel을 하지 않고, 매니저를 통해
    // 실제 생성된 뷰 모델을 받아와야한다!
    private EnemyViewModel _vm;


   
    public void BindEnemyViewModel(EnemyViewModel vm)
    {
        _vm = vm;
        _vm.PropertyChanged += OnPropChagned_View;
        _vm.InvokeOnceOnInit();
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
            case nameof(EnemyViewModel.Name):
                {
                    _textMesh_Name.text = _vm.Name;
                }
                break;
            case nameof(EnemyViewModel.CurrentLevel):
                {
                    _textMesh_Level.text = $"Lv.{_vm.CurrentLevel}";
                }
                break;
            case nameof(EnemyViewModel.CurrentHp):
                {
                    _textMesh_Level.text = $"Hp.{_vm.CurrentHp}";
                }
                break;

        }
    }

   
}
