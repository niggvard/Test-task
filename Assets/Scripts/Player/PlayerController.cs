using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }
    public static event Action ProjectileLaunched;

    public bool IsAlive { get; private set; }

    [Header("Charge settings")]
    [SerializeField] private Vector3 _defaultSize;
    [SerializeField] private float _chargeChangeValuePerTick;
    [field: SerializeField] public float MaxCharge { get; private set; }
    [field: SerializeField] public float MinCharge { get; private set; }
    [field: SerializeField] public float CurrentCharge { get; private set; }

    [Header("Projectile settings")]
    [SerializeField] private Projectile _projectilePrefab;
    [SerializeField] private Vector3 _projectileSpawnOffset;

    [Header("Movement settings")]
    [SerializeField] private Rigidbody _body;
    [SerializeField] private Vector3 _movementDirection;
    [SerializeField] private float _movementSpeed;

    private Projectile _currentProjectile;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Player controller clone");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        IsAlive = true;
        CurrentCharge = MaxCharge;
        transform.localScale = _defaultSize;

        RoadChecker.RoadAvaliabilityChanged += OnRoadAvaliabilityChanged;
        OnRoadAvaliabilityChanged();
    }

    private void OnDisable()
    {
        RoadChecker.RoadAvaliabilityChanged -= OnRoadAvaliabilityChanged;
    }

    private void OnRoadAvaliabilityChanged()
    {
        if (RoadChecker.Instance.IsRoadAvaliable)
            _body.linearVelocity = _movementDirection.normalized * _movementSpeed * Time.fixedDeltaTime;
        else
            _body.linearVelocity = new(); 
    }

    public void DecreaseCharge(float amount)
    {
        if (!IsAlive) return;

        CurrentCharge -= amount;
        var sizePercent = CurrentCharge / (MaxCharge / 100);
        transform.localScale = _defaultSize * (sizePercent / 100);

        if (CurrentCharge < MinCharge)
            Kill(); 
    }

    private void Kill()
    {
        IsAlive = false;
        FinishMenu.Instance.ShowMenu("Fail");
    }

    public void StartProjectileSpawn()
    {
        if (!IsAlive) return;

        _currentProjectile = Instantiate(_projectilePrefab, transform.position + _projectileSpawnOffset, Quaternion.identity);
        DecreaseCharge(_currentProjectile.InitialCharge);
        StartCoroutine(GrowProjectile());
    }

    public void LaunchProjectile()
    {
        if (!IsAlive) return;

        StopAllCoroutines();
        _currentProjectile.Launch();
        _currentProjectile = null;
        ProjectileLaunched?.Invoke();
    }

    private IEnumerator GrowProjectile()
    {
        while (IsAlive)
        {
            yield return new WaitForSeconds(Time.fixedDeltaTime);
            _currentProjectile.transform.position = transform.position + _projectileSpawnOffset;
            DecreaseCharge(_chargeChangeValuePerTick);
            _currentProjectile?.AddCharge(_chargeChangeValuePerTick);
        }
    }
}
