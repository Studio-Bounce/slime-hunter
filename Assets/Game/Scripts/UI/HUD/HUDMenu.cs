using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HUDMenu : Menu
{
    [Header("Damage Alert")]
    [Range(1, 100)][SerializeField] int maxAlert = 50;
    [SerializeField] float damageAlertTime = 1.0f;

    VisualElement root;

    // Player
    VisualElement healthDamageVE;
    ProgressBar healthProgressBar;
    ProgressBar specialAttackBar;

    // Pickups
    [Header("Item Pickup")]
    [SerializeField] private VisualTreeAsset itemPickupListItem;
    [SerializeField] private float itemPopupLifetime = 3.0f;
    private VisualElement itemPickupContainer;
    private VisualElement[] listItemPool = new VisualElement[5];

    private struct ListItem
    {
        public VisualElement element;
        public Label quantityLabel;
        public float lifetime;
    }
    private Dictionary<ItemSO, ListItem> itemListMap = new Dictionary<ItemSO, ListItem>();

    // Weapons
    VisualElement weaponIcon;

    // Spells
    private const string spellDisabledStyle = "spell-glyph-disabled";
    private const string spellActiveStyle = "spell-glyph-active";

    // Quests
    VisualElement questNameVE;
    Label questNameLabel;
    Label questDescriptionLabel;

    // Navigation
    [Header("Navigation")]
    [SerializeField] float compassRotationOffset = 180.0f;
    Vector3 navTarget = Vector3.zero;  // in world space
    bool navigate = false;
    VisualElement compassNeedle;

    // Attack combo
    [Header("Attack Combo")]
    VisualElement attackComboVE;
    Label comboCountLabel;
    VisualElement splAttackComboKey;
    bool isComboHUDUp = false;
    bool canDoSpecialAttack = false;
    [SerializeField] float specialAttackBtnTime = 0.5f;

    bool redAlertUp = false;

    // Cached as they're getting called in OnDestroy
    GameManager gameManager;
    QuestManager questManager;

    void Start()
    {
        root = uiDocument.rootVisualElement;
        gameManager = GameManager.Instance;
        questManager = QuestManager.Instance;

        // Player
        healthDamageVE = root.Q<VisualElement>("Red_Alert");
        VisualElement background = root.Q<VisualElement>("Background");
        VisualElement leftBg = background.Q<VisualElement>("Left");
        VisualElement leftStatusArea = leftBg.Q<VisualElement>("StatusArea");
        VisualElement statusBars = leftStatusArea.Q<VisualElement>("Bars");

        VisualElement healthVE = statusBars.Q<VisualElement>("Health");
        healthProgressBar = healthVE.Q<ProgressBar>("HealthBar");
        gameManager.OnPlayerHealthChange += UpdateHealthBar;
        UpdateHealthBar(gameManager.PlayerHealth);

        VisualElement splAttackVE = statusBars.Q<VisualElement>("SpecialAttack");
        specialAttackBar = splAttackVE.Q<ProgressBar>("SplAttackBar");
        gameManager.OnPlayerSpecialAttackChange += UpdateSpecialAttackBar;
        splAttackComboKey = statusBars.Q<VisualElement>("SplAttack_Key");
        splAttackComboKey.style.opacity = 0.0f;

        // Pickups
        InventoryManager.Instance.OnItemAdded += OnItemPickup;
        itemPickupContainer = root.Q<VisualElement>("ItemPickupListContainer");
        for (int i = 0; i < listItemPool.Length; i++)
        {
            listItemPool[i] = itemPickupListItem.CloneTree();
        }


        // Weapons
        weaponIcon = root.Q<VisualElement>("WeaponIcon");

        // Quests
        VisualElement questContainer = root.Q<VisualElement>("QuestContainer");
        questNameVE = questContainer.Q<VisualElement>("Header");
        questNameLabel = questNameVE.Q<Label>("Quest-Name");
        questDescriptionLabel = questContainer.Q<Label>("Content");
        questManager.OnActiveQuestChange += UpdateActiveQuest;
        
        questNameVE.style.display = DisplayStyle.None;
        questDescriptionLabel.style.display = DisplayStyle.None;

        // Navigation
        VisualElement navigation = leftBg.Q<VisualElement>("NavigationContainer");
        compassNeedle = navigation.Q<VisualElement>("CompassNeedle");
        navigate = false;

        // Combo
        attackComboVE = root.Q<VisualElement>("ComboContainer");
        comboCountLabel = attackComboVE.Q<Label>("ComboLabel");
        isComboHUDUp = false;
        attackComboVE.style.opacity = 0;
    }

    private void FixedUpdate()
    {
        if (navigate)
        {
            UpdateCompass();
        }

        if (gameManager.PlayerSpecialAttack == 1.0f)
        {
            if (!canDoSpecialAttack)
            {
                canDoSpecialAttack = true;
                StartCoroutine(SpecialAttackIndicator());
            }
        }
        else
        {
            canDoSpecialAttack = false;
        }
    }

    // ------------------------------ Health ------------------------------

    void UpdateHealthBar(int health)
    {
        // Damage red alert UI
        if (!redAlertUp)
        {
            StartCoroutine(InitiateRedAlert());
        }

        if (healthProgressBar != null)
        {
            healthProgressBar.value = (float)health / GameManager.Instance.PlayerMaxHealth;
        }
    }

    IEnumerator InitiateRedAlert()
    {
        redAlertUp = true;
        // Set red alert to maxAlert. Reduce it to 0 over time
        int opacity = maxAlert;
        healthDamageVE.style.opacity = opacity;
        float timeDelta = damageAlertTime / maxAlert;

        while (opacity > 0)
        {
            --opacity;
            healthDamageVE.style.opacity = opacity;

            yield return new WaitForSeconds(timeDelta);
        }
        redAlertUp = false;
    }

    // ------------------------------ Pickup ------------------------------
    private void OnItemPickup(ItemSO itemSO)
    {
        // Create new List Item
        VisualElement listItem = itemPickupListItem.CloneTree();
        Label itemImage = listItem.Q<Label>("Image");
        Label itemType = listItem.Q<Label>("ItemType");
        Label itemName = listItem.Q<Label>("ItemName");
        Label itemQuantity = listItem.Q<Label>("ItemQuantity");

        itemImage.style.backgroundImage = itemSO.icon.texture;
        itemType.text = itemSO.itemType.ToString();
        itemName.text = itemSO.itemName.ToString();

        ListItem lstItem;
        if (itemListMap.ContainsKey(itemSO))
        {
            lstItem = itemListMap[itemSO];
            int.TryParse(lstItem.quantityLabel.text, out int value);
            lstItem.quantityLabel.text = (value++).ToString();
            lstItem.lifetime = itemPopupLifetime; // Reset Lifetime
        }

        lstItem = new ListItem
        {
            quantityLabel = itemQuantity,
            lifetime = itemPopupLifetime
        };

    }

    private IEnumerator AddItemToUI(ItemSO itemSO)
    {
        

        yield return null;
    }

    // ------------------------------ Special Attack ------------------------------

    void UpdateSpecialAttackBar(float splAttackStatus)
    {
        if (specialAttackBar != null)
        {
            specialAttackBar.value = splAttackStatus;
        }
    }

    IEnumerator SpecialAttackIndicator()
    {
        float btnScale = 1.0f;
        float direction = 1;
        splAttackComboKey.style.opacity = 1.0f;
        while (canDoSpecialAttack)
        {

            // Highlight the special attack button
            btnScale += direction * (Time.deltaTime / specialAttackBtnTime);
            if (btnScale <= 1.0f || btnScale >= 2.0f)
                direction *= -1;
            splAttackComboKey.transform.scale = new Vector3(btnScale, btnScale, btnScale);

            yield return null;
        }
        splAttackComboKey.style.opacity = 0.0f;
    }

    // ------------------------------ Weapons ------------------------------

    public void UpdateWeaponIcon(Sprite icon)
    {
        weaponIcon.style.backgroundImage = icon?.texture;
    }

    // ------------------------------ Spells -------------------------------

    public void UpdateSpellCooldown(int spellNumber, int value)
    {
        VisualElement skillElement = root.Q<VisualElement>($"Spell{spellNumber}");
        Label skillTimer = skillElement.Q<Label>("Timer");

        if (value > 0) {
            skillElement.AddToClassList(spellDisabledStyle);
        } else
        {
            skillElement.RemoveFromClassList(spellDisabledStyle);
        }

        skillTimer.text = value.ToString();
    }

    public void SetSpellActive(int spellNumber)
    {
        // Set active style to selected spell and remove from rest
        var skillElements = root.Query<VisualElement>().Where(
            ve => ve.name != null && ve.name.StartsWith("Spell")
            ).ToList();
        foreach (var el in skillElements)
        {
            if (el.name.Contains(spellNumber.ToString()))
            {
                el.AddToClassList(spellActiveStyle);
            } else
            {
                el.RemoveFromClassList(spellActiveStyle);
            }
        }
    }

    public void SetSpellIcon(int spellNumber, Sprite icon)
    {
        VisualElement skillElement = root.Q<VisualElement>($"Spell{spellNumber}");
        skillElement.style.backgroundImage = icon?.texture;
    }

    // ------------------------------ Quests -------------------------------

    void UpdateActiveQuest(string questName, string questDescription)
    {
        if (questName == "")
        {
            questNameVE.style.display = DisplayStyle.None;
            questDescriptionLabel.style.display = DisplayStyle.None;
            return;
        }
        questNameVE.style.display = DisplayStyle.Flex;
        questDescriptionLabel.style.display = DisplayStyle.Flex;
        questNameLabel.text = questName;
        questDescriptionLabel.text = questDescription;
    }

    // ------------------------------ Navigation -------------------------------

    void UpdateCompass()
    {
        if (GameManager.Instance.PlayerRef == null)
        {
            return;
        }
        Vector3 playerPosition = GameManager.Instance.PlayerRef.transform.position;
        Vector3 direction = (navTarget - playerPosition);
        direction.y = 0;
        direction.Normalize();
        float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
        Transform cameraT = CameraManager.ActiveCamera.transform;
        Vector3 angleVec = new Vector3(0, 0, cameraT.eulerAngles.y - angle + compassRotationOffset);
        compassNeedle.transform.rotation = Quaternion.Euler(angleVec);
    }

    public void StartNavigation(Vector3 target)
    {
        navigate = true;
        navTarget = target;
    }

    public void StopNavigation()
    {
        navigate = false;
        navTarget = Vector3.zero;
    }

    // ------------------------------ Attack Combo -------------------------------

    public void ShowCombo()
    {
        isComboHUDUp = true;
        attackComboVE.style.opacity = 1;
    }

    public void SetComboOpacity(float opacity)
    {
        isComboHUDUp = (opacity > 0);
        attackComboVE.style.opacity = opacity;
    }

    public void HideCombo()
    {
        isComboHUDUp = false;
        attackComboVE.style.opacity = 0;
    }

    public void UpdateComboCount(int comboCounter)
    {
        if (!isComboHUDUp)
            ShowCombo();

        comboCountLabel.text = comboCounter.ToString();
    }

    private void OnDestroy()
    {
        if (gameManager != null)
        {
            gameManager.OnPlayerHealthChange -= UpdateHealthBar;
        }
        if (questManager != null)
        {
            questManager.OnActiveQuestChange -= UpdateActiveQuest;
        }
    }
}
