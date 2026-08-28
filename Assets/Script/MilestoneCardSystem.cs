using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MilestoneCardSystem : MonoBehaviour
{
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

    [Header("References")]
    [SerializeField] private CameraFollow scoreSource;
    [SerializeField] private PlayeerController player;
    [SerializeField] private GameObject cardOverlay;
    [SerializeField] private Transform cardContainer;
    [SerializeField] private GameObject cardButtonPrefab;
    [SerializeField] private Text milestoneText;
    [SerializeField] private GameManager gameManager;

    [Header("Milestone")]
    [SerializeField] private int milestoneInterval = 500;

    private readonly List<CardDefinition> cardPool = new List<CardDefinition>();
    private int nextMilestone;
    private bool choosingCard;

    private void Awake()
    {
        if (scoreSource == null)
            scoreSource = FindObjectOfType<CameraFollow>();
        if (player == null)
            player = FindObjectOfType<PlayeerController>();
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();

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

    private void OnScoreChanged(int score)
    {
        if (!choosingCard && milestoneInterval > 0 && score >= nextMilestone)
        {
            while (score >= nextMilestone)
                nextMilestone += milestoneInterval;
            ShowCardChoice();
        }
    }

    private void ShowCardChoice()
    {
        choosingCard = true;
        Time.timeScale = 0f;

        if (cardOverlay != null)
            cardOverlay.SetActive(true);
        if (milestoneText != null)
            milestoneText.text = "Milestone " + (nextMilestone - milestoneInterval);

        ClearCards();
        List<CardDefinition> choices = GetRandomChoices(3);
        foreach (CardDefinition card in choices)
        {
            GameObject cardObject = cardButtonPrefab != null
                ? Instantiate(cardButtonPrefab, cardContainer)
                : CreateFallbackCard();
            SetupCardLayout(cardObject);
            Text[] texts = cardObject.GetComponentsInChildren<Text>(true);
            if (texts.Length > 0)
                texts[0].text = card.icon + "  " + card.title;
            if (texts.Length > 1)
                texts[1].text = card.description;

            Button button = cardObject.GetComponent<Button>();
            if (button != null)
                button.onClick.AddListener(() => ChooseCard(card));
        }

        if (cardContainer is RectTransform containerRect)
            LayoutRebuilder.ForceRebuildLayoutImmediate(containerRect);
    }

    private void ChooseCard(CardDefinition card)
    {
        ApplyCard(card.type);
        choosingCard = false;
        Time.timeScale = 1f;
        if (cardOverlay != null)
            cardOverlay.SetActive(false);
    }

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

    private void ClearCards()
    {
        if (cardContainer == null)
            return;

        for (int i = cardContainer.childCount - 1; i >= 0; i--)
            Destroy(cardContainer.GetChild(i).gameObject);
    }

    private GameObject CreateFallbackCard()
    {
        GameObject cardObject = new GameObject("CardButton", typeof(RectTransform), typeof(Image), typeof(Button));
        cardObject.transform.SetParent(cardContainer, false);
        Image image = cardObject.GetComponent<Image>();
        image.color = Color.white;

        GameObject titleObject = new GameObject("Title");
        titleObject.transform.SetParent(cardObject.transform, false);
        Text title = titleObject.AddComponent<Text>();
        title.alignment = TextAnchor.MiddleCenter;
        title.color = Color.black;
        title.font = Resources.GetBuiltinResource<Font>("Arial.ttf");

        GameObject descriptionObject = new GameObject("Description");
        descriptionObject.transform.SetParent(cardObject.transform, false);
        Text description = descriptionObject.AddComponent<Text>();
        description.alignment = TextAnchor.MiddleCenter;
        description.color = Color.black;
        description.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        return cardObject;
    }

    private void SetupCardLayout(GameObject cardObject)
    {
        RectTransform rectTransform = cardObject.GetComponent<RectTransform>();
        if (rectTransform == null)
            rectTransform = cardObject.AddComponent<RectTransform>();

        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.localScale = Vector3.one;
        rectTransform.localRotation = Quaternion.identity;
        rectTransform.sizeDelta = new Vector2(220f, 280f);

        ContentSizeFitter contentSizeFitter = cardObject.GetComponent<ContentSizeFitter>();
        if (contentSizeFitter != null)
            Destroy(contentSizeFitter);

        LayoutElement layoutElement = cardObject.GetComponent<LayoutElement>();
        if (layoutElement == null)
            layoutElement = cardObject.AddComponent<LayoutElement>();
        layoutElement.preferredWidth = 220f;
        layoutElement.preferredHeight = 280f;
        layoutElement.minWidth = 180f;
        layoutElement.minHeight = 220f;
        layoutElement.flexibleWidth = 0f;
        layoutElement.flexibleHeight = 0f;

        Text[] texts = cardObject.GetComponentsInChildren<Text>(true);
        if (texts.Length > 0)
            SetupTextLayout(texts[0].rectTransform, new Vector2(0f, 0.58f), new Vector2(1f, 0.95f), 22);
        if (texts.Length > 1)
            SetupTextLayout(texts[1].rectTransform, new Vector2(0.08f, 0.08f), new Vector2(0.92f, 0.55f), 16);
    }

    private void SetupTextLayout(RectTransform rectTransform, Vector2 anchorMin, Vector2 anchorMax, int fontSize)
    {
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        Text text = rectTransform.GetComponent<Text>();
        text.fontSize = fontSize;
        text.resizeTextForBestFit = true;
        text.resizeTextMinSize = 10;
        text.resizeTextMaxSize = fontSize;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;
    }

    private void ApplyCard(CardType type)
    {
        switch (type)
        {
            case CardType.DensePlatforms:
                if (gameManager != null) gameManager.AddPlatformDensity(0.75f);
                break;
            case CardType.WidePlatforms:
                if (gameManager != null) gameManager.ChangePlatformWidth(1.2f);
                break;
            case CardType.DurableTemporary:
                if (gameManager != null) gameManager.AddTemporaryDurability(1);
                break;
            case CardType.OneUp:
                if (gameManager != null) gameManager.AddLife(1);
                break;
            case CardType.Panacea:
                if (player != null) player.ClearDebuffs();
                break;
            case CardType.JumpBoost:
                if (player != null) player.ApplyJumpBoost(1.25f);
                break;
            case CardType.SparsePlatforms:
                if (gameManager != null) gameManager.AddPlatformDensity(1.35f);
                break;
            case CardType.NarrowPlatforms:
                if (gameManager != null) gameManager.ChangePlatformWidth(0.8f);
                break;
            case CardType.TemporaryHeavy:
                if (gameManager != null) gameManager.SetTemporaryRatio(0.75f);
                break;
            case CardType.GameSpeed:
                if (player != null) player.ApplyGameSpeed(1.2f);
                break;
            case CardType.HeavyJump:
                if (player != null) player.ApplyJumpBoost(0.75f);
                break;
            case CardType.Rickroll:
                if (gameManager != null) gameManager.PlayRickroll();
                break;
        }
    }

    private void CreateCardPool()
    {
        cardPool.Add(new CardDefinition { type = CardType.DensePlatforms, title = "Platform Rapat", icon = "+", description = "Platform muncul lebih rapat." });
        cardPool.Add(new CardDefinition { type = CardType.WidePlatforms, title = "Platform Lebar", icon = "+", description = "Platform menjadi lebih lebar." });
        cardPool.Add(new CardDefinition { type = CardType.DurableTemporary, title = "Temporary Kuat", icon = "+", description = "Platform temporary bertahan lebih lama." });
        cardPool.Add(new CardDefinition { type = CardType.OneUp, title = "1UP", icon = "+", description = "Mendapat satu kesempatan hidup." });
        cardPool.Add(new CardDefinition { type = CardType.Panacea, title = "Panacea", icon = "+", description = "Menghapus efek negatif." });
        cardPool.Add(new CardDefinition { type = CardType.JumpBoost, title = "Jump Boost", icon = "+", description = "Lompatan menjadi lebih tinggi." });
        cardPool.Add(new CardDefinition { type = CardType.SparsePlatforms, title = "Platform Jauh", icon = "-", description = "Jarak antarplatform bertambah." });
        cardPool.Add(new CardDefinition { type = CardType.NarrowPlatforms, title = "Platform Sempit", icon = "-", description = "Platform menjadi lebih sempit." });
        cardPool.Add(new CardDefinition { type = CardType.TemporaryHeavy, title = "Temporary Banyak", icon = "-", description = "Platform temporary lebih sering, permanen berkurang." });
        cardPool.Add(new CardDefinition { type = CardType.GameSpeed, title = "Game Speed", icon = "+", description = "Kecepatan game meningkat." });
        cardPool.Add(new CardDefinition { type = CardType.HeavyJump, title = "Lompatan Berat", icon = "-", description = "Lompatan menjadi lebih rendah." });
        cardPool.Add(new CardDefinition { type = CardType.Rickroll, title = "Rickroll", icon = "!", description = "Putar musik kejutan dan tutup pilihan." });
    }
}
