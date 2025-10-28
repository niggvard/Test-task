using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class FinishMenu : MonoBehaviour
{
    public static FinishMenu Instance { get; private set; }

    [SerializeField] private GameObject _panel;
    [SerializeField] private TextMeshProUGUI _conditionText;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Death menu clone");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        _panel.SetActive(false);
    }

    public void ShowMenu(string text)
    {
        _conditionText.text = text;
        _panel.transform.DOScale(0, 0).SetUpdate(true);
        Time.timeScale = 0;

        _panel.SetActive(true);
        _panel.transform.DOScale(1, 2).SetEase(Ease.Linear).SetUpdate(true);
    }

    public void RestartScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
