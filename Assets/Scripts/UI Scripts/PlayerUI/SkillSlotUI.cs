using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SkillSlotState
{
    Learned,
    Available,
    Locked
}
public class SkillSlotUI : MonoBehaviour
{
    [SerializeField] private Skill storedSkill;
    [SerializeField] private Image icon;
    [SerializeField] private SkillSlotState state;
    [SerializeField] private Image border;
    [SerializeField] private Image container;

    [Header("Materials")]
    [SerializeField] private Material learnedeMaterial;
    [SerializeField] private Material availableMaterial;
    [SerializeField] private Material lockedMaterial;

    [SerializeField] public bool isSelected;

    // Start is called before the first frame update
    private void Start()
    {
        icon.sprite = storedSkill.icon;
        container = GetComponent<Image>();
    }

    private void Update()
    {
        border.enabled = isSelected;
    }

    public Skill getSkill()
    {
        return storedSkill;
    }

    public bool isAvailable() => state == SkillSlotState.Available;

    public void setState(SkillSlotState newState)
    {
        state = newState;
        switch (state)
        {
            case SkillSlotState.Learned:
                container.color = Color.yellow;
                icon.material = learnedeMaterial;
                break;
            case SkillSlotState.Available:
                container.color = Color.white;
                icon.material = availableMaterial;
                break;
            case SkillSlotState.Locked:
                container.color = Color.gray;
                icon.material = lockedMaterial;
                break;
        }
    }
}
