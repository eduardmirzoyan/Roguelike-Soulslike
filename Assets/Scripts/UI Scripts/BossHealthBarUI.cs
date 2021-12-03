using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBarUI : MonoBehaviour
{
    [SerializeField] public Slider slider;
    [SerializeField] private EnemyAI boss;

    private void FixedUpdate()
    {
        if(boss != null)
        {
            slider.maxValue = boss.GetComponent<Health>().getMaxHP();
            slider.value = boss.GetComponent<Health>().getHP();
        }
    }

    public void setBoss(EnemyAI boss)
    {
        this.boss = boss;
    }
}
