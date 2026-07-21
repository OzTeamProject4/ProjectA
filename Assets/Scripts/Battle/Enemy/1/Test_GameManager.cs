using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using UnityEngine;

public class Test_GameManager : MonoBehaviour
{
    public static Test_GameManager Inst { get; set; }

    private void Awake()
    {
        Inst = this;
    }
   
    


}