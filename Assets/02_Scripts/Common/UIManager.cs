using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Button _startBtn;

    private void OnEnable()
    {
        _startBtn.onClick.AddListener(OnStartButtonClick);
    }

    private void OnDisable()
    {
        _startBtn.onClick.RemoveAllListeners();
    }

    private void OnStartButtonClick()
    {
        SceneManager.LoadScene("Level01");
        SceneManager.LoadScene("InGame", LoadSceneMode.Additive);
        
    }
}
