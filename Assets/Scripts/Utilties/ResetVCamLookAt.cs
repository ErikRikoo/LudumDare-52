using System;
using UnityEngine;
using Cinemachine;

namespace Utilties
{
    public class ResetVCamLookAt : MonoBehaviour
    {
        private void Start()
        {
            GetComponent<CinemachineVirtualCamera>().LookAt = null;
        }
    }
}