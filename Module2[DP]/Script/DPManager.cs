using UnityEngine;
using System; // Dùng cho Event / Action cập nhật UI

public class DPManager : MonoBehaviour
{
    public static DPManager Instance { get; private set; }

    [Header("DP Settings")]
    [SerializeField] private int maxDP = 99;            // DP tối đa có thể tích lũy
    [SerializeField] private int startingDP = 10;        // Số DP ban đầu khi bắt đầu màn chơi
    [SerializeField] private float secondsPerDP = 1f;    // Bao nhiêu giây thì hồi 1 DP

    private float currentDP;
    private float timer;

    // Sự kiện thông báo khi DP thay đổi (giúp UI lắng nghe và cập nhật)
    public event Action<int> OnDPChanged;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        currentDP = startingDP;
        // Báo cho UI biết số DP khởi đầu
        OnDPChanged?.Invoke(Mathf.FloorToInt(currentDP));
    }

    private void Update()
    {
        HandleDPRecovery();
    }

    // Tự động hồi DP theo thời gian
    private void HandleDPRecovery()
    {
        if (currentDP < maxDP)
        {
            timer += Time.deltaTime;
            if (timer >= secondsPerDP)
            {
                timer -= secondsPerDP; // Giữ lại phần dư nếu DeltaTime cao
                AddDP(1);
            }
        }
    }

    // Hàm thêm DP (Dùng cho cả tự hồi và khi dùng Skill hồi DP của nhân vật như Vanguard)
    public void AddDP(int amount)
    {
        currentDP = Mathf.Clamp(currentDP + amount, 0, maxDP);
        OnDPChanged?.Invoke(Mathf.FloorToInt(currentDP));
    }

    // Kiểm tra xem có đủ DP để dùng không
    public bool HasEnoughDP(int amount)
    {
        return currentDP >= amount;
    }

    // Hàm tiêu tốn DP khi thả nhân vật
    public bool ConsumeDP(int amount)
    {
        if (HasEnoughDP(amount))
        {
            currentDP -= amount;
            OnDPChanged?.Invoke(Mathf.FloorToInt(currentDP));
            return true;
        }

        Debug.Log("Không đủ DP để thực hiện thao tác!");
        return false;
    }

    // Lấy số DP hiện tại (làm tròn xuống số nguyên)
    public int GetCurrentDP()
    {
        return Mathf.FloorToInt(currentDP);
    }
}