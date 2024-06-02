using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Animation;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField]
    private Vector2 _attackOffset;

    private Animator _animator;

    private bool _isAttack = false;            // 공격 중인지 여부
    private bool _isComboEnabled = false;      // 콤보 공격이 가능한지 여부
    private bool _isCombo = false;             // 콤보 공격 중인지 여부

    public bool IsParryEnabled = false;      // 패링이 가능한지 여부
    public bool IsParry = false;          // 패링 중인지 여부
    public int ParryCount = 0;

    private GameObject _attackInstance;
    private SpriteRenderer _spriteRenderer;
    private PlayerControl _playerControl;

    public void SetParryCount(int parryCount)
    { ParryCount = parryCount; }

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _playerControl = GetComponent<PlayerControl>();
        GameObject attackPrefab = Resources.Load<GameObject>("Prefabs/AttackEffect");
        
        _attackInstance = Instantiate(attackPrefab, transform);
    }

    private void Update()
    {
        HandleBasicAttack();
        HandleParry();
    }

    private void HandleBasicAttack()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Attack();
            _playerControl.SendDamage(_playerControl.GetDamage());
        }
    }

    private void Attack()
    {
        if (_isAttack)
        {
            if (_isComboEnabled)
            {
                _isCombo = true;
                _isComboEnabled = false;
            }
        }
        else
        {
            _animator.SetTrigger("BasicAttack");
            _isAttack = true;
            _isComboEnabled = false;
            _isCombo = false;
            SetAttackEffect();
        }
    }

    private void SetAttackEffect()
    {
        _attackInstance.SetActive(true);
        Vector3 attackPosition = transform.position + new Vector3(_spriteRenderer.flipX ? _attackOffset.x : -_attackOffset.x, _attackOffset.y, 0);
        _attackInstance.transform.position = attackPosition;
        _attackInstance.GetComponent<SpriteRenderer>().flipX = _spriteRenderer.flipX;
    }

    // 패링을 제어하는 메서드
    private void HandleParry()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _animator.SetTrigger("Parry");
        }
    }

    // 콤보 공격을 가능하게 설정
    private void EnableCombo()
    {
        _isComboEnabled = true;
    }

    // 콤보 공격을 불가능하게 설정
    private void DisableCombo()
    {
        _isComboEnabled = false;
    }

    // 콤보 공격을 제어하는 메서드
    private void HandleComboAttack()
    {
        if (_isCombo)
        {
            _animator.SetTrigger("ComboAttack");
            _attackInstance.GetComponent<Animator>().SetTrigger("PlayerComboB");
            _isComboEnabled = false;
            _isCombo = false;
        }
    }

    // 공격 종료 이벤트
    public void EndAttack()
    {
        _isAttack = false;
        _isComboEnabled = false;
        _attackInstance.SetActive(false);
    }

    // 패링을 가능하게 설정
    private void EnableParry()
    {
        IsParryEnabled = true;
    }

    // 패링을 불가능하게 설정
    private void DisableParry()
    {
        IsParryEnabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {      
        if (collision.CompareTag("MonsterAttack"))
        {
            if (IsParry) return;

            if (IsParryEnabled)
            {
                if (ParryCount > 3)
                { ParryCount = 3; }
                IsParry = true;
                UIManager.Instance.PlayerMP(ParryCount, true);

                ParryCount++;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("MonsterAttack"))
        {
            IsParry = false;
        }
    }

}
