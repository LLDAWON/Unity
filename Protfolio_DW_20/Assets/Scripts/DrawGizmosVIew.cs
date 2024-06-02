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

        // Gizmos 색상 설정
        Gizmos.color = Color.red;

        // 오버랩 영역 그리기
        Gizmos.DrawLine(new Vector2(areaMin.x, areaMin.y), new Vector2(areaMax.x, areaMin.y));
        Gizmos.DrawLine(new Vector2(areaMax.x, areaMin.y), new Vector2(areaMax.x, areaMax.y));
        Gizmos.DrawLine(new Vector2(areaMax.x, areaMax.y), new Vector2(areaMin.x, areaMax.y));
        Gizmos.DrawLine(new Vector2(areaMin.x, areaMax.y), new Vector2(areaMin.x, areaMin.y));

        // 영역 내의 모든 콜라이더를 가져오기
        Collider2D[] colliders = Physics2D.OverlapAreaAll(areaMin, areaMax);
        foreach (Collider2D collider in colliders)
        {
            // 콜라이더의 경계를 그리기
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
        }
    }
}