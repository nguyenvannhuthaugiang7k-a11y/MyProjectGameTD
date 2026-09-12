using UnityEngine;
using TMPro; // Sử dụng TextMeshPro cho UI chữ chuẩn đẹp

public class DPUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dpText; // Tham chiếu đến Text hiển thị DP

    private void OnEnable()
    {
        // Đăng ký nhận sự kiện khi DP thay đổi
        if (DPManager.Instance != null)
        {
            DPManager.Instance.OnDPChanged += UpdateDPText;
        }
    }

    private void Start()
    {
        // Lắng nghe sự kiện nếu DPManager khởi tạo muộn
        if (DPManager.Instance != null)
        {
            DPManager.Instance.OnDPChanged += UpdateDPText;
            UpdateDPText(DPManager.Instance.GetCurrentDP());
        }
    }

    private void OnDisable()
    {
        // Hủy đăng ký sự kiện khi ẩn/xóa UI để tránh rò rỉ bộ nhớ (Memory Leak)
        if (DPManager.Instance != null)
        {
            DPManager.Instance.OnDPChanged -= UpdateDPText;
        }
    }

    // Cập nhật chuỗi chữ hiển thị DP
    private void UpdateDPText(int currentDP)
    {
        if (dpText != null)
        {
            dpText.text = $"DP: {currentDP}";
        }
    }
}