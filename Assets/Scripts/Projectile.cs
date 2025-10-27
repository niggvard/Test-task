
using NUnit.Framework;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Charge settings")]
    [SerializeField] private Vector3 _defaultSize;
    [field: SerializeField] public float InitialCharge { get; private set; }
    [field: SerializeField] public float Charge { get; private set; }

    [Header("Movement Settings")]
    [SerializeField] private Vector3 _direction;
    [SerializeField] private float _speed;
    [SerializeField] private Rigidbody _body;
    [SerializeField] private GameObject _trail;

    [Header("Overlap settings")]
    [SerializeField] private LayerMask _enemyMask;
    [SerializeField] private float _radiusModifier;

    private bool isLaunched;

    private void Start()
    {
        _trail.SetActive(false);
        isLaunched = false;
        transform.localScale = _defaultSize;
        Charge = InitialCharge;
    }

    public void AddCharge(float amount)
    {
        if (isLaunched) return;

        Charge += amount;
        var sizePercent = Charge / (InitialCharge / 100);
        transform.localScale = _defaultSize * (sizePercent / 100);
    }

    public void Launch()
    {
        isLaunched = true;
        _trail.SetActive(true);
        _body.linearVelocity = _direction.normalized * _speed * Time.fixedDeltaTime;

        Destroy(gameObject, 10);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Enemy>())
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, Charge * _radiusModifier, _enemyMask);
            foreach (var hit in hits)
                hit.GetComponent<Enemy>().Kill();

            Destroy(gameObject);
        }
    }
}