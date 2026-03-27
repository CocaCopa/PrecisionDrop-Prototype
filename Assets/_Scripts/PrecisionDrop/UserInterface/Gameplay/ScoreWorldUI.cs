using CocaCopa.Unity.Components;
using TMPro;
using UnityEngine;

namespace PrecisionDrop.UserInterface.Gameplay {
    public class ScoreWorldUI : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI valueTxt;
        [SerializeField] private MoveAndFade moveAndFadeAnim;
        [Space(10)]
        [SerializeField] private Vector3 positionOffset;

        public bool IsPlayingAnimation => moveAndFadeAnim.IsPlaying;

        public void Show(int scoreValue, Vector3 position) {
            valueTxt.SetText($"+{scoreValue}");
            transform.position = position + positionOffset;
            moveAndFadeAnim.Play();
        }
    }
}