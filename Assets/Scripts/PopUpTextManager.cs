using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpTextManager : MonoBehaviour
{
    [Header("Components")]
    public static PopUpTextManager instance;
    [SerializeField] private GameObject popUpPrefab;

    [Header("Settings")]
    [SerializeField] private float spawnVariation = 0.25f;
    [SerializeField] private float spawnVelocityX;
    [SerializeField] private float spawnVelocityY;

    private void Awake() {
        // Singleton logic
        if(GameManager.instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        // Don't destroy this
        DontDestroyOnLoad(gameObject);
    }

    public void createPopup(string message, Color color, Vector2 position) {
        // Randomize spawn location
        Vector3 spawnPosition = position + new Vector2(Random.Range(-spawnVariation, spawnVariation), Random.Range(-spawnVariation, spawnVariation));

        // Get second child
        var popUpObject = Instantiate(popUpPrefab, spawnPosition, Quaternion.identity);

        var textMeshes = popUpObject.GetComponentsInChildren<TextMesh>();
        // Set text of the shadow
        textMeshes[0].text = message;

        // Set text and color of the foreground
        textMeshes[1].text = message;
        textMeshes[1].color = color;

        // IF CRIT
        if (color == Color.yellow) {
            textMeshes[0].fontSize = 80;
            textMeshes[1].fontSize = 80;
        }

        // Set velocity
        var body = popUpObject.GetComponent<Rigidbody2D>();
        body.velocity = new Vector2(spawnVelocityX * 0.02f, spawnVelocityY * 0.02f);
    }

    public void createVerticalPopup(string message, Color color, Vector2 position) {
        // Randomize spawn location
        Vector3 spawnPosition = position + new Vector2(Random.Range(-spawnVariation, spawnVariation), Random.Range(-spawnVariation, spawnVariation));

        // Get second child
        var popUpObject = Instantiate(popUpPrefab, spawnPosition, Quaternion.identity);

        var textMeshes = popUpObject.GetComponentsInChildren<TextMesh>();
        // Set text of the shadow
        textMeshes[0].text = message;

        // Set text and color of the foreground
        textMeshes[1].text = message;
        textMeshes[1].color = color;

        // Set velocity
        var body = popUpObject.GetComponent<Rigidbody2D>();
        body.velocity = new Vector2(0, spawnVelocityY * 0.02f);
    }


    public void createShortPopup(string message, Color color, Vector2 position) {
        // Randomize spawn location
        Vector3 spawnPosition = position + new Vector2(Random.Range(-spawnVariation, spawnVariation), Random.Range(-spawnVariation, spawnVariation));

        // Get second child
        var popUpObject = Instantiate(popUpPrefab, spawnPosition, Quaternion.identity);

        var textMeshes = popUpObject.GetComponentsInChildren<TextMesh>();
        // Set text of the shadow
        textMeshes[0].text = message;

        // Set text and color of the foreground
        textMeshes[1].text = message;
        textMeshes[1].color = color;

        // Set velocity
        var body = popUpObject.GetComponent<Rigidbody2D>();
        body.velocity = new Vector2(spawnVelocityX * 0.02f, spawnVelocityY / 2 * 0.02f);
    }
}
