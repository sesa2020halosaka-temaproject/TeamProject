using UnityEngine;

public class FixedScreenScaleObject : MonoBehaviour
{
    private const float INVAILD_BASE_SCALE = float.MinValue;

    /// <summary>
    /// カメラからの距離が1のときのスケール値
    /// </summary>
    [SerializeField]
    private float _baseScale = INVAILD_BASE_SCALE;

    public Camera m_TargetCamera;

    private void Start()
    {
        if (_baseScale != INVAILD_BASE_SCALE) return;
        Debug.Log("[1]" + transform.position);

        // カメラからの距離が1のときのスケール値を算出
        _baseScale = transform.localScale.x / GetDistance();

    }

    /// <summary>
    /// カメラからの距離を取得
    /// </summary>
    private float GetDistance()
    {
        float x1 = this.transform.position.x;
        float y1 = this.transform.position.y;
        float z1 = this.transform.position.z;
        float x2 = m_TargetCamera.transform.position.x;
        float y2 = m_TargetCamera.transform.position.y;
        float z2 = m_TargetCamera.transform.position.z;


        return Mathf.Sqrt(Mathf.Pow(x1 - x2, 2) + Mathf.Pow(y1 - y2, 2) + Mathf.Pow(z1 - z2, 2));
        //return (transform.position - m_TargetCamera.transform.position).magnitude;

    }

    private void LateUpdate()
    {
        transform.localScale = Vector3.one * _baseScale * GetDistance();
        Debug.Log(this.name + "[4]" + transform.localScale);
    }
}