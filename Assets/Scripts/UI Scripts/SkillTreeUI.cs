using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillTreeUI : MonoBehaviour
{
    [Header("Non visual Values")]
    [SerializeField] private Player player;
    [SerializeField] private List<Skill> allSkills;
    [SerializeField] private List<Skill> playerSkills;

    [Header("Visual Objects")]
    public Transform itemsParent;
    private int selectedSlotRow;
    private int selectedSlotColumn;

    [SerializeField] private SkillSlotUI[][] slots; // an array of arrays of slots
    

    [SerializeField] private Transform[] skillTreeLevel; // The number of levels
    private int currentTreeLevel;

    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        playerSkills = player.getPlayerSkills();
        slots = new SkillSlotUI[skillTreeLevel.Length][];
        for(int i = 0; i < skillTreeLevel.Length; i++)
        {
            slots[i] = skillTreeLevel[i].GetComponentsInChildren<SkillSlotUI>();
        }
        
        selectedSlotRow = 0;
        selectedSlotColumn = 0;
        slots[selectedSlotRow][selectedSlotColumn].isSelected = true;
    }

    public void moveSelectedItem(string direction)
    {
        switch (direction)
        {
            case "left":
                if (selectedSlotColumn % slots[selectedSlotRow].Length == 0)
                    break;
                slots[selectedSlotRow][selectedSlotColumn].isSelected = false;
                selectedSlotColumn -= 1;
                slots[selectedSlotRow][selectedSlotColumn].isSelected = true;
                break;
            case "right":
                if ((selectedSlotColumn + 1) % slots[selectedSlotRow].Length == 0)
                        break;
                slots[selectedSlotRow][selectedSlotColumn].isSelected = false;
                selectedSlotColumn += 1;
                slots[selectedSlotRow][selectedSlotColumn].isSelected = true;
                break;
            case "up":
                if (selectedSlotRow - 1 < 0)
                    break;
                slots[selectedSlotRow][selectedSlotColumn].isSelected = false;
                if (selectedSlotColumn > slots[selectedSlotRow - 1].Length - 1)
                {
                    selectedSlotColumn = slots[selectedSlotRow - 1].Length - 1;
                }
                selectedSlotRow -= 1;
                slots[selectedSlotRow][selectedSlotColumn].isSelected = true;
                break;
            case "down":
                if (selectedSlotRow + 1 >= slots.Length)
                    break;
                slots[selectedSlotRow][selectedSlotColumn].isSelected = false;
                if (selectedSlotColumn > slots[selectedSlotRow + 1].Length - 1)
                {
                    selectedSlotColumn = slots[selectedSlotRow + 1].Length - 1;
                }
                selectedSlotRow += 1;
                slots[selectedSlotRow][selectedSlotColumn].isSelected = true;
                break;
        }
    }

    public void updateSkillVisuals()
    {
        for (int i = 0; i != slots.Length; i++)
            for (int j = 0; j != slots[i].Length; j++)
            {
                slots[i][j].setState(getStateOfSkillSlot(i, j));
            }
    }

    private SkillSlotState getStateOfSkillSlot(int row, int col)
    {
        Skill skillToCheck = slots[row][col].getSkill();
        if (playerSkills.Any(skill => skill.name == skillToCheck.name)) // If the player has the skill, then return learned
        {
            return SkillSlotState.Learned;
        }
        else
        {
            // Check to see if player the skill tree level unlocked


            // Check to see if player has the prereq skill(s)
            // If playerskills has any skill that matches the name of the skillToAdd's prereq, then it satisfies prereq
            if (skillToCheck.prerequisite == null || playerSkills.Any(skill => skill.name == skillToCheck.prerequisite.name)) // Use all if more than 1 prereq
            {
                // True!
                if (GameManager.instance.skillpoints > 0) // Change this check when you implement multiple skill point costs
                {
                    // Then the skill is ready to be learned
                    return SkillSlotState.Available;
                    //GameManager.instance.skillpoints--;
                }
                else
                {
                    //Debug.Log("You do not have a skill point.");
                    //GameManager.instance.CreatePopup("You do not have enough skill points.", player.gameObject.transform.position);
                    return SkillSlotState.Locked;
                }
            }
            else
            {
                return SkillSlotState.Locked;
                //GameManager.instance.CreatePopup("You do not have the prerequisite skill for " + skillToCheck.name + " called: " + skillToCheck.prerequisite.name, player.gameObject.transform.position);
                //Debug.Log("You do not have the prerequisite skill for " + skillToAdd.name + " called: " + skillToAdd.prerequisite.name);
            }
        }

        //return SkillSlotState.Available;
    }

    public void attemptToUnlockSkillAtSelectedSlot()
    {
        if (slots[selectedSlotRow][selectedSlotColumn].isAvailable())
        {
            Skill skillToAdd = slots[selectedSlotRow][selectedSlotColumn].getSkill();
            player.learnSkill(skillToAdd); // Also make sure the player knows...
            GameManager.instance.skillpoints--;
        }
        

        /*Skill skillToAdd = slots[selectedSlotRow][selectedSlotColumn].getSkill();
        if (playerSkills.Any(skill => skill.name == skillToAdd.name)) // If the skill you want to add is in player...
        {
            // DO nothing
            GameManager.instance.CreatePopup("You already know the skill: " + skillToAdd.name, player.gameObject.transform.position);
            return;
        }
        else
        {
            // Check to see if player the skill tree level unlocked


            // Check to see if player has the prereq skill(s)
            // If playerskills has any skill that matches the name of the skillToAdd's prereq, then it satisfies prereq
            if (skillToAdd.prerequisite == null || playerSkills.Any(skill => skill.name == skillToAdd.prerequisite.name)) // Use all if more than 1 prereq
            {
                // True!
                if (GameManager.instance.skillpoints > 0)
                {
                    // True!
                    player.learnSkill(skillToAdd); // Also make sure the player knows...
                    GameManager.instance.skillpoints--;
                }
                else
                {
                    //Debug.Log("You do not have a skill point.");
                    GameManager.instance.CreatePopup("You do not have enough skill points.", player.gameObject.transform.position);
                }
            }
            else
            {
                GameManager.instance.CreatePopup("You do not have the prerequisite skill for " + skillToAdd.name + " called: " + skillToAdd.prerequisite.name, player.gameObject.transform.position);
                //Debug.Log("You do not have the prerequisite skill for " + skillToAdd.name + " called: " + skillToAdd.prerequisite.name);
            }
        }*/

    }

    public Transform getSelectedSlotPosition()
    {
        return slots[selectedSlotRow][selectedSlotColumn].transform;
    }

    public Skill getSelectedSkill()
    {
        return slots[selectedSlotRow][selectedSlotColumn].getSkill();
    }
}
