using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

public class GUIManager : MonoBehaviour
{
    private TextMeshProUGUI _hintbarText;
    private Animator _hintbarAnimator;
    private Animator _hintImageAnimator;
    private Animator _screenVictory;
    private Animator _screenDefeat;
    private Animator _restartButton;

    public bool IsPressing {
        get
        {
            return _hintImageAnimator.GetBool("IsPressing");
        }
        set
        {
            _hintImageAnimator.SetBool("IsPressing", value);
        }
    }

    private void Awake()
    {
        var canvas = transform.Find("Canvas");
        {
            Transform hintbar = canvas.Find("Hintbar");
            _hintbarAnimator = hintbar.GetComponent<Animator>();
            _hintbarText = hintbar.GetComponent<TextMeshProUGUI>();
            // _hintbarText.alpha = 0f;
        }
        {
            Transform hintImage = canvas.Find("Hint Image");
            // Image image = hintImage.GetComponent<Image>();
            _hintImageAnimator = hintImage.GetComponent<Animator>();
            // Color color = image.color;
            // color.a = 0;
            // image.color = color;
        }
        {
            Transform screen = transform.Find("Screens");
            _screenVictory = screen.Find("Victory Background").GetComponent<Animator>();
            _screenDefeat = screen.Find("Defeat Background").GetComponent<Animator>();
            _restartButton = screen.Find("Restart Button").GetComponent<Animator>();
            // _screenDefeat.keepAnimatorStateOnDisable = true;
            // _screenDefeat.keepAnimatorStateOnDisable = true;
            // _restartButton.keepAnimatorStateOnDisable = true;
            _screenVictory.gameObject.SetActive(false);
            _screenDefeat.gameObject.SetActive(false);
            _restartButton.gameObject.SetActive(false);
        }
    }

    public void EnableHintImage()
    {
        _hintImageAnimator.SetTrigger("Show");
    }
    public void DisableHintImage()
    {
        _hintImageAnimator.SetTrigger("Hide");

    }
    public void SetHintBar(string text)
    {
        // Debug.Log(text);
        _hintbarText.text = text;
        _hintbarAnimator.SetTrigger("Show");
    }

    public void OnGameOver(bool is_victory)
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        Animator screen = is_victory ? _screenVictory : _screenDefeat;
        screen.gameObject.SetActive(true);
        screen.Play("ActiveScreenBackground");
    }

    public void OnGameOverScreenDisplay()
    {
        _restartButton.gameObject.SetActive(true);
        _restartButton.Play("ActiveResetButton");
    }

    public void OnRestartButtonClick()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
