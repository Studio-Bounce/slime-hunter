using UnityEngine;
using UnityEngine.UI;

public class POISION : MonoBehaviour
{
    [SerializeField] private Text scoreText;  // 用于显示分数的UI文本
    public int score = 0;  // 分数变量
    private bool isPlayerInContact = false;  // 检测Player是否在接触

    private void Start()
    {
        // 初始化UI文本
        if (scoreText != null)
        {
            scoreText.text = "毒素: " + score;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 检查碰撞物体是否在“Player”层
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            isPlayerInContact = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // 当Player离开时，停止增加分数
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            isPlayerInContact = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 检查触发器物体是否在“Player”层
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            isPlayerInContact = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 当Player离开时，停止增加分数
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            isPlayerInContact = false;
        }
    }

    private void FixedUpdate()
    {
        // 每帧检测并增加分数
        if (isPlayerInContact)
        {
            IncreaseScore();
        }
    }

    private void IncreaseScore()
    {
        score += 1;
        if (scoreText != null)
        {
            scoreText.text = "毒素: " + score;
        }
    }

    public void UpdateScoreText()
{
    if (scoreText != null)
    {
        scoreText.text = "毒素: " + score;
    }
}
}