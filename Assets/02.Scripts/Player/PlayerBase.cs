using UnityEngine;
public class PlayerBase : MonoBehaviour
{
    protected Player _player;

    protected virtual void Start()
    {
        _player = GetComponent<Player>();
    }
}
