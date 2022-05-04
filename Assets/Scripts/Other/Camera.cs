using UnityEngine;
using System.Collections;

public class Camera : MonoBehaviour
{

    private Transform player;
    [SerializeField] private int xOffset;
    [SerializeField] private int yOffset;

    private void Start()
    {
        player = GameManager.instance.GetPlayer().transform;
    }

    private void Update()
    {
        if (player != null)
            transform.position = new Vector3(player.position.x + xOffset, player.position.y + yOffset, player.position.z - 10);
    }
}