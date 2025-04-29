using UnityEngine;
public class ItemSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _items;

    public void DropItem()
    {
        if (Random.value <= 0.3f)
        {
            int index = Random.Range(0, _items.Length);
            GameObject item = Instantiate(_items[index], transform.position, Quaternion.identity);
        }
    }
}
