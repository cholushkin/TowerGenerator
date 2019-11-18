using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ChunkShowCaseController : MonoBehaviour
{
    public GroupsController GroupController;
    public OrtoCameraSizeFitter CamSizeFitter1;
    public OrtoCameraSizeFitter CamSizeFitter2;

    private Vector3 _currentBoundsSize;
    private Vector2 _amountOfGroups;
    private long  _seed;

    public void Start()
    {
        StartCoroutine(Demo());
    }

    IEnumerator Demo()
    {
        while (true)
        {
            GroupController.gameObject.transform.position = Vector3.right * 1000;
            GroupController.gameObject.transform.localScale = Vector3.one * 100;
            _seed = GroupController.Seed;
            GroupController.SetRndConfiguration();
            _amountOfGroups = GroupController.GetAmmountOfGroups();
            var bbs = GroupController.CalculateBBSize();
            _currentBoundsSize = bbs;
            CamSizeFitter1.GetComponent<Camera>().DOOrthoSize(bbs.y, 1f);
            CamSizeFitter2.GetComponent<Camera>().DOOrthoSize(bbs.y, 1f);
            GroupController.gameObject.transform.localScale = Vector3.zero;
            GroupController.gameObject.transform.position = Vector3.zero;
            GroupController.gameObject.transform.DOScale(100f, 1f).SetEase(Ease.OutElastic);
            yield return new WaitForSeconds(3f);
            GroupController.gameObject.transform.DOScale(0f, 1f).SetEase(Ease.InBack);
            yield return new WaitForSeconds(1f);
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 200, 50), $"BB size {_currentBoundsSize.ToString()}");
        GUI.Label(new Rect(0, 50, 200, 50), $"Groups {_amountOfGroups.x} of {_amountOfGroups.y}");
        GUI.Label(new Rect(0, 100, 200, 50), $"Seed {_seed}");
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            StopAllCoroutines();
        }
    }
}
