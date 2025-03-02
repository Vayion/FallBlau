using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIDivisionOverlay : MonoBehaviour
{
    [SerializeField] GameObject divisionIconGroup;
    [SerializeField] Transform divisionIconParent;

    [SerializeField] private Camera mainCamera;

    [SerializeField] private GameObject divisionArrowPrefab;
    [SerializeField] private Transform worldUI;
    private List<UIDivisionArrow> createdDivisionArrows = new List<UIDivisionArrow>();

    public static UIDivisionOverlay instance;
    private void Awake()
    {
        instance = this;
    }

    private Dictionary<int, DivisionVisual> divisions = new Dictionary<int, DivisionVisual>();
    private Dictionary<int, GameObject> divisionIcons = new Dictionary<int, GameObject>();

    private void Start()
    {
        PlayerInteraction.OnSelectedDivisionChanged += PlayerInteraction_OnSelectedDivisionChanged;
    }

    private void OnDestroy()
    {
        PlayerInteraction.OnSelectedDivisionChanged -= PlayerInteraction_OnSelectedDivisionChanged;
    }

    private void PlayerInteraction_OnSelectedDivisionChanged(object sender, SelectedDivisionArgs e)
    {
        UpdateDivisionArrow(e.selectedDivisionIds);
    }

    public void UpdateIcon(Transform transform, int iconId)
    {
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(transform.position + Vector3.down * 0.4f + Vector3.back * 0.3f);
        divisionIcons[iconId].transform.position = screenPosition;
    }

    private void Update()
    {
        UpdateDivisionArrow(PlayerInteraction.instance.GetDivisionsSelected());
    }

    public void AddDivisionIcon(DivisionVisual divisionVisual, out UIIconGroup iconGroup, int visualId)
    {
        GameObject newDivisionIconGroup = Instantiate(divisionIconGroup, divisionIconParent);
        
        divisionIcons.Add(visualId, newDivisionIconGroup);

        iconGroup = newDivisionIconGroup.GetComponent<UIIconGroup>();
        iconGroup.visualId = visualId;
        iconGroup.SetDivisions(divisionVisual.GetDivisionIds());
    }

    public void RemoveDivisionIcon(int visualId)
    {
        if(divisionIcons.ContainsKey(visualId))
        {
            Destroy(divisionIcons[visualId]);
            divisionIcons.Remove(visualId);
        }

    }
    private void UpdateDivisionArrow(List<int> divisions)
    {
        List<int> divisionsNotCreated = new List<int>();
        divisionsNotCreated.AddRange(divisions);

        for (int i = 0; i < createdDivisionArrows.Count; i++)
        {
            UIDivisionArrow arrow = createdDivisionArrows[i];

            if (divisionsNotCreated.Contains(arrow.divisionId))
            {
                //update division arrow

                DivisionData data = VisualManager.GetDivisionData(arrow.divisionId);
                divisionsNotCreated.Remove(arrow.divisionId);

                if (data.progressPercent <= 0)
                {
                    DestroyArrow(arrow);
                }
                else
                {
                    Vector3 orginalPosition = VisualManager.GetTileVisualPosition(data.currentTileId);
                    Vector3 position = WorldToWorldUI(orginalPosition);
                    Vector3 worldPosition = WorldUIToWorld(position);
                    Vector3 arrowTargetPosition = VisualManager.GetTileVisualPosition(data.targetTileId);

                    RectTransform arrowTransform = arrow.GetComponent<RectTransform>();
                    arrowTransform.sizeDelta = new Vector2(Vector3.Distance(orginalPosition, arrowTargetPosition), arrowTransform.sizeDelta.y);
                    arrowTransform.position = worldPosition;
                    arrowTransform.rotation = LookAtPositionWorldUI(orginalPosition, arrowTargetPosition);

                    arrow.SetFill(1f - data.progressPercent);
                }
            }
            else
            {
                DestroyArrow(arrow);
            }
        }

        // create new division arrows

        for (int i = 0; i < divisionsNotCreated.Count; i++)
        {
            DivisionData data = VisualManager.GetDivisionData(divisionsNotCreated[i]);

            if(data.progressPercent <= 0)
            {
                return;
            }

            Vector3 orginalPosition = VisualManager.GetTileVisualPosition(data.currentTileId);
            Vector3 position = WorldToWorldUI(orginalPosition);
            Vector3 worldPosition = WorldUIToWorld(position);
            Vector3 arrowTargetPosition = VisualManager.GetTileVisualPosition(data.targetTileId);

            GameObject createdDivisionArrow = Instantiate(instance.divisionArrowPrefab, worldPosition, LookAtPositionWorldUI(orginalPosition, arrowTargetPosition), worldUI);

            RectTransform arrowTransform = createdDivisionArrow.GetComponent<RectTransform>();
            arrowTransform.sizeDelta = new Vector2(Vector3.Distance(orginalPosition, arrowTargetPosition), arrowTransform.sizeDelta.y);

            print(orginalPosition + " " + arrowTargetPosition);

            UIDivisionArrow arrow = createdDivisionArrow.GetComponent<UIDivisionArrow>();
            arrow.SetFill(1f - data.progressPercent);
            arrow.divisionId = divisions[i];

            createdDivisionArrows.Add(arrow);
        } 
    }

    private void DeleteDivisionArrows()
    {
        for(int i = 0; i < createdDivisionArrows.Count; i++)
        {
            Destroy(createdDivisionArrows[i].gameObject);
        }

        createdDivisionArrows.Clear();
    }

    private void DestroyArrow(UIDivisionArrow arrow)
    {
        createdDivisionArrows.Remove(arrow);
        Destroy(arrow.gameObject);
    }

    private static Quaternion LookAtPositionWorldUI(Vector3 point, Vector3 target)
    {
        Vector3 direction = target - point;
        float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
        print(direction);
        return Quaternion.Euler(90, 0, angle);

    }

    private static Vector3 WorldToWorldUI(Vector3 position)
    {
        return new Vector3(position.x, position.z, 0);
    }

    private static Vector3 WorldUIToWorld(Vector3 position)
    {
        return new Vector3(position.x, instance.worldUI.transform.position.y, position.y);
    }


}
