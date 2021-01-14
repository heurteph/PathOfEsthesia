using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FitBackground : MonoBehaviour
{
    void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Image background = GetComponent<Image>();
        if (background == null) return;

        background.SetNativeSize();
        transform.localScale = new Vector3(1, 1, 1);
        float width = background.rectTransform.rect.size.x;
        float height = background.rectTransform.rect.size.y;

        // Set the anchor at the center of the screen and center the image on it
        background.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        background.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        background.rectTransform.anchoredPosition = Vector2.zero;

        // Check if the screen's ratio is higher or wider than the image's one
        if ((float)Screen.width / Screen.height > width / height)
        {
            // then match screen's width
            background.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width);
            background.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height * Screen.width / width);
        }
        else
        {
            // then match screen's height
            background.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height);
            background.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width * Screen.height / height);
        }
    }
}
