using System;
using System.Collections.Generic;
using UnityEngine;

public class Spellbook : MonoBehaviour
{
    public static Spellbook Instance;

    public Action tickGameState;
    public Action<float> runeManaDecay;

    public float pageGap = 0.1f;
    public Vector3 pageOffset = Vector3.zero;

    public List<Page> pages = new();
    private int _pageIndex;
    public int CurrentPageIndex
    {
        get => _pageIndex;
        set
        {
            _pageIndex = value;
            ChangePage();
        }
    }

    private void Awake()
    {
        Instance = this;
        UpdatePageExistence();
    }

    private void FixedUpdate()
    {
        TickMagic();
    }

    public void UpdatePageAccess(Page page, bool accessable)
    {
        page.accessible = accessable;
        int i = pages.IndexOf(page);
        if(pages.Count > i) pages[i+1].accessible = accessable;
        UpdatePageExistence();
    }

    private void UpdatePageExistence()
    {
        float bookThickness = pages.Count / 2 * pageGap;
        for(int i = 0; i < pages.Count; i += 2)
        {
            if (!pages[i].accessible) continue;

            GameObject binding = new GameObject("binding");
            binding.transform.parent = transform;
            binding.transform.localPosition = new Vector3(0, (-bookThickness / 2) + (pageGap * (i / 2)), 0);

            Page rhs = pages[i];
            rhs.transform.parent = binding.transform;
            rhs.transform.localPosition = pageOffset;

            Page lhs = pages[i + 1];
            lhs.transform.parent = binding.transform;
            lhs.transform.localPosition = pageOffset;
            lhs.transform.localScale = new Vector3(1, -1, 1);
        }
    }

    private void ChangePage()
    {

    }

    private void TickMagic()
    {

    }
}
