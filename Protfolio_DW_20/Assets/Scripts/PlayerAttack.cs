using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Animation;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField]
    private Vector2 _attackOffset;

    private Animator _animator;

    private bool _isAttack = false;            // ���� ������ ����
    private bool _isComboEnabled = false;      // �޺� ������ �������� ����
    private bool _isCombo = false;             // �޺� ���� ������ ����

    public bool IsParryEnabled = false;      // �и��� �������� ����
    public bool IsParry = false;          // �и� ������ ����
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

    // �и��� �����ϴ� �޼���
    private void HandleParry()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _animator.SetTrigger("Parry");
        }
    }

    // �޺� ������ �����ϰ� ����
    private void EnableCombo()
    {
        _isComboEnabled = true;
    }

    // �޺� ������ �Ұ����ϰ� ����
    private void DisableCombo()
    {
        _isComboEnabled = false;
    }

    // �޺� ������ �����ϴ� �޼���
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

    // ���� ���� �̺�Ʈ
    public void EndAttack()
    {
        _isAttack = false;
        _isComboEnabled = false;
        _attackInstance.SetActive(false);
    }

    // �и��� �����ϰ� ����
    private void EnableParry()
    {
        IsParryEnabled = true;
    }

    // �и��� �Ұ����ϰ� ����
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
