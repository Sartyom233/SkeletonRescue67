using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [Header("Настройки")]
    public GameObject heartPrefab;  
    public Transform heartsParent;  
    public int maxHealth = 5;       
    public float heartOffset = 55f;  

    private Image[] hearts;

    void Start()
    {
        CreateHearts();
    }

    // Создание всех сердец при старте
    void CreateHearts()
    {
        hearts = new Image[maxHealth];
        for (int i = 0; i < maxHealth; i++)
        {
          
            GameObject heartObj = Instantiate(heartPrefab, heartsParent);
 
            RectTransform rect = heartObj.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(i * heartOffset, 0);
            hearts[i] = heartObj.GetComponent<Image>();
        }
    }


    public void UpdateHealth(int currentHealth)
    {
        if (hearts == null) return;
        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] != null)
                hearts[i].gameObject.SetActive(i < currentHealth);
        }
    }
}