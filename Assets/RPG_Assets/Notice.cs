using UnityEngine;
using UnityEngine.UI;

public class Notice : MonoBehaviour
{
    public Text uiText; // 拖动UI中的Text组件到此变量中

    private void Start()
    {
        // 确保UI文本最开始是非激活状态
        if (uiText != null)
        {
            uiText.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 检查碰撞体的图层是否是"Player"
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            // 激活UI文本
            if (uiText != null)
            {
                uiText.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 检查碰撞体的图层是否是"Player"
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            // 取消激活UI文本
            if (uiText != null)
            {
                uiText.gameObject.SetActive(false);
            }
        }
    }
}