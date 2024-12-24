using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace Systems.UISystems
{
    /// <summary>
    /// Manages the HUD elements, including sliders and warning messages.
    /// </summary>
    public class HUDController : MonoBehaviour
    {
        [System.Serializable]
        public class HUDSlider
        {
            public Slider slider;                  // Reference to the Slider component
            public TextMeshProUGUI label;         // Label for the slider (optional)
        }

        #region Inspector Variables

        [Header("HUD Sliders")]
        public HUDSlider oxygenSlider;
        public HUDSlider laserEnergySlider;
        public HUDSlider gravityFuelSlider;

        [Header("Warning Messages")]
        public TextMeshProUGUI lowOxygenWarning;
        public TextMeshProUGUI lowLaserEnergyWarning;
        public TextMeshProUGUI lowGravityFuelWarning;

        [Header("Blink Settings")]
        public float blinkInterval = 0.5f;          // Time between blinks
        public Color warningColor = Color.red;       // Color for warnings

        #endregion

        #region Private Variables

        private bool isOxygenLow = false;
        private bool isLaserEnergyLow = false;
        private bool isGravityFuelLow = false;

        private Coroutine oxygenBlinkCoroutine;
        private Coroutine laserEnergyBlinkCoroutine;
        private Coroutine gravityFuelBlinkCoroutine;

        #endregion

        #region Unity Callbacks

        private void Start()
        {
            // Initialize warnings as inactive
            lowOxygenWarning.gameObject.SetActive(false);
            lowLaserEnergyWarning.gameObject.SetActive(false);
            lowGravityFuelWarning.gameObject.SetActive(false);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Updates the Oxygen slider.
        /// </summary>
        public void UpdateOxygenBar(float current, float max)
        {
            UpdateSlider(oxygenSlider, current, max);
            CheckLowOxygen(current, max);
        }

        /// <summary>
        /// Updates the Player Laser Energy slider.
        /// </summary>
        public void UpdatePlayerLaserBar(float current, float max)
        {
            UpdateSlider(laserEnergySlider, current, max);
            CheckLowLaserEnergy(current, max);
        }

        /// <summary>
        /// Updates the Vehicle Laser Energy slider.
        /// </summary>
        public void UpdateVehicleLaserBar(float current, float max)
        {
            UpdateSlider(gravityFuelSlider, current, max);
            // Implement if vehicle laser has separate warnings
        }

        /// <summary>
        /// Updates the Gravity Fuel slider.
        /// </summary>
        public void UpdateGravityFuel(float current, float max)
        {
            UpdateSlider(gravityFuelSlider, current, max);
            CheckLowGravityFuel(current, max);
        }

        /// <summary>
        /// Updates the crystal count display on the UI.
        /// </summary>
        public void UpdateCrystalCount(int amount)
        {
            if (oxygenSlider.label != null)
            {
                // Assuming oxygenSlider.label is used for crystals, adjust accordingly
                oxygenSlider.label.text = $"Crystals: {amount}";
            }
        }

        /// <summary>
        /// Shows the oxygen warning message.
        /// </summary>
        public void ShowOxygenWarning()
        {
            if (!isOxygenLow)
            {
                isOxygenLow = true;
                StartBlinkingWarning(lowOxygenWarning, ref oxygenBlinkCoroutine);
            }
        }

        /// <summary>
        /// Shows the Game Over screen.
        /// </summary>
        public void ShowGameOverScreen()
        {
            // Implement as needed
        }

        /// <summary>
        /// Shows the Victory (Game Win) screen.
        /// </summary>
        public void ShowGameWinScreen()
        {
            // Implement as needed
        }

        /// <summary>
        /// Displays a temporary pickup message on the UI.
        /// </summary>
        public void ShowPickupMessage(string message)
        {
            // Implement as needed
        }

        /// <summary>
        /// Displays an interaction prompt on the UI.
        /// </summary>
        public void ShowInteractionPrompt(string prompt)
        {
            // Implement as needed
        }

        /// <summary>
        /// Activates the Vehicle HUD.
        /// </summary>
        public void ActivateVehicleHUD()
        {
            // Implement as needed
        }

        /// <summary>
        /// Deactivates the Vehicle HUD.
        /// </summary>
        public void DeactivateVehicleHUD()
        {
            // Implement as needed
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Updates a slider's value based on the current and maximum values.
        /// </summary>
        private void UpdateSlider(HUDSlider slider, float current, float max)
        {
            if (slider.slider != null)
            {
                slider.slider.maxValue = max;
                slider.slider.value = current;
            }
        }

        /// <summary>
        /// Checks and handles low oxygen warning.
        /// </summary>
        private void CheckLowOxygen(float current, float max)
        {
            if (current / max <= 0.2f && !isOxygenLow)
            {
                isOxygenLow = true;
                StartBlinkingWarning(lowOxygenWarning, ref oxygenBlinkCoroutine);
            }
            else if (current / max > 0.2f && isOxygenLow)
            {
                isOxygenLow = false;
                StopBlinkingWarning(lowOxygenWarning, ref oxygenBlinkCoroutine);
            }
        }

        /// <summary>
        /// Checks and handles low laser energy warning.
        /// </summary>
        private void CheckLowLaserEnergy(float current, float max)
        {
            if (current / max <= 0.2f && !isLaserEnergyLow)
            {
                isLaserEnergyLow = true;
                StartBlinkingWarning(lowLaserEnergyWarning, ref laserEnergyBlinkCoroutine);
            }
            else if (current / max > 0.2f && isLaserEnergyLow)
            {
                isLaserEnergyLow = false;
                StopBlinkingWarning(lowLaserEnergyWarning, ref laserEnergyBlinkCoroutine);
            }
        }

        /// <summary>
        /// Checks and handles low gravity fuel warning.
        /// </summary>
        private void CheckLowGravityFuel(float current, float max)
        {
            if (current / max <= 0.2f && !isGravityFuelLow)
            {
                isGravityFuelLow = true;
                StartBlinkingWarning(lowGravityFuelWarning, ref gravityFuelBlinkCoroutine);
            }
            else if (current / max > 0.2f && isGravityFuelLow)
            {
                isGravityFuelLow = false;
                StopBlinkingWarning(lowGravityFuelWarning, ref gravityFuelBlinkCoroutine);
            }
        }

        /// <summary>
        /// Starts blinking a warning message.
        /// </summary>
        private void StartBlinkingWarning(TextMeshProUGUI warningText, ref Coroutine coroutine)
        {
            if (warningText != null && coroutine == null)
            {
                coroutine = StartCoroutine(BlinkWarning(warningText));
            }
        }

        /// <summary>
        /// Stops blinking a warning message.
        /// </summary>
        private void StopBlinkingWarning(TextMeshProUGUI warningText, ref Coroutine coroutine)
        {
            if (warningText != null && coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
                warningText.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Coroutine to handle blinking of warning messages.
        /// </summary>
        private IEnumerator BlinkWarning(TextMeshProUGUI warningText)
        {
            while (true)
            {
                warningText.color = warningText.color == warningColor ? Color.clear : warningColor;
                yield return new WaitForSeconds(blinkInterval);
            }
        }

        #endregion
    }
}
