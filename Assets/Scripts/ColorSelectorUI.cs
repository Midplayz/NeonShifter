using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ColorSelectorUI : MonoBehaviour
{
    [System.Serializable]
    public class CircleUI
    {
        public Image circleImage;
        public TextMeshProUGUI numberText;
    }

    public CircleUI leftCircle;
    public CircleUI centerCircle;
    public CircleUI rightCircle;

    public Color[] colorOptions; 

    private PlayerController player;

    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        UpdateColorUI(player.GetCurrentColorIndex());
    }

    public void UpdateColorUI(int currentIndex)
    {
        int total = colorOptions.Length;

        int prevIndex = (currentIndex - 1 + total) % total;
        int nextIndex = (currentIndex + 1) % total;

        if (centerCircle.circleImage != null)
        {
            centerCircle.circleImage.color = colorOptions[currentIndex];
            centerCircle.numberText.text = (currentIndex + 1).ToString();
        }

        if (leftCircle.circleImage != null)
        {
            leftCircle.circleImage.color = colorOptions[prevIndex];
            leftCircle.numberText.text = (prevIndex + 1).ToString();
        }

        if (rightCircle.circleImage != null)
        {
            rightCircle.circleImage.color = colorOptions[nextIndex];
            rightCircle.numberText.text = (nextIndex + 1).ToString();
        }
    }
}
