using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CompanyPanel : MonoBehaviour
{
    [System.Serializable]
    public class ToggleImageSlot
    {
        public Image image;
        public Sprite spriteA;
        public Sprite spriteB;
    }

    [Header("■ 회사 ID")]
    [Tooltip("CSV의 id 컬럼과 정확히 일치해야 함 (예: Uni, Solar...)")]
    [SerializeField] private string companyId;

    [Space(8)]
    [Header("■ 기본 정보")]
    [SerializeField] private TMP_Text representativeText;
    [SerializeField] private TMP_Text foundedText;
    [SerializeField] private TMP_Text businessTypeText;
    [SerializeField] private TMP_Text sectorText;
    [SerializeField] private TMP_Text locationText;

    [Space(8)]
    [Header("■ 회사 / 제품 소개")]
    [SerializeField] private TMP_Text introText;
    [SerializeField] private TMP_Text productsText;
    [SerializeField] private TMP_Text productIntroText;

    [Space(8)]
    [Header("■ 토글 이미지 (사용 안하는 슬롯은 비워둬도 OK)")]
    [SerializeField] private ToggleImageSlot image1;
    [SerializeField] private ToggleImageSlot image2;
    [SerializeField] private ToggleImageSlot image3;
    [SerializeField] private ToggleImageSlot image4;
    [SerializeField] private ToggleImageSlot image5;

    public string CompanyId => companyId;

    public void Apply(CompanyData data)
    {
        if (data == null) return;

        SetField(representativeText, data.representative, data.representativeSize);
        SetField(foundedText, data.founded, data.foundedSize);
        SetField(businessTypeText, data.businessType, data.businessTypeSize);
        SetField(introText, data.intro, data.introSize);
        SetField(sectorText, data.sector, data.sectorSize);
        SetField(productsText, data.products, data.productsSize);
        SetField(productIntroText, data.productIntro, data.productIntroSize);
        SetField(locationText, data.location, data.locationSize);

        ApplyToggle(image1, data.useImage1B);
        ApplyToggle(image2, data.useImage2B);
        ApplyToggle(image3, data.useImage3B);
        ApplyToggle(image4, data.useImage4B);
        ApplyToggle(image5, data.useImage5B);
    }

    private static void SetField(TMP_Text label, string value, float size)
    {
        if (label == null) return;
        if (value != null) label.text = value;
        if (size > 0f) label.fontSize = size;
    }

    private static void ApplyToggle(ToggleImageSlot slot, bool useB)
    {
        if (slot == null || slot.image == null) return;

        var sprite = useB ? slot.spriteB : slot.spriteA;
        if (sprite != null) slot.image.sprite = sprite;
    }
}
