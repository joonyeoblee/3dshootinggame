using UnityEngine;
public class Singleton<T> : MonoBehaviour
{
    public static T Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = GetComponent<T>();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
