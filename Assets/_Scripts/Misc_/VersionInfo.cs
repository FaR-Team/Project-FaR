using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VersionInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _versionNumberText;
    void Start()
    {
        _versionNumberText.text = "Ver: " + Application.version;
    }
}