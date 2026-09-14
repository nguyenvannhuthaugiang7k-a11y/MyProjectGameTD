using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("Tham chiếu UI")]
    [SerializeField] private Image fillImage; // Ảnh Fill đại diện cho máu (thường màu đỏ/xanh)
    [SerializeField] private Canvas canvas;

    private Transform mainCameraTransform;

    private void Awake()
    {
        if (Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }

        if (canvas != null)
        {
            canvas.worldCamera = Camera.main;
        }
    }

    private void LateUpdate()
    {
        // Kỹ thuật Billboard: Xoay thanh máu luôn đối diện trực tiếp với Camera
        if (mainCameraTransform != null)
        {
            transform.rotation = mainCameraTransform.rotation;
        }
    }

    /// <summary>
    /// Cập nhật tỉ lệ thanh máu (từ 0.0 đến 1.0)
    /// </summary>
    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (fillImage != null && maxHealth > 0)
        {
            fillImage.fillAmount = Mathf.Clamp01(currentHealth / maxHealth);
        }
    }

    /// <summary>
    /// Bật/Tắt thanh máu khi cần
    /// </summary>
    public void SetVisibility(bool isVisible)
    {
        if (canvas != null)
        {
            canvas.enabled = isVisible;
        }
    }
}