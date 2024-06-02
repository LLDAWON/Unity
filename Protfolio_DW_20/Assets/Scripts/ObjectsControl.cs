using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsControl : MonoBehaviour
{
    private Animator _animator;
    private void Awake()
    {
        //_animator = transform.GetChild(0).GetComponent<Animator>();
        _animator = GetComponent<Animator>();
        //_animator.Rebind();
        _animator.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            _animator.enabled = true;
            //_animator.SetTrigger("Open");
        }
    }

}
