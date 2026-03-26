// ═══════════════════════════════════════════════════════════════
// LoadingProgressBar.cs
// ═══════════════════════════════════════════════════════════════
using UnityEngine;
using UnityEngine.UI;

namespace LoadingSystem
{
    /// <summary>
    /// Simple UI component that displays loading progress.
    /// Attach to a UI Image or Slider component.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class LoadingProgressBar : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private LoadingProgressTracker _tracker;
        [SerializeField] private Text _progressText;
        [SerializeField] private Text _statusText;

        [Header("Settings")]
        [SerializeField] private bool _showPercentage = true;
        [SerializeField] private string _percentageFormat = "0%";
        [SerializeField] private string _statusFormat = "Loading: {0}";

        private Image _progressImage;
        private Slider _progressSlider;

        private void Awake()
        {
            _progressImage = GetComponent<Image>();
            _progressSlider = GetComponent<Slider>();

            if (_tracker == null)
                _tracker = GetComponentInParent<LoadingProgressTracker>();
        }

        private void OnEnable()
        {
            if (_tracker != null)
            {
                _tracker.OnProgressChanged.AddListener(UpdateProgress);
                _tracker.OnStatusChanged.AddListener(UpdateStatus);
            }
        }

        private void OnDisable()
        {
            if (_tracker != null)
            {
                _tracker.OnProgressChanged.RemoveListener(UpdateProgress);
                _tracker.OnStatusChanged.RemoveListener(UpdateStatus);
            }
        }

        private void UpdateProgress(float progress)
        {
            // Update visual progress
            if (_progressImage != null)
                _progressImage.fillAmount = progress;
            
            if (_progressSlider != null)
                _progressSlider.value = progress;

            // Update text
            if (_progressText != null && _showPercentage)
            {
                _progressText.text = (progress * 100f).ToString(_percentageFormat);
            }
        }

        private void UpdateStatus(string status)
        {
            if (_statusText != null)
            {
                _statusText.text = string.Format(_statusFormat, status);
            }
        }

        /// <summary>
        /// Manually set progress (useful for testing or custom tracking).
        /// </summary>
        public void SetProgress(float progress)
        {
            UpdateProgress(Mathf.Clamp01(progress));
        }

        /// <summary>
        /// Manually set status text.
        /// </summary>
        public void SetStatus(string status)
        {
            UpdateStatus(status);
        }
    }
}