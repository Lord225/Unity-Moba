using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BoundsCheck.Direction;


public static class RectExt
{
    public static float DistanceToEdge(this Rect rect, Vector2 pos)
    {
        var verDelta = Mathf.Min(Mathf.Abs(rect.xMax - pos.x), Mathf.Abs(rect.xMin - pos.x));
        var horDelta = Mathf.Min(Mathf.Abs(rect.yMax - pos.y), Mathf.Abs(rect.yMin - pos.y));

        return Mathf.Min(verDelta, horDelta);
    }
}
public class BoundsCheck
{
    public float _size { get; }
    public float Outersize => 1000;

    public BoundsCheck(float size)
    {
        _size = size;
    }
    [Flags]
    public enum Direction
    {
        None = 0,
        Top = 1,
        Bottom = 2, 
        Left = 4,
        Right = 8,
    }

    public Direction CheckMouseInBounds(Vector3 mousePosition)
    {
        Vector2 mousepos = mousePosition;

        var topRect = Rect.MinMaxRect(-Outersize, Screen.height-_size, Screen.width+ Outersize, Screen.height+ Outersize);
        var botRect = Rect.MinMaxRect(-Outersize, -Outersize, Screen.width+ Outersize, _size);
        var leftRect = Rect.MinMaxRect(-Outersize, -Outersize, _size, Screen.height+ Outersize);
        var rightRect = Rect.MinMaxRect(Screen.width-_size, -Outersize, Screen.width+ Outersize, Screen.width + Outersize);

        var topdir = topRect.Contains(mousepos) ? Top : None;
        var botdir = botRect.Contains(mousepos) ? Bottom : None;
        var leftdir = leftRect.Contains(mousepos) ? Left : None;
        var rightdir = rightRect.Contains(mousepos) ? Right : None;

        return topdir|botdir|leftdir|rightdir;
    }
    /// <summary>
    /// Returns distance to edge of the screen if inside and negative value in outside
    /// </summary>
    /// <param name="mousePosition"></param>
    /// <returns></returns>
    public float GetDistanceToEdge(Vector3 mousePosition)
    {
        var mousepos = new Vector2(mousePosition.x, mousePosition.y);
        var screenRect = new Rect(0, 0, Screen.width, Screen.height);

        var contains = screenRect.Contains(mousepos) ? 1.0f : -1.0f;

        return screenRect.DistanceToEdge(mousepos) * contains;
    }
}


public class CameraControll : MonoBehaviour
{
    public float BaseBoundsSize = 30;
    public float BaseSpeed = 5.0f;
    public AnimationCurve CameraSpeedFalloff;

    [Space]
    public bool UseMouse = true;
    public bool IsCameraLocked = false;
    public bool UseKeyboard = true;

    [Space]
    public Transform FollowTarget;
    public Vector3 Displacment;

    public enum Follow
    {
        FollowTeamA,
        FollowTeamB,
        NoDefaultFollow,
    }

    [Space]
    public Follow AutoFollow;

    private BoundsCheck _boundsCheck;
    private CharacterController _controller;

    private Vector3 NormalizeMousePos(Vector3 pos)
    {
        var screen = new Vector3(Screen.width, Screen.height, 0);

        var scaled = 2.0f*pos-(screen);

        return new Vector3(scaled.x / screen.x, 0, scaled.y / screen.y);
    }

    private Vector3 GetMoveVector()
    {
        var dir = _boundsCheck.CheckMouseInBounds(Input.mousePosition);

        var pos = Vector3.zero;

        if ((dir & Bottom) == Bottom)
        {
            pos += Vector3.back;
        }
        if ((dir & Top) == Top)
        {
            pos += Vector3.forward;
        }
        if ((dir & Right) == Right)
        {
            pos += Vector3.right;
        }
        if ((dir & Left) == Left)
        {
            pos += Vector3.left;
        }

        return pos.normalized;
    }

    private float EvaluateSpeed()
    {
        var dis = _boundsCheck.GetDistanceToEdge(Input.mousePosition);

        var disNormalized = Mathf.Clamp01((_boundsCheck._size - dis) / _boundsCheck._size);
        return CameraSpeedFalloff.Evaluate(disNormalized);
    }

    Vector3 MouseUpdate()
    {
        if (!UseMouse) return Vector3.zero;

        var pos = GetMoveVector();

        var displacment = NormalizeMousePos(Input.mousePosition).normalized;

        var speed = EvaluateSpeed();

        return (pos + displacment * pos.magnitude) * speed * BaseSpeed;
    }

    Vector3 KeyboardUpdate()
    {
        if (!UseKeyboard) return Vector3.zero;

        Debug.LogWarning("Not implemented");

        return Vector3.zero;
    }


    IEnumerator FindTargetCorutine()
    {
        while (true)
        {
            if (AutoFollow != Follow.NoDefaultFollow)
            {
                var heros = FindObjectsOfType<Hero>();

                switch (AutoFollow)
                {
                    case Follow.FollowTeamA:
                        foreach (var hero in heros)
                        {
                            if (hero.team == Team.TeamA)
                                FollowTarget = hero.transform;
                        }
                        break;
                    case Follow.FollowTeamB:
                        foreach (var hero in heros)
                        {
                            if (hero.team == Team.TeamB)
                                FollowTarget = hero.transform;
                        }
                        break;
                }
            }

            yield return new WaitForSecondsRealtime(1.0f);
        }
    }
    private void Start()
    {
        _boundsCheck = new BoundsCheck(BaseBoundsSize);
        _controller = GetComponent<CharacterController>();

        StartCoroutine(FindTargetCorutine());
    }

    private void Update()
    {
        if (IsCameraLocked && FollowTarget != null)
        {
            var pos = new Vector3(FollowTarget.position.x, 0, FollowTarget.position.z) + Displacment;

            transform.position = pos;
        }
        else
        {
            var mouse = MouseUpdate();

            var keyboard = KeyboardUpdate();

            _controller.Move((mouse + keyboard) * Time.deltaTime);
        }
    }
}
