using System;
using System.Collections;
using System.Collections.Generic;
using Shapes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Spellbook : MonoBehaviour
{
    public static Spellbook Instance;

    public UnityEvent OnSpellbookOpened, OnSpellbookClosed;
    public Action<float> runeElementalDecay;

    public InputAction turnPageAction;
    public float timeToPage;
    public float bookThickness;
    public float pageAngle;
    public Vector3 bindingOffset;
    public Vector3 pageOffset;

    public SoundEffect turnPageSFX;

    public float runeLineThickness;

    public GameObject frontCover, backCover, spine;

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
    private Action bufferedAction;
    private bool turning;

    private void Awake()
    {
        Instance = this;
        Rune._imbuedMana.Clear();

        Draw.ScaleMode = Shapes.ScaleMode.Coordinate;
        Draw.LineGeometry = LineGeometry.Flat2D;
        Draw.ThicknessSpace = ThicknessSpace.Meters;
        Draw.Thickness = runeLineThickness;

        BindBook();
    }

    private void Update()
    {
        if (!turning && bufferedAction != null)
        {
            bufferedAction?.Invoke();
            bufferedAction = null;
        }
    }

    private void OnEnable()
    {
        turnPageAction.performed += TurnPage;
        turnPageAction.Enable();
        GameTick.Subscribe(ManaDecay, 101);
    }

    private void OnDisable()
    {
        turnPageAction.performed -= TurnPage;
        turnPageAction.Disable();
        GameTick.Unsubscribe(ManaDecay, 101);
    }

    private void ManaDecay()
    {
        runeElementalDecay?.Invoke(runeElementalDecayAmount);
    }

    public Page GetPageFromContainedRune(Rune rune)
    {
        foreach(Page p in pages)
        {
            if (p.ContainsRune(rune)) return p;
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

        GameObject binding = new GameObject("Front Cover Binding");
        binding.transform.parent = transform;
        //Instantiate(frontCover, binding.transform);
        frontCover.transform.parent = binding.transform;
        frontCover.transform.position = Vector3.zero;
        binding.transform.rotation = Quaternion.Euler(0, 0, 90 - pageAngle / 2);
        bindings.Add(binding.transform);

        for (int i = 0; i < pages.Count; i += 2)
        {
            binding = new GameObject($"Pages {i}-{i+1} Binding");
            binding.transform.parent = transform;

            Page rhs = pages[i];
            rhs.transform.parent = binding.transform;

            Page lhs = pages[i + 1];
            lhs.transform.parent = binding.transform;
            lhs.transform.localPosition = pageOffset;
            lhs.transform.localScale = new Vector3(lhs.transform.localScale.x * -1, -1, lhs.transform.localScale.z);

            binding.transform.rotation = Quaternion.Euler(0, 0, 90 - pageAngle / 2);
            bindings.Add(binding.transform);
        }

        binding = new GameObject("Back Cover Binding");
        binding.transform.parent = transform;
        //Instantiate(backCover, binding.transform);
        backCover.transform.parent = binding.transform;
        backCover.transform.position = Vector3.zero;
        binding.transform.rotation = Quaternion.Euler(0, 0, 90 - pageAngle / 2);
        bindings.Add(binding.transform);

        float pageGap = bookThickness / (bindings.Count - 1); // The 0th binding is on top
        for (int i = 0; i < bindings.Count; i++)
        {
            bindings[i].transform.localPosition = new Vector3(0, bookThickness - pageGap * i, 0) + bindingOffset;
        }

        //spine = Instantiate(spine, transform);
        spine.transform.localPosition = (bindings[0].localPosition + bindings[bindings.Count - 1].localPosition) / 2;
        spine.transform.rotation = Quaternion.FromToRotation(Vector3.up, bindings[0].position - bindings[bindings.Count - 1].position);
    }

    private void TurnPage(InputAction.CallbackContext context)
    {
        int direction = context.ReadValue<float>() > 0 ? -1 : 1;
        if (!turning)
        {
            CurrentRHSBindingIndex += direction;
        }
        else if(bufferedAction == null)
        {
            bufferedAction = () => CurrentRHSBindingIndex += direction;
        }
    }

    private IEnumerator GoToPage(int targetBindingIndex)
    {
        turning = true;

        int initialIndex = _RHSBindingIndex;
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
                    if (updatedIndex == 0 || updatedIndex == bindings.Count - 1 || pages[(updatedIndex - 1) * 2 + 1].accessible)
                    {
                        // Increment to the next accessible page
                        doPageAnimations += FlipPageAnimation(updatedIndex, direction, flipTimePerPage);
                        updatedIndex++;
                        break;
                    }
                    else
                    {
                        if (updatedIndex > 0 && updatedIndex < bindings.Count - 1 && pages[(updatedIndex - 1) * 2 + 1].block)
                        {
                            targetBindingIndex = updatedIndex;
                            break;
                        }
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
                    if (updatedIndex == 1 || updatedIndex == bindings.Count || pages[(updatedIndex - 1) * 2 - 2].accessible)
                    {
                        // Increment to the next accessible page
                        updatedIndex--;
                        doPageAnimations += FlipPageAnimation(updatedIndex, direction, flipTimePerPage);
                        break;
                    }
                    else
                    {
                        if (updatedIndex > 0 && updatedIndex < bindings.Count - 1 && pages[(updatedIndex - 1) * 2 - 2].block)
                        {
                            targetBindingIndex = updatedIndex;
                            break;
                        }
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
            if (turnPageSFX != null) 
            {
                if(direction < 0)
                {
                    if(_RHSBindingIndex != 0 && _RHSBindingIndex != bindings.Count - 1) turnPageSFX.Play(transform.position);
                }
                else if(_RHSBindingIndex != 1 && _RHSBindingIndex != bindings.Count)
                {
                    turnPageSFX.Play(transform.position);
                }
            } 
            float elapsedTime = 0;
            while (elapsedTime < flipTimePerPage)
            {
                elapsedTime += Time.deltaTime;
                doPageAnimations?.Invoke(elapsedTime);
                spine.transform.localPosition = (bindings[0].localPosition + bindings[bindings.Count - 1].localPosition) / 2;
                spine.transform.rotation = Quaternion.FromToRotation(Vector3.up, bindings[0].position - bindings[bindings.Count - 1].position);
                yield return new WaitForEndOfFrame();
            }
            doPageAnimations = null;
            if (_RHSBindingIndex == 0 || _RHSBindingIndex == bindings.Count) OnSpellbookClosed?.Invoke();
            if ((initialIndex == 0 || initialIndex == bindings.Count) && !(_RHSBindingIndex == 0 || _RHSBindingIndex == bindings.Count)) OnSpellbookOpened?.Invoke();
        }
        turning = false;
    }

    private Action<float> FlipPageAnimation(int index, int direction, float flipTime)
    {
        Transform binding = bindings[index];
        Quaternion startRotation = binding.rotation;
        Quaternion endRotation = binding.rotation * Quaternion.Euler(0, 0, pageAngle * direction);
        float pageGap = bookThickness / (bindings.Count - 1);
        float startGap = bookThickness - pageGap * index;
        float endGap = pageGap * index;

        return (float elapsedTime) =>
        {
            if (direction > 0) binding.transform.localPosition = new Vector3(0, Mathf.Lerp(startGap, endGap, elapsedTime / flipTime), 0);
            else binding.transform.localPosition = new Vector3(0, Mathf.Lerp(endGap, startGap, elapsedTime / flipTime), 0) + bindingOffset;
            binding.rotation = Quaternion.Slerp(startRotation, endRotation, elapsedTime / flipTime);
        };
    }
}
