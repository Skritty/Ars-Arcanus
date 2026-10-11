using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Spellbook : MonoBehaviour
{
    public static Spellbook Instance;

    public Action tickGameState;
    public Action tickDrawUpdate;
    public Action<float> runeElementalDecay;

    public InputAction turnPageAction;
    public float timeToPage;
    public float pageGap;
    public float pageAngle;
    public Vector3 pageOffset;

    public float runeElementalDecayAmount;

    public List<Page> pages = new();
    public List<Transform> bindings = new();
    private int _RHSBindingIndex;
    public int CurrentRHSBindingIndex
    {
        get => _RHSBindingIndex;
        set
        {
            StartCoroutine(GoToPage(value));
        }
    }
    private bool turning;

    private void Awake()
    {
        Instance = this;
        BindBook();
        turnPageAction.performed += TurnPage;
        turnPageAction.Enable();
    }

    private void Update()
    {
        tickDrawUpdate?.Invoke();
    }

    private void FixedUpdate()
    {
        TickMagic();
    }

    public Page GetPageFromContainedRune(Rune rune)
    {
        foreach(Page p in pages)
        {
            if (p.allRunes.Contains(rune)) return p;
        }
        return null;
    }

    public void UpdatePageAccess(Page page, bool accessable)
    {
        int i = pages.IndexOf(page);
        if (i % 2 == 0) i--;
        if(i > 0) pages[i].accessible = accessable;
        if (pages.Count > i) pages[i+1].accessible = accessable;
    }

    private void BindBook()
    {
        List<Page> pageInstances = new List<Page>();
        foreach(Page page in pages)
        {
            pageInstances.Add(Instantiate(page));
        }
        pages = pageInstances;
        for(int i = 0; i < pages.Count; i += 2)
        {
            GameObject binding = new GameObject("binding");
            binding.transform.parent = transform;
            binding.transform.localPosition = Vector3.zero;

            Page rhs = pages[i];
            rhs.transform.parent = binding.transform;
            rhs.transform.localPosition = pageOffset;

            Page lhs = pages[i + 1];
            lhs.transform.parent = binding.transform;
            lhs.transform.localPosition = pageOffset;
            lhs.transform.localScale = new Vector3(lhs.transform.localScale.x * -1, -1, lhs.transform.localScale.z);

            binding.transform.rotation = Quaternion.Euler(0, 0, 90 - pageAngle / 2);
            bindings.Add(binding.transform);
        }

        float bookThickness = bindings.Count * pageGap;
        for (int i = 0; i < bindings.Count; i++)
        {
            bindings[i].transform.localPosition = new Vector3(0, bookThickness - pageGap * i, 0);
        }
    }

    private void TurnPage(InputAction.CallbackContext context)
    {
        CurrentRHSBindingIndex += context.ReadValue<float>() > 1 ? 1 : -1;
    }
    [Button]
    public void TurnForward() => CurrentRHSBindingIndex++;
    [Button]
    public void TurnBack() => CurrentRHSBindingIndex--;

    private IEnumerator GoToPage(int targetBindingIndex)
    {
        if (!turning)
        {
            turning = true;

            int direction = (int)Mathf.Sign(targetBindingIndex - _RHSBindingIndex);
            float flipTimePerPage = timeToPage / Mathf.Abs(targetBindingIndex - _RHSBindingIndex); // Take the initial expected time to flip
            Action<float> doPageAnimations = null;

            while (true)
            {
                // If we are at the target index, the leftmost page, or the rightmost page, stop
                if (_RHSBindingIndex == targetBindingIndex || (direction < 0 && _RHSBindingIndex <= 0) || (direction > 0 && _RHSBindingIndex >= bindings.Count)) break;

                int updatedIndex = _RHSBindingIndex;
                if (direction > 0)
                {
                    while (updatedIndex <= bindings.Count)
                    {
                        if (pages[updatedIndex * 2 + 1].accessible)
                        {
                            // Increment to the next accessible page
                            doPageAnimations += FlipPageAnimation(updatedIndex, direction, flipTimePerPage);
                            updatedIndex++;
                            break;
                        }
                        else
                        {
                            // Skip the page if it is inaccessable (we still have to animate it)
                            doPageAnimations += FlipPageAnimation(updatedIndex, direction, flipTimePerPage);
                            updatedIndex++;
                            targetBindingIndex++;
                        }
                    }
                }
                else
                {
                    while (updatedIndex > 0)
                    {
                        if (pages[updatedIndex * 2 - 2].accessible)
                        {
                            // Increment to the next accessible page
                            updatedIndex--;
                            doPageAnimations += FlipPageAnimation(updatedIndex, direction, flipTimePerPage);
                            break;
                        }
                        else
                        {
                            // Skip the page if it is inaccessable (we still have to animate it)
                            updatedIndex--;
                            targetBindingIndex--;
                            doPageAnimations += FlipPageAnimation(updatedIndex, direction, flipTimePerPage);
                        }
                    }
                }

                // Did we skip pages to either end of the book and not find a page to turn to? Stay here
                if (updatedIndex > bindings.Count || updatedIndex < 0)
                {
                    doPageAnimations = null;
                    break;
                }
                _RHSBindingIndex = updatedIndex;

                // Animate page flips
                float elapsedTime = 0;
                while (elapsedTime < flipTimePerPage)
                {
                    elapsedTime += Time.deltaTime;
                    doPageAnimations?.Invoke(elapsedTime);
                    yield return new WaitForEndOfFrame();
                }
                doPageAnimations = null;
            }
            turning = false;
        }
    }

    private Action<float> FlipPageAnimation(int index, int direction, float flipTime)
    {
        Transform binding = bindings[index];
        Quaternion startRotation = binding.rotation;
        Quaternion endRotation = binding.rotation * Quaternion.Euler(0, 0, pageAngle * direction);
        float bookThickness = pages.Count / 2f * pageGap;
        float startGap = bookThickness - pageGap * index;
        float endGap = pageGap * (index + 1);

        return (float elapsedTime) =>
        {
            if (direction > 0) binding.transform.localPosition = new Vector3(0, Mathf.Lerp(startGap, endGap, elapsedTime / flipTime), 0);
            else binding.transform.localPosition = new Vector3(0, Mathf.Lerp(endGap, startGap, elapsedTime / flipTime), 0);
            binding.rotation = Quaternion.Slerp(startRotation, endRotation, elapsedTime / flipTime);
        };
    }

    private void TickMagic()
    {
        runeElementalDecay?.Invoke(runeElementalDecayAmount);
        tickGameState?.Invoke();
    }
}
