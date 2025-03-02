using System.Collections.Generic;
using UnityEngine;

public class DivisionVisual : MonoBehaviour
{
    private List<int> divisionIds = new List<int>();
    public int visualId;
    private int tileId;
    public int targetId;

    private UIIconGroup icon;

    public Vector3 target {  get; private set; }
    public Vector3 originalPosition { get; private set; }

    private float moveProgress;

    public void Setup(List<int> id, Vector3 position, Quaternion rotation, int visualId_, int tileId_, int targetId_)
    {
        divisionIds.AddRange(id);
        visualId = visualId_;
        originalPosition = position;
        target = position;
        transform.rotation = rotation;
        transform.position = position;
        tileId = tileId_; 
        targetId = targetId_;
    }

    void Start()
    {
        UIDivisionOverlay.instance.AddDivisionIcon(this, out icon, visualId);
    }

    private void OnDestroy()
    {
        UIDivisionOverlay.instance.RemoveDivisionIcon(visualId);
    }


    void Update()
    {
        UIDivisionOverlay.instance.UpdateIcon(transform, visualId);

        // Smooth movement of division
        if ((target - originalPosition).sqrMagnitude > 0.0001f) // Use squared magnitude for performance
        {
            moveProgress += Time.deltaTime * 2; // Adjust speed factor here
            moveProgress = Mathf.Clamp01(moveProgress); // Clamp progress to avoid overshooting

            transform.position = Vector3.Lerp(originalPosition, target, moveProgress);

            if (moveProgress >= 1f) // Ensure final position is accurate
            {
                transform.position = target;
                originalPosition = target;
                moveProgress = 0;
            }
        }
        else
        {
            moveProgress = 0; // Ensure moveProgress is reset
            originalPosition = transform.position;
        }

    }

    public void AddDivision(int divisionId)
    {
        divisionIds.Add(divisionId);

        if(icon != null)
        {
            icon.SetDivisions(GetDivisionIds());
        }
    }

    public void RemoveDivision(int divisionId)
    {
        divisionIds.Remove(divisionId);

        if(icon != null)
        {
            icon.SetDivisions(GetDivisionIds());
        }

        if(divisionIds.Count <= 0)
        {
            VisualManager.DestroyVisual(visualId);
        }
    }

    public void HighlightUI(bool on)
    {
        //icon.SetIconHighlight(on);
    }
    
    public List<int> GetDivisionIds()
    {
        return divisionIds;
    }

    public int GetTile()
    {
        return tileId;  
    }
}
