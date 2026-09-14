using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class OperatorCardUI : MonoBehaviour, IPointerDownHandler
{
    [Header("Dữ liệu Nhân vật")]
    public OperatorData data;

    [Header("Tham chiếu UI Component")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI dpCostText;
    [SerializeField] private GameObject disabledOverlay;
    [SerializeField] private Image cooldownOverlayImage;

    private float currentCooldown = 0f;
    private bool isReady = true;

    private void Start()
    {
        SetupCardUI();
    }

    public void SetupCardUI()
    {
        if (data == null) return;

        if (iconImage != null) iconImage.sprite = data.icon;
        if (dpCostText != null) dpCostText.text = data.dpCost.ToString();
    }

    private void Update()
    {
        HandleCooldown();
        UpdateCardAvailability();
    }

    private void HandleCooldown()
    {
        if (currentCooldown > 0)
        {
            currentCooldown -= Time.deltaTime;

            if (cooldownOverlayImage != null && data != null)
                cooldownOverlayImage.fillAmount =
                    currentCooldown / data.redeployCooldown;

            if (currentCooldown <= 0)
            {
                currentCooldown = 0f;
                isReady = true;

                if (cooldownOverlayImage != null)
                    cooldownOverlayImage.fillAmount = 0f;
            }
        }
    }

    private void UpdateCardAvailability()
    {
        if (DPManager.Instance == null || data == null) return;

        bool hasEnoughDP = DPManager.Instance.HasEnoughDP(data.dpCost);
        bool canUse = hasEnoughDP && isReady;

        if (disabledOverlay != null)
            disabledOverlay.SetActive(!canUse);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isReady) return;
        if (data == null) return;
        if (PlacementManager.Instance == null) return;

        if (DPManager.Instance.HasEnoughDP(data.dpCost))
        {
            PlacementManager.Instance.SelectOperatorToPlace(data, this);
        }
    }

    public void StartCooldown()
    {
        isReady = false;
        currentCooldown = data.redeployCooldown;
        if (cooldownOverlayImage != null) cooldownOverlayImage.fillAmount = 1f;
    }
}