using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GUIManager : MonoBehaviour
{
    private TextMeshProUGUI _hintbarText;
    private Animator _hintbarAnimator;
    private Animator _hintImageAnimator;

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
            Transform hintImage = canvas.Find("HintImage");
            // Image image = hintImage.GetComponent<Image>();
            _hintImageAnimator = hintImage.GetComponent<Animator>();
            // Color color = image.color;
            // color.a = 0;
            // image.color = color;
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
}
