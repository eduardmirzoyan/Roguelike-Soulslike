using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GameObject floatingTextPrefab;
    [SerializeField] private ExperienceBar xpBar;
    [SerializeField] private Text goldText;
    [SerializeField] private LootTable equipmentLootTable;
    [SerializeField] private LootTable consumableLootTable;
    [SerializeField] private GameObject stunnedAnimationObject;
    [SerializeField] private CameraShake mainCamera;
    [SerializeField] private PathfindingMap pathfindingMap;

     [Header("Materials")]
    [SerializeField] private Material perilousMaterial;
    [SerializeField] private Material defaultMaterial;

    [Header("Temp for TESTING ONLY!")]
    [SerializeField] public Sprite icon1;
    [SerializeField] public Sprite icon2;

    public static GameManager instance; // Accessible by every class at any point
    public Player player;

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
        player = GameObject.Find("Player").GetComponent<Player>();
        xpBar = GameObject.Find("Experience Bar").GetComponent<ExperienceBar>();
        xpBar.setMaxExperience(100);
        xpBar.setExperience(0);

        goldText = GameObject.Find("Gold Counter").GetComponent<Text>();
        goldText.text = "$ " + gold;

        // Get pathfinding map for platformer AIs
        pathfindingMap = GameObject.Find("Pathfinder Map").GetComponent<PathfindingMap>();

        SceneManager.sceneLoaded += saveState; // Once a new scene is loaded, game is saved
        DontDestroyOnLoad(gameObject);
    }

    // Resources for game
    public List<Sprite> playerSprites;
    public List<Sprite> weaponSprties;
    public List<int> weaponPrices;
    public List<int> xpTable;

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
        GameObject prefab = Instantiate(floatingTextPrefab, position, Quaternion.identity);
        prefab.GetComponentInChildren<TextMesh>().text = text;
    }

    public void CreatePopup(string text, Vector3 position, Color textColor)
    {
        float spawnVariation = 0.25f;
        Vector3 spawnPosition = position + new Vector3(Random.Range(-spawnVariation, spawnVariation), Random.Range(-spawnVariation, spawnVariation), 0);
        GameObject prefab = Instantiate(floatingTextPrefab, spawnPosition, Quaternion.identity);
        prefab.GetComponentInChildren<TextMesh>().text = text;
        prefab.GetComponentInChildren<TextMesh>().color = textColor;    
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
            CreatePopup("LEVEL UP!", player.gameObject.transform.position, Color.cyan);
        }
        xpBar.setExperience(experience);
    }

    public void addGold(int gold)
    {
        this.gold += gold;
        goldText.text = "$ " + this.gold;
    }

    public Item randomizeItem()
    {
        // First randomly choose between a weapon or armor
        var itemType = (ItemType)Random.Range(1, 3); // Gives (1, weapon) or (2, armor)
        
        // Then randomly choose weapon type or armor type (light, med, heavy)
        switch (itemType)
        {
            case ItemType.Weapon:
                var weaponType = (WeaponType)Random.Range(0, 2); // Chooses one of the weapontypes

                // Then set appropriate sprite
                //Sprite sprite;

                // Then set the level of the gear based on floor level

                // Create the SO item
                WeaponItem weaponItem = new WeaponItem();
                weaponItem.weaponType = weaponType;
                weaponItem.name = "Random Sword";
                weaponItem.damage = Random.Range(1, 5);
                weaponItem.sprite = icon1;

                return weaponItem;
                //break;
            case ItemType.Armor:
                var armorSlot = (EquipmentSlot)Random.Range(0, 5);
                var armorType = (ArmorType)Random.Range(0, 3);

                // Then set appropriate sprite
                //Sprite sprite;

                // Then set the level of the gear based on floor level

                // Create the SO item
                ArmorItem armorItem = new ArmorItem();
                armorItem.armorType = armorType;
                armorItem.equipSlot = armorSlot;
                armorItem.name = "Random Armor";
                armorItem.defenseValue = Random.Range(1, 10);
                armorItem.bonusStamina = Random.Range(0, 3) * 5;
                armorItem.sprite = icon2;

                return armorItem;

                //break;
        }

        // If no appropriate item was created, then return null
        return null;

        // Then decide the core stats of gear based on gear level

        // Finally randomize # of sub stats
    }

    public Item getItemDrop()
    {
        return equipmentLootTable.getDrop();
    }

    public Item getConsumableDrop()
    {
        return consumableLootTable.getDrop();
    }

    // Temp work around before status effects
    public void stunAnimation(Transform transform, float duration)
    {
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + 0.35f, transform.position.z);
        var stun = Instantiate(stunnedAnimationObject, pos, Quaternion.identity, transform);
        
        stun.GetComponent<TimedDestroy>().setDestroyTimer(duration);  
    }

    public Material getPerilousMaterial() => perilousMaterial;
    public Material getDefaultMaterial() => defaultMaterial;

    public void shakeCamera(float duration, float magnitude)
    {
        mainCamera = GameObject.Find("Main Camera").GetComponent<CameraShake>();
        StartCoroutine(mainCamera.Shake(duration, magnitude));
    }

    public PathfindingMap GetPathfindingMap() {
        return pathfindingMap;
    }
}
