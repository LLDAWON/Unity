using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class PlayerSkillAttack : MonoBehaviour
{
    [SerializeField]
    private Vector2 _attackOffset;
    [SerializeField]
    private float _skillRange;
    
    private bool _isAttack = false;
    private bool _isHIt = false;

    private Animator _animator; 
    private GameObject _attackInstance;
    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider;
    private PlayerAttack _playerAttack;
    private PlayerControl _playerControl;
    private Collider2D[] _skillBox;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _playerAttack = GetComponent<PlayerAttack>();
        _collider = GetComponent<Collider2D>();
        _playerControl = GetComponent<PlayerControl>();
        GameObject skillPrefab = Resources.Load<GameObject>("Prefabs/SkillEffect");
        _attackInstance = Instantiate(skillPrefab, transform);
        
    }

    private void Update()
    {
        Skill();
        
    }

    public void Skill()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            int count = _playerAttack.ParryCount;

            if (count > 0)
            {
                _skillBox = Physics2D.OverlapBoxAll(transform.position, new Vector2(_skillRange, 10), 0);

                foreach (Collider2D collider in _skillBox)
                {
                    if (collider.CompareTag("Monster"))
                    {
                        _animator.SetBool("Hit", true);
                        _isHIt = true;
                        _playerControl.SendDamage(_playerControl.GetSkillDamage());
                    }
                    
                }

                if(!_isHIt)
                {
                    _animator.SetTrigger("SkillStart");
                }


                    _isAttack = true;

                UIManager.Instance.PlayerMP(count-1, false); 
                _playerAttack.SetParryCount(count - 1);
            }
            return;

        }

        if (_isAttack)
        {
            if(_spriteRenderer.flipX)
            {
                transform.Translate(Vector2.left  * 15.0f * Time.deltaTime);
            }
            else if(!_spriteRenderer.flipX) 
            {
                transform.Translate(Vector2.right * 15.0f * Time.deltaTime);
            }
        }

    }
    private void SetSkillEffect()
    {
        _attackInstance.SetActive(true);
        Vector3 attackPosition = transform.position + new Vector3(_spriteRenderer.flipX ? _attackOffset.x : -_attackOffset.x, _attackOffset.y, 0);
        _attackInstance.transform.position = attackPosition;
        _attackInstance.GetComponent<SpriteRenderer>().flipX = _spriteRenderer.flipX;

        if(_isHIt)
        {
            _attackInstance.GetComponent<Animator>().SetTrigger("Hit");
        }
    }

    private void SkillEnd()
    {
        _animator.SetBool("Hit", false);
        _attackInstance.SetActive(false);
        _isAttack = false;
        _isHIt = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawCube(transform.position, new Vector3(_skillRange,0.1f, 0));
    }
}
