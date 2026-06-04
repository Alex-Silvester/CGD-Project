using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[ExecuteInEditMode]
public class ImageFillGradient : MonoBehaviour
{

    [SerializeField] private Gradient _gradient = null;
    [SerializeField] private Image _image = null;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    [ExecuteAlways]
    private void Update()
    {
        _image.color = _gradient.Evaluate(_image.fillAmount);
    }

}
