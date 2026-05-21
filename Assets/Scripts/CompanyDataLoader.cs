using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class CompanyDataLoader : MonoBehaviour
{
    [Header("CSV File (StreamingAssets 기준 상대 경로)")]
    [SerializeField] private string csvFileName = "companies.csv";

    [Header("Hot Reload")]
    [SerializeField] private bool watchForChanges = true;
    [SerializeField] private float watchIntervalSeconds = 1f;

    private CompanyPanel[] panels;
    private string csvPath;
    private DateTime lastWriteTime;
    private float watchTimer;

    void Awake()
    {
        csvPath = Path.Combine(Application.streamingAssetsPath, csvFileName);
        panels = FindObjectsByType<CompanyPanel>(FindObjectsInactive.Include, FindObjectsSortMode.None);
    }

    void Start()
    {
        LoadAndApply();
    }

    void Update()
    {
        if (!watchForChanges) return;

        watchTimer += Time.unscaledDeltaTime;
        if (watchTimer < watchIntervalSeconds) return;
        watchTimer = 0f;

        if (!File.Exists(csvPath)) return;
        var currentWrite = File.GetLastWriteTime(csvPath);
        if (currentWrite != lastWriteTime)
            LoadAndApply();
    }

    private void LoadAndApply()
    {
        if (!File.Exists(csvPath))
        {
            Debug.LogWarning($"[CompanyDataLoader] CSV not found: {csvPath}");
            return;
        }

        try
        {
            lastWriteTime = File.GetLastWriteTime(csvPath);

            string text;
            using (var stream = new FileStream(csvPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = new StreamReader(stream, Encoding.UTF8, true))
            {
                text = reader.ReadToEnd();
            }

            var rows = SplitCsv(text);
            if (rows.Count < 2) return;

            var header = rows[0];
            var byId = new Dictionary<string, CompanyData>(StringComparer.OrdinalIgnoreCase);

            for (int i = 1; i < rows.Count; i++)
            {
                var cols = rows[i];
                if (cols.Count == 0) continue;

                var data = MapToData(header, cols);
                if (!string.IsNullOrEmpty(data.id))
                    byId[data.id] = data;
            }

            foreach (var panel in panels)
            {
                if (panel == null || string.IsNullOrEmpty(panel.CompanyId)) continue;
                if (byId.TryGetValue(panel.CompanyId, out var data))
                    panel.Apply(data);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[CompanyDataLoader] Failed to load CSV: {e.Message}");
        }
    }

    private static List<List<string>> SplitCsv(string text)
    {
        var rows = new List<List<string>>();
        var current = new List<string>();
        var field = new StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];

            if (inQuotes)
            {
                if (c == '"')
                {
                    if (i + 1 < text.Length && text[i + 1] == '"')
                    {
                        field.Append('"');
                        i++;
                    }
                    else inQuotes = false;
                }
                else if (c == '\r')
                {
                    // Excel(Windows)이 셀 안 줄바꿈을 \r\n으로 저장하므로 \r은 버림
                }
                else field.Append(c);
            }
            else
            {
                if (c == '"') inQuotes = true;
                else if (c == ',')
                {
                    current.Add(field.ToString());
                    field.Clear();
                }
                else if (c == '\r')
                {
                    // ignore, handle on \n
                }
                else if (c == '\n')
                {
                    current.Add(field.ToString());
                    field.Clear();
                    rows.Add(current);
                    current = new List<string>();
                }
                else field.Append(c);
            }
        }

        if (field.Length > 0 || current.Count > 0)
        {
            current.Add(field.ToString());
            rows.Add(current);
        }

        return rows;
    }

    private static CompanyData MapToData(List<string> header, List<string> cols)
    {
        var data = new CompanyData();
        int count = Math.Min(header.Count, cols.Count);

        for (int i = 0; i < count; i++)
        {
            var key = header[i].Trim();
            var val = cols[i];

            switch (key)
            {
                case "id": data.id = val.Trim(); break;

                case "대표자": data.representative = val; break;
                case "설립일": data.founded = val; break;
                case "업태": data.businessType = val; break;
                case "회사소개": data.intro = val; break;
                case "종목": data.sector = val; break;
                case "주요생산품목": data.products = val; break;
                case "주요제품소개": data.productIntro = val; break;
                case "소재지": data.location = val; break;

                case "대표자_글자크기": float.TryParse(val.Trim(), out data.representativeSize); break;
                case "설립일_글자크기": float.TryParse(val.Trim(), out data.foundedSize); break;
                case "업태_글자크기": float.TryParse(val.Trim(), out data.businessTypeSize); break;
                case "회사소개_글자크기": float.TryParse(val.Trim(), out data.introSize); break;
                case "종목_글자크기": float.TryParse(val.Trim(), out data.sectorSize); break;
                case "주요생산품목_글자크기": float.TryParse(val.Trim(), out data.productsSize); break;
                case "주요제품소개_글자크기": float.TryParse(val.Trim(), out data.productIntroSize); break;
                case "소재지_글자크기": float.TryParse(val.Trim(), out data.locationSize); break;

                case "이미지1": data.useImage1B = IsImageB(val); break;
                case "이미지2": data.useImage2B = IsImageB(val); break;
                case "이미지3": data.useImage3B = IsImageB(val); break;
                case "이미지4": data.useImage4B = IsImageB(val); break;
                case "이미지5": data.useImage5B = IsImageB(val); break;

                // 회사명, 메모 등 무시되는 컬럼은 case 없이 자동 스킵
            }
        }

        return data;
    }

    private static bool IsImageB(string val)
    {
        if (string.IsNullOrWhiteSpace(val)) return false;
        var v = val.Trim();
        return v.Equals("B", StringComparison.OrdinalIgnoreCase) || v == "1" || v.Equals("true", StringComparison.OrdinalIgnoreCase);
    }
}
