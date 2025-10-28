using System;
using System.Collections.Generic;
using UnityEngine;

public class RoadChecker : MonoBehaviour
{
    public static RoadChecker Instance { get; private set; }
    public static event Action RoadAvaliabilityChanged;

    public bool IsRoadAvaliable { get; private set; }
    [SerializeField] private Rigidbody _body;
    private List<Enemy> _enemies;
    private Vector3 _defaultOffset;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Road checker clone");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        _enemies = new();
    }

    private void Start()
    {
        IsRoadAvaliable = true;
        _defaultOffset = transform.position - PlayerController.Instance.transform.position;
        
        PlayerController.ProjectileLaunched += ChangeWidth;
        Enemy.EnemyKilled += OnEnemyKilled;
    }

    private void OnDisable()
    {
        PlayerController.ProjectileLaunched -= ChangeWidth;
        Enemy.EnemyKilled -= OnEnemyKilled;
    }

    private void OnEnemyKilled(Enemy enemy)
    {
        if (_enemies.Contains(enemy))
        {
            _enemies.Remove(enemy);
            OnEnemiesCountChanged();
        }
    }

    private void ChangeWidth()
    {
        var scale = transform.localScale;
        scale.x = PlayerController.Instance.transform.localScale.x;
        transform.localScale = scale;
    }

    private void FixedUpdate()
    {
        if (IsRoadAvaliable)
            _body.MovePosition(PlayerController.Instance.transform.position + _defaultOffset); 
    }

    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy;
        if (enemy = other.GetComponent<Enemy>())
        {
            if (enemy.IsDying) return;

            _enemies.Add(enemy);
            OnEnemiesCountChanged();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Enemy enemy;
        if (enemy = other.GetComponent<Enemy>())
        {
            if (enemy.IsDying) return;

            _enemies.Remove(enemy);
            OnEnemiesCountChanged();
        }
    }

    private void OnEnemiesCountChanged()
    {
        if (_enemies.Count > 0)
            IsRoadAvaliable = false;
        else
            IsRoadAvaliable = true;

        if (_enemies.Count == 0 || _enemies.Count == 1)
            RoadAvaliabilityChanged?.Invoke();
    }
}
