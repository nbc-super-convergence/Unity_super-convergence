using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public class MapGameChmChm : MapBase
{
    [SerializeField] private ChmChmWindow Owner;
    [SerializeField] private ChmChmWindow[] Others;

    private bool imOwner;

    private void Start()
    {
    }

    public void OpenOwnerWindow()
    {
        StartCoroutine(Owner.OpenStart());
    }

    public void OpenOthersWindow()
    {
        foreach (ChmChmWindow window in Others)
        {
            StartCoroutine(window.OpenStart());
        }
    }

    public void CloseAllWindow()
    {
        StartCoroutine(Owner.CloseStart());
        foreach (ChmChmWindow window in Others)
        {
            StartCoroutine(window.CloseStart());
        }
    }
}
