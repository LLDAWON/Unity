using System.Collections;
using UnityEditor.U2D.Animation;
using UnityEngine;
using UnityEngine.UI;

public class MonsterControl : CharacterControl
{
    [SerializeField]
    private float _traceRange = 5.0f;          // ���� ����
    [SerializeField]
    private float _attackRange = 1.0f;         // ���� ����
    [SerializeField]
    private Vector2 _patrolTimeRange = new Vector2(1.0f, 3.0f);  // ���� �ð� ����
    [SerializeField]
    private Vector2 _stayIntervalRange = new Vector2(1.0f, 3.0f); // ���� ���� ����
    [SerializeField]
    private float _height = 1.5f;              // HP�� ����
    [SerializeField]
    private Vector3 _offset;                  // ���� ������Ʈ ��ġ ������

    private enum MState
    {
        PATROL,  // ���� ����
        TRACE,   // ���� ����
        ATTACK,  // ���� ����
        HURT,    // �ǰ� ����
        RETURN   // ���� �������� ���ư��� ���� �߰�
    }

    private MState _curState;                 // ���� ���� ����
    private SpawnData _spawnData;

    private bool _isRuturn = false;
    private bool _isTurn = false;             // ���� �� �Ͻ� ���� ����    
    private bool _isAttack = false;           // ���� �� ����

    public bool _isParrying = false;

    private float _attackTime = 0.0f;         // ���� Ÿ�̸�

    private PlayerControl _playerControl;      // �÷��̾� ��Ʈ�� ��ũ��Ʈ ����
    private GameObject _attackObject;         // ���� ���� ������Ʈ
    private GameObject _hpBar;                // HP �� ������Ʈ

    protected override void Awake()
    {
        base.Awake();
        InitializeMonster();
    }

    private void InitializeMonster()
    {
        GameObject attackPrefab = Resources.Load<GameObject>("Prefabs/Attack");
        _attackObject = Instantiate(attackPrefab);
        _attackObject.SetActive(false);
    }

    private void Start()
    {
        _playerControl = GameManager.Instance.Player;
        StartCoroutine(SetActionState());
        StartCoroutine(ActionAI());

        GameObject hpBarPrefab = Resources.Load<GameObject>("Prefabs/M_BG_HP");
        _hpBar = Instantiate(hpBarPrefab, UIManager.Instance.MonsterHpPanel);
    }

    protected override void Update()
    {
        UpdateHpBarPosition();

        if (_velocity.x > 0)
        {
            _spriteRenderer.flipX = true;
        }
        else if (_velocity.x < 0)
        {
            _spriteRenderer.flipX = false;
        }

        _attackObject.GetComponent<SpriteRenderer>().flipX = _spriteRenderer.flipX;

        if (_curHP <= 0)
        {
            gameObject.SetActive(false);
            _hpBar.SetActive(false);
        }
    }

    private void UpdateHpBarPosition()
    {
        Vector3 hpBarPos = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + _height, transform.position.z));
        _hpBar.transform.position = hpBarPos;
        _hpBar.GetComponent<Slider>().value = (_curHP/_characterData.MaxHp);
    }

    private void FixedUpdate()
    {
        base.Move();

        CheckParry();
    }

    private IEnumerator ActionAI()
    {
        while (true)
        {
            switch (_curState)
            {
                case MState.PATROL:
                    Patrol();
                    break;
                case MState.TRACE:
                    Trace();
                    break;
                case MState.ATTACK:
                    ExecuteAttack();
                    break;
                case MState.RETURN:
                    ReturnToStart();
                    break;
            }
            yield return null;
        }
    }

    private IEnumerator SetActionState()
    {
        while (true)
        {
            if (_curState == MState.HURT)
                continue;

            Vector2 targetPos = _playerControl.transform.position;
            float distance = Vector2.Distance(transform.position, targetPos);

            if (distance < _attackRange)
            {
                _curState = MState.ATTACK;
            }
            else if (distance < _traceRange)
            {
                _curState = MState.TRACE;
            }
            else
            {
                if (_curState == MState.TRACE)
                {
                    _curState = MState.RETURN; // TRACE ���¿��� ������ ����� RETURN ���·� ��ȯ
                }
                else if (_curState != MState.PATROL)
                {
                    _curState = MState.PATROL;
                }
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator PatrolStay()
    {
        _isTurn = true;
        _velocity.x = 0.0f;
        _animator.SetFloat("Speed", 0.0f);

        float stayTime = Random.Range(_patrolTimeRange.x, _patrolTimeRange.y);

        yield return new WaitForSeconds(stayTime);

        Turn();
    }

    private void Patrol()
    {
        if (_isTurn) return;

        Vector2 direction;

        if(_isRuturn)
        {
            direction = _spawnData.startPos - (Vector2)transform.position;            
        }
        else
        {
            direction = _spawnData.destPos - (Vector2)transform.position;
        }
        _velocity.x = _characterData.WalkSpeed;
        _velocity.x *= (direction.x > 0) ? 1 : -1;

        _animator.SetFloat("Speed", Mathf.Abs(_velocity.x));

        if(Mathf.Abs(direction.x) < 0.1f)
        {
            StartCoroutine(PatrolStay());
        }
    }
    
    private void Turn()
    {
        _isRuturn = !_isRuturn;        

        _animator.SetTrigger("Turn");
    }

    private void EndTurn()
    {
        _isTurn = false;
        _animator.SetTrigger("EndTurn");
    }

    private void Trace()
    {
        _animator.SetTrigger("EndTurn");
        Vector2 direction = _playerControl.transform.position - transform.position;
        _velocity.x = (direction.x > 0.0f) ? _characterData.RunSpeed : -_characterData.RunSpeed;
        _animator.SetFloat("Speed", _characterData.RunSpeed);
    }

    private void ReturnToStart()
    {
        Vector2 direction = _spawnData.startPos - (Vector2)transform.position;
        float distance = direction.magnitude;
    
        if (distance > 0.1f)
        {
            _velocity.x = (direction.x > 0.0f) ? _characterData.RunSpeed : -_characterData.RunSpeed;
            _animator.SetFloat("Speed", _characterData.RunSpeed);
        }
        else
        {
            _curState = MState.PATROL;
        }
    }

    private void CheckParry()
    {
        if (_playerControl.IsParry() && _isAttack)
        {
            EndAttack();
            _animator.SetTrigger("ParrySuccess");
            _isParrying = true;            
        }
    }

    private void ExecuteAttack()
    {
        if (!_playerControl.gameObject.activeSelf)
            _curState =  MState.PATROL;

        _velocity.x = 0.0f;
        _animator.SetFloat("Speed", _velocity.x);
        if (_isAttack) return;

        _attackTime += Time.deltaTime;

        if (_attackTime >= 3.0f)
        {
            _attackTime = 0.0f;
            StartAttack();            
        }
    }

    private void StartAttack()
    {
        _isAttack = true;
        _animator.SetBool("BasicAttack", _isAttack);       
    }

    private void EndAttack()
    {
        _isAttack = false;
        _animator.SetBool("BasicAttack", _isAttack);
        _attackObject.SetActive(false);
    }

    private void ActivateAttackEffect()
    {
        _attackObject.SetActive(true);
        Vector3 attackPosition = transform.position + new Vector3(_spriteRenderer.flipX ? _offset.x : -_offset.x, _offset.y, 0);
        _attackObject.transform.position = attackPosition;
        Hit();
    }

    private void DeactivateAttackEffect()
    {
        _attackObject.SetActive(false);
        _attackObject.GetComponent<MonsterAttackEffect>().SetHit(false);
    }

    private void EndHit()
    {
        _animator.SetTrigger("HurtEnd");
        _isParrying = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _traceRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }

    public void Spawn(SpawnData spawnData)
    {
        _spawnData = spawnData;

        transform.position = spawnData.startPos;
        gameObject.SetActive(true);
    }

    public void Hit()
    {
        if (!_isParrying)
        {
            Vector2 pos = _collider2D.bounds.center;
            if (_spriteRenderer.flipX)
                pos += _attackOffset;
            else
                pos -= _attackOffset;


            Collider2D[] collider = Physics2D.OverlapBoxAll(pos, _attackSize, 0);

            foreach (Collider2D collider2d in collider)
            {
                if (collider2d.CompareTag("Player"))
                {
                    _playerControl.Hurt(_characterData.Power);
                }
            }   
            
        }
    }
}
