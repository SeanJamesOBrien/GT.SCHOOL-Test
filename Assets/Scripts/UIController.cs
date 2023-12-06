using System;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public static event Action<int> OnGradeSelected = delegate { };

    [SerializeField] GameObject infoPanel;
    [SerializeField] TextMeshProUGUI gradeText;
    [SerializeField] TextMeshProUGUI clusterText;
    [SerializeField] TextMeshProUGUI standardText;

    private void Start()
    {
        JengaBlock.OnRightClick += JangleBlock_OnRightClick;
        infoPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        JengaBlock.OnRightClick -= JangleBlock_OnRightClick;
    }

    public void SetCameraPosition(int index)
    {
        OnGradeSelected?.Invoke(index);
    }

    private void JangleBlock_OnRightClick(StudentInfo info)
    {
        infoPanel.SetActive(true);
        gradeText.text = info.grade + ": " + info.domain;
        clusterText.text = info.cluster;
        standardText.text = info.standardid + ": " + info.standarddescription;
    }
}