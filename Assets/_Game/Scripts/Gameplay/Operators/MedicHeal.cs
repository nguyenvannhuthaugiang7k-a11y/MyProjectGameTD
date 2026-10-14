using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MedicHeal : MonoBehaviour
{
    [Header("Heal")]
    [SerializeField] private float healAmount = 20f;
    [SerializeField] private float healInterval = 1f;

    [Header("Tầm Heal")]
    [SerializeField] private float healRange = 3f;

    private float healTimer = 0f;

    private void Update()
    {
        healTimer += Time.deltaTime;

        if (healTimer < healInterval)
            return;

        OperatorHealth target = FindLowestHealthOperator();

        if (target != null)
        {
            Heal(target);
            healTimer = 0f;
        }
    }

    private OperatorHealth FindLowestHealthOperator()
    {
        OperatorHealth[] operators =
            FindObjectsByType<OperatorHealth>(FindObjectsSortMode.None);

        List<OperatorHealth> validTargets = new List<OperatorHealth>();

        foreach (OperatorHealth op in operators)
        {
            if (op == null)
                continue;

            if (op.gameObject == gameObject)
                continue;

            float distance =
                Vector3.Distance(transform.position, op.transform.position);

            if (distance > healRange)
                continue;

            if (op.CurrentHealth >= op.MaxHealth)
                continue;

            validTargets.Add(op);
        }

        return validTargets
            .OrderBy(op => op.CurrentHealth / op.MaxHealth)
            .FirstOrDefault();
    }

    private void Heal(OperatorHealth target)
    {
        target.Heal(healAmount);

        Debug.Log(
            $"[MedicHeal] {gameObject.name} hồi {healAmount} HP cho " +
            $"{target.gameObject.name}."
        );
    }
}