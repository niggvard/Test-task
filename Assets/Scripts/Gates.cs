using System.Collections;
using DG.Tweening;
using UnityEngine;

public class Gates : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<RoadChecker>())
        {
            transform.DOMoveY(-3, 1).SetEase(Ease.Linear);
            StartCoroutine(WinCooldown());
        }
    }

    private IEnumerator WinCooldown()
    {
        yield return new WaitForSeconds(3);
        FinishMenu.Instance.ShowMenu("Success");
    }
}
