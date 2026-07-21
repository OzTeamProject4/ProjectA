using Unity.Behavior;
using UnityEngine;

public class EnemySkillController : MonoBehaviour
{
    public EnemySkillViewModel vm;
    [SerializeField] private float speed = 10f;

    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
       
    }

    private void FixedUpdate()
    {
        // 바라보는 방향(forward)으로 속도 부여
        _rb.linearVelocity = transform.forward * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
            Test_GameObjectManager.Inst.DespawnSkill(vm.InstanceId);
        
    }

}
