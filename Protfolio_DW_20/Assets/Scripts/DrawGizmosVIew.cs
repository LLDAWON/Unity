using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ColliderVisualizer : MonoBehaviour
{
    private Collider2D _collider2D;

    private void Start()
    {
        _collider2D = GetComponent<Collider2D>();
    }

    private void OnDrawGizmos()
    {
        if (_collider2D == null)
            _collider2D = GetComponent<Collider2D>();

        if (_collider2D == null)
            return;

        Vector2 areaMin = _collider2D.bounds.min;
        Vector2 areaMax = new Vector2(transform.position.x, transform.position.y);

        // Gizmos ���� ����
        Gizmos.color = Color.red;

        // ������ ���� �׸���
        Gizmos.DrawLine(new Vector2(areaMin.x, areaMin.y), new Vector2(areaMax.x, areaMin.y));
        Gizmos.DrawLine(new Vector2(areaMax.x, areaMin.y), new Vector2(areaMax.x, areaMax.y));
        Gizmos.DrawLine(new Vector2(areaMax.x, areaMax.y), new Vector2(areaMin.x, areaMax.y));
        Gizmos.DrawLine(new Vector2(areaMin.x, areaMax.y), new Vector2(areaMin.x, areaMin.y));

        // ���� ���� ��� �ݶ��̴��� ��������
        Collider2D[] colliders = Physics2D.OverlapAreaAll(areaMin, areaMax);
        foreach (Collider2D collider in colliders)
        {
            // �ݶ��̴��� ��踦 �׸���
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
        }
    }
}