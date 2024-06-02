using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackControl : MonoBehaviour
{
    [SerializeField]
    protected Vector2 _attackOffset;

    protected bool _isAttack = false;

    protected Animator _animator;
    protected SpriteRenderer _spriteRenderer;
    protected GameObject _attackInstance;

    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    protected void SetSkillEffect()
    {
        _attackInstance.SetActive(true);
        Vector3 attackPosition = transform.position + new Vector3(_spriteRenderer.flipX ? _attackOffset.x : -_attackOffset.x, _attackOffset.y, 0);
        _attackInstance.transform.position = attackPosition;
        _attackInstance.GetComponent<SpriteRenderer>().flipX = _spriteRenderer.flipX;
    }
}
