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
        public float resetTimeFix = 0.015f;
        [Space(10)]

        [Header("BUTTON LAYERS")]
        public GameObject normal;
        public GameObject highlighted;
        public GameObject clicked;
        public GameObject disable;
        private GameObject currentButton;
        [Space(10)]

        [Header("BUTTON EVENT")]
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
            currentButton = normal;
            //Loading mixer from Assets/Resources/Sounds
            AudioMixer mixer = Resources.Load("Sounds/_Mixer") as AudioMixer;
            audioSource = NewSound.instance.UIAudio;
            if(audioSource == null)
            {
                audioSource = new GameObject().AddComponent<AudioSource>();
            }           
            //setting the audio source to use the UI exposed variables from the mixer;
            audioSource.outputAudioMixerGroup = mixer.FindMatchingGroups("UI")[0];
            //Loading clips from Assets/Resources/Sounds
            audioClipHover = Resources.Load("Sounds/Hover") as AudioClip;
            audioClipClick = Resources.Load("Sounds/Click") as AudioClip;

            //Grab and Set all text to match inspector set value
            normalText = normal.GetComponentInChildren<TextMeshProUGUI>();
            highlightText = highlighted.GetComponentInChildren<TextMeshProUGUI>();
            clickText = clicked.GetComponentInChildren<TextMeshProUGUI>();
            disableText = disable.GetComponentInChildren<TextMeshProUGUI>();
            normalText.text = buttonName;
            highlightText.text = buttonName;
            clickText.text = buttonName;
            disableText.text = buttonName;
            if (disabled)
            {
                ButtonEvent(disable);
            }
        }

        //inteface mouse clicked
        public void OnPointerClick(PointerEventData eventData)
        {
            if (disabled) return;
            audioSource.PlayOneShot(audioClipClick);

            ButtonEvent(clicked);

            //starting short counter to reset button after click.
            FixClick = clickFixDelay();
            StartCoroutine(FixClick);
        }

        //interface on mouse enter.
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (disabled) return;
            isHighlighted = true;
            audioSource.PlayOneShot(audioClipHover);
            ButtonEvent(highlighted);
        }

        //interface on mouse enter.
        public void OnPointerExit(PointerEventData eventData)
        {
            if (disabled) return;
            isHighlighted = false;
            ButtonEvent(normal);
        }

        #region ButtonFix
        //The following Ienumerator waits for the inspector set fix time to reset clicked buttons to normal or highlighted.
        private IEnumerator clickFixDelay()
        {
            yield return new WaitForSecondsRealtime(resetTimeFix);
            //Invokes the inspector set events of this button
            buttonAction.Invoke();
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

        public void ToggleDisabled()
        {
            disabled = !disabled;
            if (disabled)
            {
                ButtonEvent(disable);
            }
            else
            {
                ButtonEvent(normal);
            }
        }
        //Method to alternate previous button state to inactive and the new one to active.
        private void ButtonEvent(GameObject gO)
        {
            currentButton.SetActive(false);
            gO.SetActive(true);
            currentButton = gO;
        }
    }
}
