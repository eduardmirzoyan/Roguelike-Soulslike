using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GameObject popUpTextPrefab;
    [SerializeField] private ExperienceBar xpBar;
    [SerializeField] private Text goldText;
    [SerializeField] private GameObject stunnedAnimationObject;
    [SerializeField] private CameraShake mainCamera;
    [SerializeField] private PathfindingMap pathfindingMap;
    [SerializeField] private Player player;

    [SerializeField] private GameObject corpsePrefab;
    public Sprite aggroIndicatorSprite;
    public Sprite deaggroIndicatorSprite;
    
    public static GameManager instance; // Accessible by every class at any point
    private void Awake()
    {
        if(GameManager.instance != null)
        {
            Destroy(gameObject);
            return;
        }

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        instance = this; // Sets this to itself
        xpBar = GameObject.Find("Experience Bar").GetComponent<ExperienceBar>();
        xpBar.setMaxExperience(100);
        xpBar.setExperience(0);

        goldText = GameObject.Find("Gold Counter").GetComponent<Text>();
        goldText.text = "$ " + gold;

        // Get pathfinding map for platformer AIs
        pathfindingMap = GameObject.Find("Pathfinder Map").GetComponent<PathfindingMap>();

        player = GameObject.Find("Player").GetComponent<Player>();

        SceneManager.sceneLoaded += saveState; // Once a new scene is loaded, game is saved
        // DontDestroyOnLoad(gameObject);
    }

    // References
    // public Player player;

    // public weapon weapon...

    // Game data values
    public int gold = 0;
    public int experience = 0;
    public int skillpoints = 1;
    public int level = 1;

    // Saves game state
    /*
     * INT preferedSkin
     * INT gold
     * INT experience
     * INT playerLevel
     * 
     * 
     * */

    public Player GetPlayer() {
        return player;
    }

    public void saveState(Scene scene, LoadSceneMode mode)
    {
        string s = "";
        s += "0" + "|";
        s += gold.ToString() + "|";
        s += experience.ToString() + "|";
        s += level.ToString();

        PlayerPrefs.SetString("SaveState", s);


        SceneManager.sceneLoaded -= saveState;
        Debug.Log("Saved your state.");
    }

    // Loads your state
    public void loadState()
    {
        if (!PlayerPrefs.HasKey("SaveState")) // If there is not savestate, do nothing
            return;

        string[] data = PlayerPrefs.GetString("SaveState").Split('|');
        // Assumes our data is formated: 0|10|15|2 -> "0", "10", "15", "2"

        // Set player gold
        gold = int.Parse(data[1]);
        experience = int.Parse(data[2]);
        level = int.Parse(data[3]);

        Debug.Log("Loaded your state.");
    }

    public void CreatePopup(string text, Vector3 position)
    {
        GameObject prefab = Instantiate(popUpTextPrefab, position, Quaternion.identity);

        // Set text
        var textMeshes = prefab.GetComponentsInChildren<TextMesh>();
        foreach (var textMesh in textMeshes) {
            textMesh.text = text;
        }
    }
    
    public void addExperience(int xp)
    {
        experience += xp;
        if (experience >= 100)
        {
            experience -= 100;
            level++;
            skillpoints++;
            player.GetComponent<Health>().increaseMaxHealth(10);
            PopUpTextManager.instance.createPopup("LEVEL UP!", Color.cyan, player.gameObject.transform.position + Vector3.up * 0.5f);
        }
        xpBar.setExperience(experience);
    }

    public void addGold(int gold)
    {
        this.gold += gold;
        goldText.text = "$ " + this.gold;
    }

    public string getExperienceStatus() {
        return experience + " / " + 100;
    }

    // Temp work around before status effects
    public void stunAnimation(Transform transform, float duration)
    {
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + 0.35f, transform.position.z);
        var stun = Instantiate(stunnedAnimationObject, pos, Quaternion.identity, transform);
        
        stun.GetComponent<TimedDestroy>().setDestroyTimer(duration);  
    }

    public void shakeCamera(float duration, float magnitude)
    {
        mainCamera = GameObject.Find("Main Camera").GetComponent<CameraShake>();
        StartCoroutine(mainCamera.Shake(duration, magnitude));
    }

    public void spawnCorpse(GameObject host){
        var corpse = Instantiate(corpsePrefab).GetComponent<Corpse>();
        corpse.setHost(host);
    }
    
}
