// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;
using UnityEngine.Events;

namespace Eloi.PongTracking
{
    public class PongMono_RelayFirstWebcamFound : MonoBehaviour
    {
        public bool m_updateEveryFrame = true;
        public string m_lastChoosedWebcamName;
        public string m_lastUpdateDateTime;
        private WebCamTexture m_webcamTexture;
        public UnityEvent<WebCamTexture> m_onWebcamFound = new UnityEvent<WebCamTexture>();
        private void Start()
        {
            TryToFetchFirstWebcam();
        }

        public void TryToFetchFirstWebcam()
        {
            if (m_webcamTexture != null && !m_webcamTexture.isPlaying)
            {
                m_webcamTexture = null;
            }

            if (WebCamTexture.devices.Length > 0 && m_webcamTexture == null)
            {
                m_webcamTexture = new WebCamTexture(WebCamTexture.devices[0].name);
                m_webcamTexture.Play();

                m_lastChoosedWebcamName = m_webcamTexture.deviceName;
                m_lastUpdateDateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                RelayFirstWebcamFound(m_webcamTexture);
            }
        }

        private void Update()
        {
            TryToFetchFirstWebcam();
        }
        public void RelayFirstWebcamFound(WebCamTexture webcamTexture)
        {
            if (webcamTexture != null)
            {
                m_onWebcamFound.Invoke(webcamTexture);
            }
        }
    }
}
