using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Added: required for TMP_Text

public class MilestoneCardSystem : MonoBehaviour
{
    // --------------------------------------------------------------------
    // 1. Enums & Data Classes
    // --------------------------------------------------------------------
    private enum CardType
    {
        DensePlatforms,
        WidePlatforms,
        DurableTemporary,
        OneUp,
        Panacea,
        JumpBoost,
        SparsePlatforms,
        NarrowPlatforms,
        TemporaryHeavy,
        GameSpeed,
        HeavyJump,
        Rickroll
    }

    [System.Serializable]
    private class CardDefinition
    {
        public CardType type;
        public string title;
        public string icon;
        public string description;
    }

    // --------------------------------------------------------------------
    // 2. Serialized References
    // --------------------------------------------------------------------
    [Header("References")]
    [SerializeField] private CameraFollow scoreSource;
    [SerializeField] private PlayerController player;   // Fixed typo: PlayeerController → PlayerController
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject cardOverlay;
    [SerializeField] private Transform cardContainer;
    [SerializeField] private GameObject cardButtonPrefab;

    [Header("Milestone")]
    [SerializeField] private int milestoneInterval = 500;

    [Header("Button Sound")]
    [SerializeField] private AudioSource buttonAudioSource;
    [SerializeField] private AudioClip buttonClickSound;

    // --------------------------------------------------------------------
    // 3. Runtime State
    // --------------------------------------------------------------------
    private readonly List<CardDefinition> cardPool = new List<CardDefinition>();
    private int nextMilestone;
    private bool choosingCard;

    // --------------------------------------------------------------------
    // 4. Unity Lifecycle
    // --------------------------------------------------------------------
    private void Awake()
    {
        // Resolve references if not set in inspector
        if (scoreSource == null)
            scoreSource = FindObjectOfType<CameraFollow>();
        if (player == null)
            player = FindObjectOfType<PlayerController>();
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();

        // Sanity checks
        if (cardContainer == null)
            Debug.LogWarning("[MilestoneCardSystem] cardContainer belum diassign.");
        if (cardButtonPrefab == null)
            Debug.LogWarning("[MilestoneCardSystem] cardButtonPrefab belum diassign, fallback card akan dibuat.");

        CreateCardPool();
        nextMilestone = milestoneInterval;

        if (cardOverlay != null)
            cardOverlay.SetActive(false);
    }

    private void OnEnable()
    {
        if (scoreSource != null)
            scoreSource.ScoreChanged += OnScoreChanged;
    }

    private void OnDisable()
    {
        if (scoreSource != null)
            scoreSource.ScoreChanged -= OnScoreChanged;
    }

    // --------------------------------------------------------------------
    // 5. Score Handling
    // --------------------------------------------------------------------
    private void OnScoreChanged(int score)
    {
        if (!choosingCard && score >= nextMilestone)
        {
            // Advance to the next milestone(s) that have been passed
            while (score >= nextMilestone)
                nextMilestone += milestoneInterval;

            ShowCardChoice();
        }
    }

    // --------------------------------------------------------------------
    // 6. UI: Show Card Choice
    // --------------------------------------------------------------------
    private void ShowCardChoice()
    {
        if (cardContainer == null)
        {
            Debug.LogError("[MilestoneCardSystem] Kartu tidak bisa muncul karena cardContainer kosong.");
            return;
        }

        // If no prefab is assigned, create a simple fallback button
        GameObject prefab = cardButtonPrefab;
        if (prefab == null)
            prefab = CreateFallbackCard();

        choosingCard = true;
        Time.timeScale = 0f;

        if (cardOverlay != null)
            cardOverlay.SetActive(true);

        ClearCards();

        List<CardDefinition> choices = GetRandomChoices(3);
        foreach (CardDefinition card in choices)
        {
            GameObject cardObject = Instantiate(prefab, cardContainer, false);
            cardObject.SetActive(true);
            cardObject.transform.localScale = Vector3.one;

            // Layout element – ensure the prefab contains one
            LayoutElement le = cardObject.GetComponent<LayoutElement>();
            if (le == null) le = cardObject.AddComponent<LayoutElement>();
            le.preferredWidth = 420f;
            le.preferredHeight = 520f;

            // Set title / description
            TMP_Text[] texts = cardObject.GetComponentsInChildren<TMP_Text>(true);
            if (texts.Length > 0) texts[0].text = card.title;
            if (texts.Length > 1) texts[1].text = card.description;

            // Button logic
            Button button = cardObject.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => ChooseCard(card));
            }
        }

        // Optional layout helper for a rect‑transform container
        if (cardContainer is RectTransform containerRect)
        {
            containerRect.anchorMin = containerRect.anchorMax = containerRect.pivot = new Vector2(0.5f, 0.5f);
            containerRect.anchoredPosition = Vector2.zero;
            containerRect.sizeDelta = new Vector2(1400f, 600f);

            HorizontalLayoutGroup layout = containerRect.GetComponent<HorizontalLayoutGroup>();
            if (layout == null) layout = containerRect.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 40f;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            LayoutRebuilder.ForceRebuildLayoutImmediate(containerRect);
        }
    }

    // --------------------------------------------------------------------
    // 7. Card Selection
    // --------------------------------------------------------------------
    private void ChooseCard(CardDefinition card)
    {
        PlayButtonClickSound();
        ApplyCard(card.type);

        choosingCard = false;
        Time.timeScale = 1f;

        if (cardOverlay != null)
            cardOverlay.SetActive(false);

        ClearCards();
    }

    // --------------------------------------------------------------------
    // 8. Audio
    // --------------------------------------------------------------------
    private void PlayButtonClickSound()
    {
        if (buttonAudioSource != null && buttonClickSound != null)
            buttonAudioSource.PlayOneShot(buttonClickSound);
    }

    // --------------------------------------------------------------------
    // 9. Random Card Selection
    // --------------------------------------------------------------------
    private List<CardDefinition> GetRandomChoices(int amount)
    {
        List<CardDefinition> pool = new List<CardDefinition>(cardPool);
        List<CardDefinition> choices = new List<CardDefinition>();

        amount = Mathf.Min(amount, pool.Count);
        for (int i = 0; i < amount; i++)
        {
            int index = Random.Range(0, pool.Count);
            choices.Add(pool[index]);
            pool.RemoveAt(index);
        }

        return choices;
    }

    // --------------------------------------------------------------------
    // 10. Cleanup
    // --------------------------------------------------------------------
    private void ClearCards()
    {
        if (cardContainer == null)
            return;

        for (int i = cardContainer.childCount - 1; i >= 0; i--)
        {
            Transform child = cardContainer.GetChild(i);
            if (child == null)
                continue;

            Destroy(child.gameObject);
        }
    }

    // --------------------------------------------------------------------
    // 11. Fallback Button Creation
    // --------------------------------------------------------------------
    private GameObject CreateFallbackCard()
    {
        if (cardContainer == null)
            return null;

        // Simple button: Image + Button components
        GameObject cardObject = new GameObject("CardButton", typeof(RectTransform), typeof(Image), typeof(Button));
        cardObject.transform.SetParent(cardContainer, false);

        Image image = cardObject.GetComponent<Image>();
        if (image != null)
            image.color = Color.white; // Basic white background

        // Add a Text child for the title (using TMP)
        GameObject textGO = new GameObject("Title", typeof(RectTransform), typeof(TMP_Text));
        textGO.transform.SetParent(cardObject.transform, false);
        TMP_Text tmp = textGO.GetComponent<TMP_Text>();
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontSize = 36;
        tmp.color = Color.black;
        tmp.text = "Card";

        return cardObject;
    }

    // --------------------------------------------------------------------
    // 12. Apply Card Effect
    // --------------------------------------------------------------------
    private void ApplyCard(CardType type)
    {
        switch (type)
        {
            case CardType.DensePlatforms:
                gameManager?.AddPlatformDensity(0.75f);
                break;
            case CardType.WidePlatforms:
                gameManager?.ChangePlatformWidth(1.2f);
                break;
            case CardType.DurableTemporary:
                gameManager?.AddTemporaryDurability(1);
                break;
            case CardType.OneUp:
                gameManager?.AddLife(1);
                break;
            case CardType.Panacea:
                player?.ClearDebuffs();
                break;
            case CardType.JumpBoost:
                player?.ApplyJumpBoost(1.25f);
                break;
            case CardType.SparsePlatforms:
                gameManager?.AddPlatformDensity(1.35f);
                break;
            case CardType.NarrowPlatforms:
                gameManager?.ChangePlatformWidth(0.8f);
                break;
            case CardType.TemporaryHeavy:
                gameManager?.SetTemporaryRatio(0.75f);
                break;
            case CardType.GameSpeed:
                player?.ApplyGameSpeed(1.2f);
                break;
            case CardType.HeavyJump:
                player?.ApplyJumpBoost(0.75f);
                break;
            case CardType.Rickroll:
                gameManager?.PlayRickroll();
                break;
        }
    }

    // --------------------------------------------------------------------
    // 13. Card Pool Setup
    // --------------------------------------------------------------------
    private void CreateCardPool()
    {
        cardPool.Add(new CardDefinition { type = CardType.DensePlatforms, title = "Platform Rapat", icon = "+", description = "Platform muncul lebih rapat." });
        cardPool.Add(new CardDefinition { type = CardType.WidePlatforms, title = "Platform Lebar", icon = "+", description = "Platform jadi lebih lebar." });
        cardPool.Add(new CardDefinition { type = CardType.DurableTemporary, title = "Temporary Kuat", icon = "+", description = "Platform temporary lebih tahan lama." });
        cardPool.Add(new CardDefinition { type = CardType.OneUp, title = "1UP", icon = "+", description = "Tambah 1 nyawa." });
        cardPool.Add(new CardDefinition { type = CardType.Panacea, title = "Panacea", icon = "+", description = "Hapus debuff." });
        cardPool.Add(new CardDefinition { type = CardType.JumpBoost, title = "Jump Boost", icon = "+", description = "Lompatan lebih tinggi." });
        cardPool.Add(new CardDefinition { type = CardType.SparsePlatforms, title = "Platform Jauh", icon = "-", description = "Jarak antar platform lebih jauh." });
        cardPool.Add(new CardDefinition { type = CardType.NarrowPlatforms, title = "Platform Sempit", icon = "-", description = "Platform lebih sempit." });
        cardPool.Add(new CardDefinition { type = CardType.TemporaryHeavy, title = "Temporary Banyak", icon = "-", description = "Platform temporary lebih sering muncul." });
        cardPool.Add(new CardDefinition { type = CardType.GameSpeed, title = "Game Speed", icon = "+", description = "Kecepatan game naik." });
        cardPool.Add(new CardDefinition { type = CardType.HeavyJump, title = "Lompatan Berat", icon = "-", description = "Lompatan jadi lebih ringan." });
        cardPool.Add(new CardDefinition { type = CardType.Rickroll, title = "Rickroll", icon = "!", description = "Mainkan musik kejutan." });
    }
}