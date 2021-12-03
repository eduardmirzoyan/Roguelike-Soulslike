using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Skill : ScriptableObject
{
    [SerializeField] public new string name;
    [TextArea(15, 20)]
    [SerializeField] public string description;

    [SerializeField] public Skill prerequisite;
    [SerializeField] public Sprite icon;
}
