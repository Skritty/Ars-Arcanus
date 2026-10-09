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
    public float timeToPage = 0.4f;
    public float pageGap = 0.1f;
    public float pageAngle = 160f;
    public Vector3 pageOffset = Vector3.zero;

    public float runeElementalDecayAmount = 0.01f;

    public List<Page> pages = new();
    public List<Transform> bindings = new();
    private int _bindingIndex;
    public int CurrentRHSBindingIndex
    {
        get => _bindingIndex;
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

    public void UpdatePageAccess(Page page, bool accessable)
    {
        page.accessible = accessable;
        int i = pages.IndexOf(page);
        if(pages.Count > i) pages[i+1].accessible = accessable;
        BindBook();
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
    public void TurnForward() => CurrentRHSBindingIndex ++;
    [Button]
    public void TurnBack() => CurrentRHSBindingIndex--;

    private IEnumerator GoToPage(int targetBindingIndex)
    {
        if (!turning)
        {
            turning = true;
            int direction = (int)Mathf.Sign(targetBindingIndex - _bindingIndex);
            while (_bindingIndex != targetBindingIndex)
            {
                if ((direction < 0 && _bindingIndex == 0) || (direction > 0 && _bindingIndex == bindings.Count)) break;
                float flipTime = timeToPage / Mathf.Abs(targetBindingIndex - _bindingIndex);
                float elapsedTime = 0;
                int bindingIndex = direction > 0 ? _bindingIndex : _bindingIndex - 1;
                Transform binding = bindings[bindingIndex];
                Quaternion startRotation = binding.rotation;
                Quaternion endRotation = binding.rotation * Quaternion.Euler(0, 0, pageAngle * direction);
                float bookThickness = pages.Count / 2f * pageGap;
                float startGap = bookThickness - pageGap * bindingIndex;
                float endGap = pageGap * (bindingIndex + 1);

                while (elapsedTime < flipTime)
                {
                    if(direction > 0) binding.transform.localPosition = new Vector3(0, Mathf.Lerp(startGap, endGap, elapsedTime / flipTime), 0);
                    else binding.transform.localPosition = new Vector3(0, Mathf.Lerp(endGap, startGap, elapsedTime / flipTime), 0);

                    binding.rotation = Quaternion.Slerp(startRotation, endRotation, elapsedTime / flipTime);
                    yield return null;
                    elapsedTime += Time.deltaTime;
                }
                binding.transform.localPosition = new Vector3(0, direction > 0 ? endGap : startGap, 0);
                binding.rotation = endRotation;
                _bindingIndex += direction;
            }
            turning = false;
        }
    }

    private void TickMagic()
    {
        runeElementalDecay?.Invoke(runeElementalDecayAmount);
        tickGameState?.Invoke();
    }
}
