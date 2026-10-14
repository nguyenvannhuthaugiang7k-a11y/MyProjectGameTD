using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SquadOperatorCardUI : MonoBehaviour
{
    [Header("Operator Data")]
    [SerializeField] private OperatorData data;

    [Header("UI")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI dpCostText;
    [SerializeField] private TextMeshProUGUI classText;
    [SerializeField] private GameObject selectedOverlay;
    [SerializeField] private GameObject selectedBorder;

    public OperatorData Data => data;

    private void Start()
    {
        SetupUI();
        RefreshVisual();
    }

    public void Setup(OperatorData operatorData)
    {
        data = operatorData;

        SetupUI();
        RefreshVisual();
    }

    private void SetupUI()
    {
        if (data == null)
            return;

        // Icon
        if (iconImage != null)
            iconImage.sprite = data.icon;

        // Operator Name
        if (nameText != null)
            nameText.text = data.operatorName;

        // DP Cost
        if (dpCostText != null)
            dpCostText.text = data.dpCost.ToString();

        // Operator Class
        if (classText != null)
            classText.text = data.operatorClass.ToString().ToUpper();
    }

    public void OnClick()
    {
        if (data == null)
            return;

        if (SquadManager.Instance == null)
        {
            Debug.LogError(
                "[SquadOperatorCardUI] Không tìm thấy SquadManager!"
            );

            return;
        }

        if (SquadManager.Instance.IsSelected(data))
        {
            SquadManager.Instance.RemoveOperator(data);
        }
        else
        {
            SquadManager.Instance.AddOperator(data);
        }

        RefreshVisual();
    }

    private void RefreshVisual()
    {
        if (SquadManager.Instance == null || data == null)
            return;

        bool isSelected =
            SquadManager.Instance.IsSelected(data);

        if (selectedOverlay != null)
            selectedOverlay.SetActive(isSelected);

        if (selectedBorder != null)
            selectedBorder.SetActive(isSelected);
    }
}