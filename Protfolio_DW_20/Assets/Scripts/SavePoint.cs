using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    private GameObject _parent;
    private Animator _animatorBody;
    private Animator _animatorShield;
    private Animator _gameObject;
    private GameManager _gameManager;

    private void Awake()
    {
        _parent = GameObject.Find("Objects/SavePointBody");
        _animatorBody = GetComponent<Animator>();
        _animatorShield = _parent.transform.GetChild(1).GetComponent<Animator>();
        _gameObject = _parent.transform.GetChild(3).GetComponent<Animator>();        
        _animatorBody.enabled = false;
        _animatorShield.enabled = false;
        _gameObject.gameObject.SetActive(false);
        _gameObject.enabled = false;
    }

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _animatorBody.enabled = true;
            _gameManager.SetSavePoint(transform.position);
        }
    }
    private void ShieldAnimation()
    {
        _animatorShield.enabled = true;
    }

    private void PointStart()
    {
        _animatorBody.gameObject.SetActive(false);
        _animatorShield.gameObject.SetActive(false);
        _gameObject.gameObject.SetActive(true);
        _gameObject.enabled = true;
    }
        
}
