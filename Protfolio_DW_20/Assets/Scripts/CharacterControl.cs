using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Animation;
using UnityEngine;

public class CharacterControl : MonoBehaviour
{
    [SerializeField]
    protected int _characterKey;
    [SerializeField]
    protected Vector2 _attackOffset = new Vector2(1, 0);
    [SerializeField]
    protected Vector2 _attackSize = new Vector2(3, 3);

    protected CharacterData _characterData;

    protected float _moveSpeed;
    protected float _curHP = 0.0f;

    protected bool _isDash = false;
    protected bool _isJump = false;

    protected Vector2 _velocity;

    protected Animator _animator;
    protected SpriteRenderer _spriteRenderer;
    protected Rigidbody2D _rigidbody2D;
    protected BoxCollider2D _collider2D;
    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _collider2D = GetComponent<BoxCollider2D>();

        _characterData = DataTable.Instance.GetCharacterData(_characterKey);

        _curHP = _characterData.MaxHp;
    }
    protected virtual void Update()
    {     
        Dash();
    }

    private void FixedUpdate()
    {
        Move();
        Jump();
    }

    protected void Move()
    {
        if (_isDash) return;

        transform.Translate(Vector2.right * _velocity.x * Time.fixedDeltaTime);

    }

    private void Dash()
    {
        if (_isDash)
        {
            if (_spriteRenderer.flipX)
            {
                transform.Translate(Vector2.left * _characterData.RunSpeed * Time.deltaTime);
            }
            else
            {
                transform.Translate(Vector2.right * _characterData.RunSpeed * Time.deltaTime);
            }
        }

    }
    private void DashEnd()
    {
        _isDash = false;
    }

    private void Jump()
    {
        _velocity.y = _rigidbody2D.velocity.y;
        _animator.SetFloat("Velocity", _velocity.y);
    }

    public void Hurt(float damage)
    {
        _curHP -= damage;
        _animator.SetTrigger("Hurt");
        print("HP : " + _curHP);
    }

    virtual protected void PushWall(Collider2D collision)
    {
        Vector2 pos = transform.position;

        Vector2 direction = transform.position - collision.bounds.center;

        if (direction.x > 0.0f)
            pos.x = collision.bounds.max.x + _collider2D.size.x * 0.5f;
        else
            pos.x = collision.bounds.min.x - _collider2D.size.x * 0.5f ;

        transform.position = pos;
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Wall")
        {
            PushWall(collision);
        }
    }

}
