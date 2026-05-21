using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompanyPanelManager : MonoBehaviour
{
    [Header("Uni")]
    [SerializeField] private Button buttonUni;
    [SerializeField] private GameObject panelUni;

    [Header("Solar")]
    [SerializeField] private Button buttonSolar;
    [SerializeField] private GameObject panelSolar;

    [Header("Gervr")]
    [SerializeField] private Button buttonGervr;
    [SerializeField] private GameObject panelGervr;

    [Header("Rk")]
    [SerializeField] private Button buttonRk;
    [SerializeField] private GameObject panelRk;

    [Header("Esnt")]
    [SerializeField] private Button buttonEsnt;
    [SerializeField] private GameObject panelEsnt;

    [Header("Pro")]
    [SerializeField] private Button buttonPro;
    [SerializeField] private GameObject panelPro;

    [Header("Green")]
    [SerializeField] private Button buttonGreen;
    [SerializeField] private GameObject panelGreen;

    [Header("Gc")]
    [SerializeField] private Button buttonGc;
    [SerializeField] private GameObject panelGc;

    [Header("Ko")]
    [SerializeField] private Button buttonKo;
    [SerializeField] private GameObject panelKo;

    [Header("Lao")]
    [SerializeField] private Button buttonLao;
    [SerializeField] private GameObject panelLao;

    [Header("Solution")]
    [SerializeField] private Button buttonSolution;
    [SerializeField] private GameObject panelSolution;

    [Header("Home")]
    [SerializeField] private Button buttonHome;

    [Header("Panel Tween")]
    [SerializeField] private float fadeInDuration = 0.28f;
    [SerializeField] private float fadeOutDuration = 0.18f;
    [SerializeField] private float startScale = 0.9f;

    private GameObject[] allPanels;
    private readonly Dictionary<GameObject, CanvasGroup> canvasGroups = new();
    private readonly Dictionary<GameObject, Coroutine> activeTweens = new();
    private GameObject currentPanel;

    void Start()
    {
        allPanels = new[]
        {
            panelUni, panelSolar, panelGervr, panelRk, panelEsnt,
            panelPro, panelGreen, panelGc, panelKo, panelLao, panelSolution
        };

        foreach (var panel in allPanels)
        {
            if (panel == null) continue;
            var cg = panel.GetComponent<CanvasGroup>();
            if (cg == null) cg = panel.AddComponent<CanvasGroup>();
            canvasGroups[panel] = cg;
            cg.alpha = 0f;
            panel.transform.localScale = Vector3.one;
            panel.SetActive(false);
        }

        Bind(buttonUni, panelUni);
        Bind(buttonSolar, panelSolar);
        Bind(buttonGervr, panelGervr);
        Bind(buttonRk, panelRk);
        Bind(buttonEsnt, panelEsnt);
        Bind(buttonPro, panelPro);
        Bind(buttonGreen, panelGreen);
        Bind(buttonGc, panelGc);
        Bind(buttonKo, panelKo);
        Bind(buttonLao, panelLao);
        Bind(buttonSolution, panelSolution);

        if (buttonHome != null)
        {
            AttachHoverEffect(buttonHome);
            buttonHome.onClick.AddListener(HideCurrentPanel);
        }
    }

    private void Bind(Button button, GameObject panel)
    {
        if (button == null || panel == null) return;
        AttachHoverEffect(button);
        button.onClick.AddListener(() => ShowPanel(panel));
    }

    private void AttachHoverEffect(Button button)
    {
        if (button.GetComponent<ButtonHoverEffect>() == null)
            button.gameObject.AddComponent<ButtonHoverEffect>();
    }

    private void ShowPanel(GameObject target)
    {
        if (currentPanel == target) return;

        if (currentPanel != null)
            StartTween(currentPanel, FadeOut(currentPanel, true));

        currentPanel = target;
        StartTween(target, FadeIn(target));
    }

    private void HideCurrentPanel()
    {
        if (currentPanel == null) return;
        StartTween(currentPanel, FadeOut(currentPanel, true));
        currentPanel = null;
    }

    private void StartTween(GameObject panel, IEnumerator routine)
    {
        if (activeTweens.TryGetValue(panel, out var running) && running != null)
            StopCoroutine(running);
        activeTweens[panel] = StartCoroutine(routine);
    }

    private IEnumerator FadeIn(GameObject panel)
    {
        var cg = canvasGroups[panel];
        var tr = panel.transform;
        panel.SetActive(true);

        Vector3 from = Vector3.one * startScale;
        Vector3 to = Vector3.one;
        float fromAlpha = cg.alpha;

        tr.localScale = from;
        float t = 0f;
        while (t < fadeInDuration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / fadeInDuration);
            float eased = EaseOutCubic(k);
            cg.alpha = Mathf.LerpUnclamped(fromAlpha, 1f, eased);
            tr.localScale = Vector3.LerpUnclamped(from, to, eased);
            yield return null;
        }
        cg.alpha = 1f;
        tr.localScale = to;
        activeTweens[panel] = null;
    }

    private IEnumerator FadeOut(GameObject panel, bool deactivateWhenDone)
    {
        var cg = canvasGroups[panel];
        var tr = panel.transform;

        Vector3 from = tr.localScale;
        Vector3 to = Vector3.one * startScale;
        float fromAlpha = cg.alpha;

        float t = 0f;
        while (t < fadeOutDuration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / fadeOutDuration);
            float eased = EaseOutCubic(k);
            cg.alpha = Mathf.LerpUnclamped(fromAlpha, 0f, eased);
            tr.localScale = Vector3.LerpUnclamped(from, to, eased);
            yield return null;
        }
        cg.alpha = 0f;
        tr.localScale = Vector3.one;
        if (deactivateWhenDone) panel.SetActive(false);
        activeTweens[panel] = null;
    }

    private static float EaseOutCubic(float x)
    {
        return 1f - Mathf.Pow(1f - x, 3f);
    }
}
