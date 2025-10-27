using System;
using DG.Tweening;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public static event Action<Enemy> EnemyKilled;
    public bool IsDying { get; private set; }

    [SerializeField] private float _dyingAnimationDuration;

    [Header("Material settings")]
    [SerializeField] private Material _dyingMaterial;
    [SerializeField] private MeshRenderer _meshRenderer;

    private void Start()
    {
        IsDying = false;
    }

    public void Kill()
    {
        if (IsDying) return;

        IsDying = true;
        EnemyKilled?.Invoke(this);
        _meshRenderer.material = _dyingMaterial;
        transform.DOScale(0.01f, _dyingAnimationDuration).SetEase(Ease.InBounce)
            .OnComplete(() => Destroy(gameObject));
    }
}
