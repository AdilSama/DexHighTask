using System;
using UnityEngine;

public class Tween : ITick
{
    public enum TweenType { Move, Scale, Fade, ArcMove }
    private readonly TweenType tweenType;

    public GameObject TargetObj;
    public RectTransform TargetRect;
    public CanvasGroup TargetCG;

    public Vector3 StartVec3, EndVec3;
    public float StartFloat, EndFloat;
    
    public Vector2 StartVec2, EndVec2, Center;
    public float StartAngle, EndAngle, ArcHeight;
    public bool Clockwise;

    public float Duration, Elapsed = 0f;
    
    public Action onComplete;

    public Tween(GameObject obj, Vector3 to, float duration, TweenType type)
    {
        TargetObj = obj;
        var start = obj.transform.localScale;
        StartVec3 = start;
        EndVec3 = to;
        Duration = duration;
        tweenType = type;
        ProcessingUpdate.Instance.Subscribe(this);
    }
    public Tween(RectTransform rect, Vector2 to, float duration, TweenType type)
    {
        TargetRect = rect;
        StartVec2 = rect.anchoredPosition;
        EndVec2 = to;
        Duration = duration;
        tweenType = type;
        ProcessingUpdate.Instance.Subscribe(this);
    }
    public Tween(CanvasGroup cg, float to, float duration, TweenType type)
    {
        TargetCG = cg;
        EndFloat = to;
        Duration = duration;
        tweenType = type;
        ProcessingUpdate.Instance.Subscribe(this);
    }
    
    public Tween SetOnComplete(Action callback)
    {
        onComplete = callback;
        return this;
    }

    public static Tween Move(RectTransform rect, Vector2 to, float duration) => new (rect, to, duration, TweenType.Move);
    public static Tween Scale(GameObject obj, Vector3 to, float duration) => new (obj, to, duration, TweenType.Scale);
    public static Tween Fade(CanvasGroup cg, float to, float duration) => new (cg, to, duration, TweenType.Fade);
    public static Tween MoveCircular(RectTransform rect, RectTransform target, RectTransform center, float duration, bool clockwise = true)
    {
        Vector2 start = rect.anchoredPosition;
        Vector2 end = target.anchoredPosition;
        Vector2 pivot = center.anchoredPosition;

        Vector2 fromCenter = start - pivot;
        Vector2 toCenter = end - pivot;

        float startAngle = Mathf.Atan2(fromCenter.y, fromCenter.x) * Mathf.Rad2Deg;
        float endAngle = Mathf.Atan2(toCenter.y, toCenter.x) * Mathf.Rad2Deg;

        if (clockwise && endAngle > startAngle)
            endAngle -= 360f;
        if (!clockwise && endAngle < startAngle)
            endAngle += 360f;

        float radius = fromCenter.magnitude;

        Tween tween = new Tween(rect, target.anchoredPosition, duration, TweenType.ArcMove);
        tween.StartAngle = startAngle;
        tween.EndAngle = endAngle;
        tween.Center = pivot;
        tween.ArcHeight = radius;
        tween.Clockwise = clockwise;

        return tween;
    }

    public void Tick()
    {
        Elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(Elapsed / Duration);

        switch (tweenType)
        {
            case TweenType.Move:
                TargetRect.anchoredPosition = Vector2.Lerp(StartVec2, EndVec2, t);
                break;

            case TweenType.Scale:
                TargetObj.transform.localScale = Vector3.Lerp(StartVec3, EndVec3, t);
                break;

            case TweenType.Fade:
                TargetCG.alpha = Mathf.Lerp(StartFloat, EndFloat, t);
                break;

            case TweenType.ArcMove:
                float angle = Mathf.Lerp(StartAngle, EndAngle, t);
                float rad = angle * Mathf.Deg2Rad;
                Vector2 offset = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * ArcHeight;
                TargetRect.anchoredPosition = Center + offset;
                break;
        }
        if (Elapsed >= Duration)
        {
            ProcessingUpdate.Instance.Unsubscribe(this);
            onComplete?.Invoke();
        }
    }

}
