using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class OperatorCardUI : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Dữ liệu Nhân vật")]
    public OperatorData data;

    [Header("Tham chiếu UI Component")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI dpCostText;
    [SerializeField] private GameObject disabledOverlay;
    [SerializeField] private Image cooldownOverlayImage;
    [SerializeField] private Image cardBorder;
    [SerializeField] private GameObject selectedOverlay;

    private float currentCooldown = 0f;

    // Card có thể sử dụng để chọn/deploy
    private bool isReady = true;

    // Operator hiện đang ở trên sân
    private bool isDeployed = false;

    private void Start()
    {
        SetupCardUI();
    }

    public void Setup(OperatorData operatorData)
    {
        data = operatorData;

        currentCooldown = 0f;
        isReady = true;
        isDeployed = false;

        if (cooldownOverlayImage != null)
            cooldownOverlayImage.fillAmount = 0f;

        if (selectedOverlay != null)
            selectedOverlay.SetActive(false);

        SetupCardUI();
    }

    public void SetupCardUI()
    {
        if (data == null)
            return;

        if (iconImage != null)
            iconImage.sprite = data.icon;

        if (dpCostText != null)
            dpCostText.text = data.dpCost.ToString();
    }

    private void Update()
    {
        HandleCooldown();
        UpdateCardAvailability();
    }

    private void HandleCooldown()
    {
        if (currentCooldown > 0f)
        {
            currentCooldown -= Time.deltaTime;

            if (cooldownOverlayImage != null &&
                data != null &&
                data.redeployCooldown > 0f)
            {
                cooldownOverlayImage.fillAmount =
                    currentCooldown / data.redeployCooldown;
            }

            // Cooldown hoàn thành
            if (currentCooldown <= 0f)
            {
                currentCooldown = 0f;
                isReady = true;
                isDeployed = false;

                if (cooldownOverlayImage != null)
                    cooldownOverlayImage.fillAmount = 0f;

                if (selectedOverlay != null)
                    selectedOverlay.SetActive(false);

                Debug.Log($"[OperatorCardUI] {data.operatorName} đã sẵn sàng deploy lại.");
            }
        }
    }

    private void UpdateCardAvailability()
    {
        if (DPManager.Instance == null || data == null)
            return;

        bool hasEnoughDP =
            DPManager.Instance.HasEnoughDP(data.dpCost);

        // Operator đang trên sân → khóa hoàn toàn
        if (isDeployed)
        {
            if (disabledOverlay != null)
                disabledOverlay.SetActive(true);

            return;
        }

        bool canUse =
            hasEnoughDP && isReady;

        if (disabledOverlay != null)
            disabledOverlay.SetActive(!canUse);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (data == null)
            return;

        if (PlacementManager.Instance == null)
            return;

        // ==========================================
        // OPERATOR ĐANG Ở TRÊN SÂN
        // ==========================================
        // Không cho click để deploy lần 2.
        if (isDeployed)
        {
            Debug.Log($"[OperatorCardUI] {data.operatorName} đang ở trên sân.");
            return;
        }

        // ==========================================
        // CARD ĐANG ĐƯỢC CHỌN
        // ==========================================
        // Click lần 2 để bỏ chọn.
        if (PlacementManager.Instance.IsOperatorSelected(this))
        {
            PlacementManager.Instance.CancelOperatorSelection();

            if (selectedOverlay != null)
                selectedOverlay.SetActive(false);

            Debug.Log($"[OperatorCardUI] Đã bỏ chọn: {data.operatorName}");

            return;
        }

        // ==========================================
        // ĐANG COOLDOWN
        // ==========================================
        if (!isReady)
        {
            Debug.Log($"[OperatorCardUI] {data.operatorName} đang cooldown.");
            return;
        }

        // ==========================================
        // KIỂM TRA DP
        // ==========================================
        if (DPManager.Instance == null)
            return;

        if (!DPManager.Instance.HasEnoughDP(data.dpCost))
        {
            Debug.Log($"[OperatorCardUI] Không đủ DP để deploy {data.operatorName}.");
            return;
        }

        // ==========================================
        // CHỌN OPERATOR ĐỂ DEPLOY
        // ==========================================
        if (!PlacementManager.Instance.TrySelectOperator(this))
        {
            return;
        }

        SetSelected(true);

        Debug.Log($"[OperatorCardUI] Đã chọn: {data.operatorName}");
    }

    // =========================================================
    // GỌI KHI OPERATOR DEPLOY THÀNH CÔNG
    // =========================================================
    public void SetDeployed()
    {
        isDeployed = true;
        isReady = false;

        // Giữ card ở trạng thái mờ
        if (selectedOverlay != null)
            selectedOverlay.SetActive(true);

        // Không hiển thị vòng cooldown khi Operator vẫn còn trên sân
        if (cooldownOverlayImage != null)
            cooldownOverlayImage.fillAmount = 0f;

        Debug.Log($"[OperatorCardUI] {data.operatorName} đã deployed và bị khóa.");
    }

    // =========================================================
    // GỌI KHI OPERATOR RETREAT
    // =========================================================
    public void StartCooldown()
    {
        if (data == null)
            return;

        isDeployed = false;
        isReady = false;

        currentCooldown = data.redeployCooldown;

        // Tắt trạng thái selected
        if (selectedOverlay != null)
            selectedOverlay.SetActive(false);

        // Bắt đầu vòng cooldown
        if (cooldownOverlayImage != null)
            cooldownOverlayImage.fillAmount = 1f;

        Debug.Log(
            $"[OperatorCardUI] {data.operatorName} bắt đầu cooldown: {data.redeployCooldown}s"
        );
    }

    public void SetSelected(bool selected)
    {
        if (selectedOverlay != null)
            selectedOverlay.SetActive(selected);
    }

    // =========================================================
    // HOVER
    // =========================================================
    // Hiện tại KHÔNG làm thay đổi độ mờ card.
    public void OnPointerEnter(PointerEventData eventData)
    {
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }
}