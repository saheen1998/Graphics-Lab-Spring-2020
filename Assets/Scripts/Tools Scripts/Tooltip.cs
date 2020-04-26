using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    public Text tooltipText;
    public RectTransform bg;

    public void ShowTooltip(string str) {
        gameObject.SetActive(true);

        tooltipText.text = str;
        Vector2 bgSize = new Vector2(tooltipText.preferredWidth + 8, tooltipText.preferredHeight + 8);

        bg.sizeDelta = bgSize;
    }

    public void HideTooltip() {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<RectTransform>().position = Input.mousePosition;
    }
}
