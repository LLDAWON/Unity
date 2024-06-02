using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerControl : CharacterControl
{
    private int _smokeCount = 3;               // 남은 스모크 수
    private bool _isDead = false;
    private bool _isDown = false;
    private bool _isWall = false;
    private bool _isUp = false;

    private GameManager _gameManager;
    private GameObject _SmokeInstance;
    private PlayerAttack _playerAttack;
    private PlayerSkillAttack _playerSkillAttack;
    
    [SerializeField]
    private Vector2 _offset;

    protected override void Awake()
    {
        base.Awake();    
        UIManager.Instance.PlayerHPValue(_curHP, _characterData.MaxHp);

        _playerAttack = GetComponent<PlayerAttack>();
        _playerSkillAttack = GetComponent<PlayerSkillAttack>();

        GameObject smokePrefab = Resources.Load<GameObject>("Prefabs/PlayerSmoke");
        _SmokeInstance = Instantiate(smokePrefab, transform);
    }

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        //SpawnAtSavePoint();
    }

    public void SpawnAtSavePoint()
    {
        Vector3 spawnPosition = _gameManager.GetSavePoint();
        transform.position = spawnPosition;
        ReSpawn();
    }


    protected override void Update()
    {
        base.Update();
        HandleInput();
    }

    public void ReSpawn()
    {
        _curHP = _characterData.MaxHp;        
        gameObject.SetActive(true);
        UIManager.Instance.PlayerReSet();
    }

    private void HandleInput()
    {
        Down();
        HandleMovement();
        HandleDash();
        HandleJump();
        HandleSmoke();
        HandleDead();

        HandleWallMove();
        UIManager.Instance.PlayerHPValue(_curHP,_characterData.MaxHp);
    }

    // 플레이어의 이동을 제어하는 메서드
    private void HandleMovement()
    {
        if (_isWall) return;

        _velocity.x = Input.GetAxis("Horizontal");
        _moveSpeed = (_velocity.x != 0.0f) ? _characterData.WalkSpeed : 0.0f;
        _velocity *= _moveSpeed;
        _animator.SetFloat("Speed", Mathf.Abs(_velocity.x));

        if (_velocity.x < 0.0f)
            _spriteRenderer.flipX = true;
        else if (_velocity.x > 0.0f)
            _spriteRenderer.flipX = false;
    }

    // 대시를 제어하는 메서드
    private void HandleDash()
    {
        if (_isJump) return;

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _animator.SetTrigger("Dash");
            _isDash = true;
        }
    }

    // 점프를 제어하는 메서드
    private void HandleJump()
    {
        if (_isUp) return;
        
        if (Input.GetKeyDown(KeyCode.Space) && !_isJump)
        {
            _rigidbody2D.AddForce(Vector2.up * _characterData.JumpPower);
            _animator.SetBool("IsAir", true);
            _isJump = true;

            if(_isWall)
            {
                _animator.SetTrigger("WallJump");
                _isUp = false;
                _isWall = false;
                _rigidbody2D.gravityScale = 2;
            }

        }
    }

    // 스모크 사용을 제어하는 메서드
    private void HandleSmoke()
    {
        if (Input.GetKeyDown(KeyCode.Z) && _smokeCount > 0)
        {
            _animator.SetTrigger("Smoke");
            _smokeCount--;
            HandleHP(30);
            UIManager.Instance.PlayerSmoke(_smokeCount);
            SetSmokeEffect();
        }
    }

    private void HandleHP(float damage)
    {
        _curHP += damage;
        UIManager.Instance.PlayerHPValue(_curHP, _characterData.MaxHp);
    }

    private void HandleDead()
    {
        if(_isDead) return;

        if(_curHP <= 0)
        {
            _animator.SetTrigger("Dead");       
            _isDead = true;
        }
    }

    private void SetSmokeEffect()
    {
        _SmokeInstance.SetActive(true);
        Vector3 attackPosition = transform.position + new Vector3(_spriteRenderer.flipX ? _offset.x : -_offset.x, _offset.y, 0);
        _SmokeInstance.transform.position = attackPosition;
        _SmokeInstance.GetComponent<SpriteRenderer>().flipX = _spriteRenderer.flipX;
    }

    private void Down()
    {
        if(Input.GetKey(KeyCode.S))
        {
             _isDown = false;
            if (Input.GetKey(KeyCode.Space))
            {
                _isJump = true;
                _isDown = true;
                CheckWall();
            }            
        }

        if (Input.GetKey(KeyCode.W))
        {
            _isUp = false;
            if (Input.GetKey(KeyCode.Space))
            {
                _isUp = true;
                CheckWall();
            }
        }
    }

    private void HandleWallMove()
    {
        if(_isWall)
        {
            _velocity.y = Input.GetAxis("Vertical");

            _moveSpeed = (_velocity.y != 0.0f) ? _characterData.WalkSpeed : 0.0f;
            _velocity *= _moveSpeed;
            _animator.SetFloat("WallSpeed", Mathf.Abs(_velocity.y));

            transform.Translate(Vector2.up *  _velocity.y * Time.fixedDeltaTime);
            _rigidbody2D.gravityScale = 0;
        }
    }

    private void WallFalse()
    {
        //_animator.SetBool("Wall", false);
    }

    private void SmokeEffectEnd()
    {
        _SmokeInstance.SetActive(false);
    }

    private void EndDead()
    {
        _isDead = false;
        gameObject.SetActive(false);
        
        _playerAttack.EndAttack();
        _gameManager.RespawnPlayer(this);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Tile"))
        {
            _animator.SetBool("IsAir", false);
            _isJump = false;
        }
    }

    public bool IsParry()
    {
        return _playerAttack.IsParry;
    }

    public float GetDamage()
    {
        return _characterData.Power;
    }

    public float GetSkillDamage()
    {
        return _characterData.SkillPower;
    }

    private void CheckWall()
    {
        Collider2D WallBox = Physics2D.OverlapBox(transform.position, _collider2D.size, 0);

        if(WallBox.CompareTag("WallTile"))
        {
            if (_isUp)
            {
                _animator.SetTrigger("Wall");
                _isWall = true;
                _isJump = false;
                return;
            }
        }

        _isUp = false;

        //WallBox = Physics2D.OverlapBox(transform.position, new Vector2(_collider2D.size.x, _collider2D.size.y+5.0f), 0);

        Collider2D[] WallBoxAll =  Physics2D.OverlapBoxAll(transform.position, _collider2D.size, 0);

        foreach(Collider2D collider2D in WallBoxAll)
        {
            if (collider2D.CompareTag("DownTile"))
            {
                if (_isDown)
                {
                    collider2D.isTrigger = true;
                    _animator.SetBool("IsAir", true);
                }
                else
                {
                    collider2D.isTrigger = false;
                    _animator.SetBool("IsAir", false);
                    _isJump = false;
                }
            }
        }        
    }
    public void SendDamage(float damage)
    {
        Vector2 pos = _collider2D.bounds.center;
        if (_spriteRenderer.flipX)
            pos -= _attackOffset;
        else
            pos += _attackOffset;

        Collider2D[] colliders = Physics2D.OverlapBoxAll(pos, _attackSize, 0.0f);

        foreach (Collider2D collider in colliders)
        {
            MonsterControl monster = collider.GetComponent<MonsterControl>();

            if (monster == null) continue;

            monster.Hurt(damage);
        }
    }
}