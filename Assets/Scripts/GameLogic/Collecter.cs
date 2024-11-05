using UnityEngine;
using UnityEngine.UI;

public class Collecter : MonoBehaviour
{
    [SerializeField] private Text taskText;

    private int itemsCollected = 0;
    private int totalItems = 3;  

    private void Update()
    {
        UpdateTaskText();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Interactable"))
        {
            CollectItem(collision.gameObject);
        }
    }

    private void CollectItem(GameObject item)
    {
        itemsCollected++;
        item.SetActive(false);  
    }

    private void UpdateTaskText()
    {
        if (itemsCollected < totalItems)
        {
            taskText.text = $"Собрано объектов: {itemsCollected}/{totalItems}";
        }
        else
        {
            taskText.text = "Все объекты собраны!";
        }
    }
}
