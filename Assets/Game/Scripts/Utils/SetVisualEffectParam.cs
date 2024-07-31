using UnityEngine;
using UnityEngine.VFX;

public class SetVisualEffectParam : MonoBehaviour
{
    public VisualEffect vfx;
    public string paramName;
    void Awake()
    {
        Debug.Assert(vfx != null, "VisualEffect Required");
    }

    public void SetInt(int value)
    {
        vfx.SetInt(paramName, value);
    }

    public void SetFloat(float value)
    {
        vfx.SetFloat(paramName, value);
    }

    public void SetBool(bool value)
    {
        vfx.SetBool(paramName, value);
    }

    public void SetVector2(Vector2 value)
    {
        vfx.SetVector2(paramName, value);
    }

    public void SetVector3(Vector3 value)
    {
        vfx.SetVector3(paramName, value);
    }
}
