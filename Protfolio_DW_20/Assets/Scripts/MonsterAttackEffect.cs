using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Animation;
using UnityEngine;

public class MonsterAttackEffect : MonoBehaviour
{
    public bool IsHit = false;

    private float _damage;

    [SerializeField]
    private MonsterControl _monsterControl;

    public void SetHit(bool hit)
    {
        IsHit = hit;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {       
        if (collision.CompareTag("Player"))
        {
            if (IsHit) return;

            if (!_monsterControl._isParrying)
            {
                //_monsterControl.GetDamage();
                IsHit = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            IsHit = false;
        }

    }

}
