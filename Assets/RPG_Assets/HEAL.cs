using UnityEngine;

public class HEAL : MonoBehaviour
{
    [SerializeField] private POISION poisonScript; // 引用POISION脚本

    private void OnTriggerEnter(Collider other)
    {
        // 检查触发器物体是否在“Player”层
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            // 将分数归零
            if (poisonScript != null)
            {
                poisonScript.score = 0;
                poisonScript.UpdateScoreText();
            }
        }
    }
}