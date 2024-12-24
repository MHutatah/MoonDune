using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace Core.Managers
{
    /// <summary>
    /// Manages all UI elements, ensuring consistent updates and visibility control.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        #region Inspector Variables

        [Header("Gravity Fuel UI")]
        [SerializeField] private Slider gravityFuelSlider;           // Slider representing gravity fuel
        [SerializeField] private Image gravityFuelFillImage;         // Fill image of the gravity fuel slider

        [Header("Player Laser UI")]
        [SerializeField] private Slider playerLaserSlider;           // Slider representing player's laser energy
        [SerializeField] private Image playerLaserFillImage;         // Fill image of the player's laser slider

        [Header("Vehicle Laser UI")]
        [SerializeField] private Slider vehicleLaserSlider;          // Slider representing vehicle's laser energy
        [SerializeField] private Image vehicleLaserFillImage;        // Fill image of the vehicle's laser slider

        [Header("Oxygen UI")]
        [SerializeField] private Slider oxygenSlider;                 // Slider representing player's oxygen
        [SerializeField] private Image oxygenFillImage;               // Fill image of the oxygen slider
        [SerializeField] private GameObject oxygenWarningMessage;     // UI element indicating low oxygen

        [Header("Crystal Count UI")]
        [SerializeField] private TextMeshProUGUI crystalCountText;    // Text element showing crystal count

        [Header("Game State UI")]
        [SerializeField] private GameObject gameOverScreen;           // UI element for Game Over
        [SerializeField] private GameObject gameWinScreen;            // UI element for Victory

        [Header("Pickup Message UI")]
        [SerializeField] private TextMeshProUGUI pickupMessageText;   // Text element for pickup messages
        [SerializeField] private float pickupMessageDuration = 2f;    // Duration the pickup message is displayed

        [Header("Interaction Prompt UI")]
        [SerializeField] private TextMeshProUGUI interactionPromptText; // Text element for interaction prompts
        [SerializeField] private string enterVehiclePrompt = "Press E to enter vehicle";
        [SerializeField] private string exitVehiclePrompt = "Press E to exit vehicle";

        [Header("Vehicle HUD UI")]
        [SerializeField] private GameObject vehicleHUD;               // Reference to the Vehicle HUD UI GameObject

        #endregion

        #region Private Variables

        private Coroutine pickupMessageCoroutine;
        private Coroutine interactionPromptCoroutine;
        private bool isOxygenWarningActive = false;

        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                // Optionally persist across scenes
                // DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            // Initialize UI elements
            InitializeUIElements();
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes all UI elements to their default states.
        /// </summary>
        private void InitializeUIElements()
        {
            // Initialize Gravity Fuel Slider
            InitializeSlider(gravityFuelSlider, 100f);

            // Initialize Player Laser Slider
            InitializeSlider(playerLaserSlider, 100f);

            // Initialize Vehicle Laser Slider
            InitializeSlider(vehicleLaserSlider, 100f);

            // Initialize Oxygen Slider
            InitializeSlider(oxygenSlider, 100f);

            // Initialize Warnings and Messages
            SetActive(oxygenWarningMessage, false);
            SetActive(gameOverScreen, false);
            SetActive(gameWinScreen, false);
            SetActive(pickupMessageText?.gameObject, false);
            SetActive(interactionPromptText?.gameObject, false);
            SetActive(vehicleHUD, false);

            // Initialize Crystal Count Text
            if (crystalCountText != null)
            {
                crystalCountText.text = "Crystals: 0";
            }
        }

        #endregion

        #region UI Update Methods

        /// <summary>
        /// Updates the gravity fuel bar on the UI.
        /// </summary>
        public void UpdateGravityFuelBar(float current, float max)
        {
            UpdateSlider(gravityFuelSlider, gravityFuelFillImage, current, max, Color.red, Color.green);
        }

        /// <summary>
        /// Updates the player's laser energy bar on the UI.
        /// </summary>
        public void UpdatePlayerLaserBar(float current, float max)
        {
            UpdateSlider(playerLaserSlider, playerLaserFillImage, current, max, Color.red, Color.yellow);
        }

        /// <summary>
        /// Updates the vehicle's laser energy bar on the UI.
        /// </summary>
        public void UpdateVehicleLaserBar(float current, float max)
        {
            UpdateSlider(vehicleLaserSlider, vehicleLaserFillImage, current, max, Color.red, Color.yellow);
        }

        /// <summary>
        /// Updates the oxygen bar on the UI.
        /// </summary>
        public void UpdateOxygenBar(float current, float max)
        {
            UpdateSlider(oxygenSlider, oxygenFillImage, current, max, Color.green, Color.red);
        }

        /// <summary>
        /// Updates the crystal count display on the UI.
        /// </summary>
        public void UpdateCrystalCount(int amount)
        {
            if (crystalCountText != null)
            {
                crystalCountText.text = $"Crystals: {amount}";
            }
        }

        #endregion

        #region UI Display Methods

        /// <summary>
        /// Shows the oxygen warning message.
        /// </summary>
        public void ShowOxygenWarning()
        {
            if (!isOxygenWarningActive)
            {
                isOxygenWarningActive = true;
                ShowBlinkingWarning(oxygenWarningMessage, Color.red, 0.5f);
            }
        }

        /// <summary>
        /// Shows the Game Over screen.
        /// </summary>
        public void ShowGameOverScreen()
        {
            SetActive(gameOverScreen, true);
        }

        /// <summary>
        /// Shows the Victory (Game Win) screen.
        /// </summary>
        public void ShowGameWinScreen()
        {
            SetActive(gameWinScreen, true);
        }

        /// <summary>
        /// Displays a temporary pickup message on the UI.
        /// </summary>
        public void ShowPickupMessage(string message)
        {
            ShowTemporaryMessage(pickupMessageText, message, pickupMessageDuration, ref pickupMessageCoroutine);
        }

        /// <summary>
        /// Displays an interaction prompt on the UI.
        /// </summary>
        public void ShowInteractionPrompt(string prompt)
        {
            if (!string.IsNullOrEmpty(prompt) && interactionPromptText != null)
            {
                interactionPromptText.text = prompt;
                SetActive(interactionPromptText.gameObject, true);
            }
            else if (interactionPromptText != null)
            {
                SetActive(interactionPromptText.gameObject, false);
            }
        }

        /// <summary>
        /// Activates the Vehicle HUD.
        /// </summary>
        public void ActivateVehicleHUD()
        {
            SetActive(vehicleHUD, true);
        }

        /// <summary>
        /// Deactivates the Vehicle HUD.
        /// </summary>
        public void DeactivateVehicleHUD()
        {
            SetActive(vehicleHUD, false);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initializes a slider with a max value.
        /// </summary>
        private void InitializeSlider(Slider slider, float maxValue)
        {
            if (slider != null)
            {
                slider.maxValue = maxValue;
                slider.value = maxValue;
            }
        }

        /// <summary>
        /// Updates a slider's value and color based on the current and maximum values.
        /// </summary>
        private void UpdateSlider(Slider slider, Image fillImage, float current, float max, Color minColor, Color maxColor)
        {
            if (slider != null && fillImage != null)
            {
                slider.maxValue = max;
                slider.value = current;

                // Change the fill color based on percentage
                float percentage = current / max;
                fillImage.color = Color.Lerp(minColor, maxColor, percentage);

                // Trigger warnings if necessary
                if (percentage <= 0.2f)
                {
                    if (slider == oxygenSlider)
                    {
                        ShowOxygenWarning();
                    }
                    // Add additional conditions for other sliders if needed
                }
            }
        }

        /// <summary>
        /// Sets the active state of a GameObject.
        /// </summary>
        private void SetActive(GameObject obj, bool isActive)
        {
            if (obj != null && obj.activeSelf != isActive)
            {
                obj.SetActive(isActive);
            }
        }

        /// <summary>
        /// Displays a temporary message on the UI.
        /// </summary>
        private void ShowTemporaryMessage(TextMeshProUGUI messageText, string message, float duration, ref Coroutine coroutine)
        {
            if (messageText != null)
            {
                if (coroutine != null)
                {
                    StopCoroutine(coroutine);
                }

                messageText.text = message;
                messageText.gameObject.SetActive(true);
                coroutine = StartCoroutine(HideAfterDelay(messageText.gameObject, duration));
            }
        }

        /// <summary>
        /// Starts blinking a warning message.
        /// </summary>
        private void ShowBlinkingWarning(GameObject warningObject, Color blinkColor, float interval)
        {
            if (warningObject != null && !warningObject.activeSelf)
            {
                warningObject.SetActive(true);
                StartCoroutine(BlinkWarning(warningObject, blinkColor, interval));
            }
        }

        /// <summary>
        /// Coroutine to hide a GameObject after a delay.
        /// </summary>
        private IEnumerator HideAfterDelay(GameObject obj, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }

        /// <summary>
        /// Coroutine to handle blinking of warning messages.
        /// </summary>
        private IEnumerator BlinkWarning(GameObject warningObject, Color blinkColor, float interval)
        {
            Image warningImage = warningObject.GetComponent<Image>();
            if (warningImage == null)
            {
                warningImage = warningObject.GetComponentInChildren<Image>();
                if (warningImage == null)
                {
                    yield break; // No Image component found
                }
            }

            while (true)
            {
                warningImage.color = warningImage.color == Color.clear ? blinkColor : Color.clear;
                yield return new WaitForSeconds(interval);
            }
        }

        #endregion
    }
}
