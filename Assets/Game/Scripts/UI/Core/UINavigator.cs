using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class UINavigator : MonoBehaviour
{
    private VisualElement root;
    private FocusController focusController;
    private Navigable focusedNavigable;
    private List<Navigable> navigables = new List<Navigable>();
    private bool initialized = false;

    private enum Direction
    {
        Up, Down, Left, Right, None
    }

    private class Navigable
    {
        public VisualElement ve;
        public Navigable left;
        public Navigable right;
        public Navigable up;
        public Navigable down;

        public override string ToString()
        {
            return $"ve: {(ve != null ? ve.name : "null")}(" +
               $"  left: {(left != null ? left.ve.name : "null")}," +
               $"  right: {(right != null ? right.ve.name : "null")}," +
               $"  up: {(up != null ? up.ve.name : "null")}," +
               $"  down: {(down != null ? down.ve.name : "null")}" +
               $")";
        }
    }

    void Start()
    {
        var uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;
        focusController = root.focusController;

        StartCoroutine(LateStart());
    }

    // UIDocument positions need a frame to calculate element positions
    IEnumerator LateStart()
    {
        yield return null;
        PopulateNavigables(root);
        LinkNavigables();
        initialized = true;
    }

    void Update()
    {
        if (initialized && InputManager.IsGamepad) Navigate();
    }

    private void Navigate()
    {
        if (navigables.Count == 0) return;

        // If no element is focused, focus the first one
        if (focusedNavigable == null)
            focusedNavigable = navigables[0];
        switch (GetDirection())
        {
            case Direction.None:
                return;
            case Direction.Left:
                focusedNavigable = focusedNavigable.left ?? focusedNavigable;
                break;
            case Direction.Right:
                focusedNavigable = focusedNavigable.right ?? focusedNavigable;
                break;
            case Direction.Up:
                focusedNavigable = focusedNavigable.up ?? focusedNavigable;
                break;
            case Direction.Down:
                focusedNavigable = focusedNavigable.down ?? focusedNavigable;
                break;
        }

        focusedNavigable.ve.Focus();
        Debug.Log(focusedNavigable);
        Debug.Log((focusController.focusedElement as VisualElement).name);
    }

    private Direction GetDirection()
    {
        Vector2 dir = InputManager.JoystickDelta;
        if (Input.GetKeyDown(KeyCode.UpArrow))
            return Direction.Up;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            return Direction.Down;
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            return Direction.Right;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            return Direction.Left;
        else
            return Direction.None;

        //Vector2 dir = InputManager.JoystickDelta;
        //if (dir.y > 0.5f)
        //    return Direction.Up;
        //else if (dir.y < -0.5f)
        //    return Direction.Down;
        //else if (dir.x > 0.5f)
        //    return Direction.Right;
        //else if (dir.x < -0.5f)
        //    return Direction.Left;
        //else
        //    return Direction.None;
    }

    // Recursively get all visual elements
    private void PopulateNavigables(VisualElement _root)
    {
        _ProcessNavigable(_root);

        foreach (var ve in _root.Children())
        {
            PopulateNavigables(ve);
        }
    }

    private void _ProcessNavigable(VisualElement ve)
    {
        if (ve.focusable)
        {
            navigables.Add(new Navigable()
            {
                ve = ve
            });

            // Assign the initial navigable
            if (focusController.focusedElement == ve)
            {
                focusedNavigable = navigables[navigables.Count - 1];
            }
        }
    }

    private void LinkNavigables()
    {
        foreach (var nav in navigables)
        {
            nav.left = GetClosestElementInDirection(nav, Direction.Left);
            nav.right = GetClosestElementInDirection(nav, Direction.Right);
            nav.up = GetClosestElementInDirection(nav, Direction.Up);
            nav.down = GetClosestElementInDirection(nav, Direction.Down);
        }
    }

    private Navigable GetClosestElementInDirection(Navigable sourceNav, Direction dir)
    {
        Vector2 sourcePos = sourceNav.ve.worldBound.center;
        Navigable closestNavigable = null;
        float closestDist = Mathf.Infinity;

        Action<Vector2, Navigable> UpdateClosestNavigable = 
            (Vector2 targetPos, Navigable nav) =>
            {
                float newDist = Vector2.Distance(sourcePos, targetPos);
                if (closestDist > newDist)
                {
                    closestNavigable = nav;
                    closestDist = newDist;
                }
            };

        foreach(var nav in navigables)
        {
            if (nav == sourceNav) continue;
            Vector2 targetPos = nav.ve.worldBound.center;

            switch (dir)
            {
                case Direction.Up:
                    if (targetPos.y < sourcePos.y)
                        UpdateClosestNavigable(targetPos, nav);
                    break;
                case Direction.Down:
                    if (targetPos.y > sourcePos.y)
                        UpdateClosestNavigable(targetPos, nav);
                    break;
                case Direction.Left:
                    if (targetPos.x < sourcePos.x)
                        UpdateClosestNavigable(targetPos, nav);
                    break;
                case Direction.Right:
                    if (targetPos.x > sourcePos.x)
                        UpdateClosestNavigable(targetPos, nav);
                    break;
            }
        }

        return closestNavigable;
    }
}
