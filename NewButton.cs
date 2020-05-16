using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using TMPro;
using UnityEngine.EventSystems;

namespace LobbyClient
{

    public class NewButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Button Config")]
        public string buttonName;
        public bool disabled = false;
        public float resetTimeFix = 0.5f;
        [Space(10)]

        [Header("Button Layers")]
        public GameObject normal;
        public GameObject highlighted;
        public GameObject clicked;
        public GameObject disable;
        public GameObject currentButton;
        [Space(10)]

        [Header("TIMER EVENT")]
        public UnityEvent buttonAction;

        private IEnumerator FixClick;
        private bool isHighlighted = false;

        private TextMeshProUGUI normalText;
        private TextMeshProUGUI highlightText;
        private TextMeshProUGUI clickText;
        private TextMeshProUGUI disableText;

        private AudioSource audioSource;
        private AudioClip audioClipHover;
        private AudioClip audioClipClick;

        void Start()
        {
            AudioMixer mixer = Resources.Load("Sounds/_Mixer") as AudioMixer;

            audioSource = this.gameObject.AddComponent<AudioSource>();
            audioSource.outputAudioMixerGroup = mixer.FindMatchingGroups("UI")[0];
            audioClipHover = Resources.Load("Sounds/Hover") as AudioClip;
            audioClipClick = Resources.Load("Sounds/Click") as AudioClip;

            normalText = normal.GetComponentInChildren<TextMeshProUGUI>();
            highlightText = highlighted.GetComponentInChildren<TextMeshProUGUI>();
            clickText = clicked.GetComponentInChildren<TextMeshProUGUI>();
            disableText = disable.GetComponentInChildren<TextMeshProUGUI>();

            normalText.text = buttonName;
            highlightText.text = buttonName;
            clickText.text = buttonName;
            disableText.text = buttonName;
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            audioSource.PlayOneShot(audioClipClick);

            ButtonEvent(clicked);

            FixClick = clickFixDelay();   
            StartCoroutine(FixClick);

            buttonAction.Invoke();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            audioSource.PlayOneShot(audioClipHover);
            ButtonEvent(highlighted);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (isHighlighted)
            {
                ButtonEvent(highlighted);
            }
            else
            {
                ButtonEvent(normal);
            }           
        }

        #region ButtonFix
        private IEnumerator clickFixDelay()
        {
            yield return new WaitForSecondsRealtime(resetTimeFix);
            if (isHighlighted)
            {
                ButtonEvent(highlighted);
            }
            else
            {
                ButtonEvent(normal);
            }
            yield return false;
        }
        #endregion

        private void ButtonEvent(GameObject gO)
        {
            currentButton.SetActive(false);
            gO.SetActive(true);
            currentButton = gO;
        }
    }
}
