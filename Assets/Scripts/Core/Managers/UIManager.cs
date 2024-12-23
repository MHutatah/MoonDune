using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace Core.Managers
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;

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
        [SerializeField] private GameObject gameOverScreen;          // UI element for Game Over
        [SerializeField] private GameObject gameWinScreen;           // UI element for Victory

        [Header("Pickup Message UI")]
        [SerializeField] private TextMeshProUGUI pickupMessageText;  // Text element for pickup messages
        [SerializeField] private float pickupMessageDuration = 2f;   // Duration the pickup message is displayed

        [Header("Interaction Prompt UI")]
        [SerializeField] private TextMeshProUGUI interactionPromptText; // Text element for interaction prompts
        [SerializeField] public string enterVehiclePrompt = "Press E to enter vehicle"; // Made public for access
        [SerializeField] public string exitVehiclePrompt = "Press E to exit vehicle";   // Made public for access

        [Header("Vehicle HUD UI")]
        [SerializeField] private GameObject vehicleHUD;              // Reference to the Vehicle HUD UI GameObject

        private Coroutine pickupMessageCoroutine;
        private Coroutine interactionPromptCoroutine;

        private void Awake()
        {
            // Implement Singleton pattern
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

        /// <summary>
        /// Initializes all UI elements to their default states.
        /// </summary>
        private void InitializeUIElements()
        {
            // Initialize Gravity Fuel Slider
            if (gravityFuelSlider != null)
            {
                gravityFuelSlider.maxValue = 100f; // Default max value, to be updated by script
                gravityFuelSlider.value = gravityFuelSlider.maxValue;
            }

            // Initialize Player Laser Slider
            if (playerLaserSlider != null)
            {
                playerLaserSlider.maxValue = 100f; // Default max value, to be updated by script
                playerLaserSlider.value = playerLaserSlider.maxValue;
            }

            // Initialize Vehicle Laser Slider
            if (vehicleLaserSlider != null)
            {
                vehicleLaserSlider.maxValue = 100f; // Default max value, to be updated by script
                vehicleLaserSlider.value = vehicleLaserSlider.maxValue;
            }

            // Initialize Oxygen Slider
            if (oxygenSlider != null)
            {
                oxygenSlider.maxValue = 100f; // Default max value, to be updated by script
                oxygenSlider.value = oxygenSlider.maxValue;
            }

            // Initialize Insufficient Fuel Message
            if (oxygenWarningMessage != null)
            {
                oxygenWarningMessage.SetActive(false);
            }

            // Initialize Crystal Count Text
            if (crystalCountText != null)
            {
                crystalCountText.text = "Crystals: 0";
            }

            // Initialize Game Over and Victory Screens
            if (gameOverScreen != null)
            {
                gameOverScreen.SetActive(false);
            }

            if (gameWinScreen != null)
            {
                gameWinScreen.SetActive(false);
            }

            // Initialize Pickup Message Text
            if (pickupMessageText != null)
            {
                pickupMessageText.gameObject.SetActive(false);
            }

            // Initialize Interaction Prompt Text
            if (interactionPromptText != null)
            {
                interactionPromptText.gameObject.SetActive(false);
            }

            // Initialize Vehicle HUD
            if (vehicleHUD != null)
            {
                vehicleHUD.SetActive(false); // Ensure HUD is inactive at start
            }
        }

        /// <summary>
        /// Updates the gravity fuel bar on the UI.
        /// </summary>
        /// <param name="current">Current gravity fuel.</param>
        /// <param name="max">Maximum gravity fuel.</param>
        public void UpdateGravityFuelBar(float current, float max)
        {
            if (gravityFuelSlider != null && gravityFuelFillImage != null)
            {
                gravityFuelSlider.maxValue = max;
                gravityFuelSlider.value = current;

                // Change the fill color based on fuel percentage
                float fuelPercentage = current / max;
                gravityFuelFillImage.color = Color.Lerp(Color.red, Color.green, fuelPercentage);
            }
        }

        /// <summary>
        /// Updates the player's laser energy bar on the UI.
        /// </summary>
        /// <param name="current">Current laser energy.</param>
        /// <param name="max">Maximum laser energy.</param>
        public void UpdatePlayerLaserBar(float current, float max)
        {
            if (playerLaserSlider != null && playerLaserFillImage != null)
            {
                playerLaserSlider.maxValue = max;
                playerLaserSlider.value = current;

                // Change the fill color based on energy percentage
                float energyPercentage = current / max;
                playerLaserFillImage.color = Color.Lerp(Color.red, Color.yellow, energyPercentage);
            }
        }

        /// <summary>
        /// Updates the vehicle's laser energy bar on the UI.
        /// </summary>
        /// <param name="current">Current laser energy.</param>
        /// <param name="max">Maximum laser energy.</param>
        public void UpdateVehicleLaserBar(float current, float max)
        {
            if (vehicleLaserSlider != null && vehicleLaserFillImage != null)
            {
                vehicleLaserSlider.maxValue = max;
                vehicleLaserSlider.value = current;

                // Change the fill color based on energy percentage
                float energyPercentage = current / max;
                vehicleLaserFillImage.color = Color.Lerp(Color.red, Color.yellow, energyPercentage);
            }
        }

        /// <summary>
        /// Updates the oxygen bar on the UI.
        /// </summary>
        /// <param name="current">Current oxygen level.</param>
        /// <param name="max">Maximum oxygen level.</param>
        public void UpdateOxygenBar(float current, float max)
        {
            if (oxygenSlider != null && oxygenFillImage != null)
            {
                oxygenSlider.maxValue = max;
                oxygenSlider.value = current;

                // Change the fill color based on oxygen percentage
                float oxygenPercentage = current / max;
                oxygenFillImage.color = Color.Lerp(Color.green, Color.red, 1f - oxygenPercentage);
            }
        }

        /// <summary>
        /// Shows the oxygen warning message.
        /// </summary>
        public void ShowOxygenWarning()
        {
            if (oxygenWarningMessage != null && !oxygenWarningMessage.activeSelf)
            {
                oxygenWarningMessage.SetActive(true);
                StartCoroutine(HideOxygenWarningAfterDelay());
            }
        }

        /// <summary>
        /// Coroutine to hide the oxygen warning after a delay.
        /// </summary>
        private IEnumerator HideOxygenWarningAfterDelay()
        {
            yield return new WaitForSeconds(2f); // Duration of the warning
            if (oxygenWarningMessage != null)
            {
                oxygenWarningMessage.SetActive(false);
            }
        }

        /// <summary>
        /// Updates the crystal count display on the UI.
        /// </summary>
        /// <param name="amount">Number of crystals collected.</param>
        public void UpdateCrystalCount(int amount)
        {
            if (crystalCountText != null)
            {
                crystalCountText.text = $"Crystals: {amount}";
            }
        }

        /// <summary>
        /// Shows the Game Over screen.
        /// </summary>
        public void ShowGameOverScreen()
        {
            if (gameOverScreen != null)
            {
                gameOverScreen.SetActive(true);
            }
        }

        /// <summary>
        /// Shows the Victory (Game Win) screen.
        /// </summary>
        public void ShowGameWinScreen()
        {
            if (gameWinScreen != null)
            {
                gameWinScreen.SetActive(true);
            }
        }

        /// <summary>
        /// Displays a temporary pickup message on the UI.
        /// </summary>
        /// <param name="message">The message to display.</param>
        public void ShowPickupMessage(string message)
        {
            if (pickupMessageText != null)
            {
                // If a message is already being displayed, stop it
                if (pickupMessageCoroutine != null)
                {
                    StopCoroutine(pickupMessageCoroutine);
                }

                pickupMessageText.text = message;
                pickupMessageText.gameObject.SetActive(true);
                pickupMessageCoroutine = StartCoroutine(HidePickupMessageAfterDelay());
            }
        }

        /// <summary>
        /// Coroutine to hide the pickup message after a delay.
        /// </summary>
        private IEnumerator HidePickupMessageAfterDelay()
        {
            yield return new WaitForSeconds(pickupMessageDuration);
            if (pickupMessageText != null)
            {
                pickupMessageText.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Displays an interaction prompt on the UI.
        /// </summary>
        /// <param name="prompt">The prompt message to display.</param>
        public void ShowInteractionPrompt(string prompt)
        {
            if (interactionPromptText != null)
            {
                // If a prompt is already being displayed, stop it
                if (interactionPromptCoroutine != null)
                {
                    StopCoroutine(interactionPromptCoroutine);
                }

                if (!string.IsNullOrEmpty(prompt))
                {
                    interactionPromptText.text = prompt;
                    interactionPromptText.gameObject.SetActive(true);
                    interactionPromptCoroutine = StartCoroutine(HideInteractionPromptAfterDelay());
                }
                else
                {
                    interactionPromptText.gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Coroutine to hide the interaction prompt after a short delay.
        /// </summary>
        private IEnumerator HideInteractionPromptAfterDelay()
        {
            yield return new WaitForSeconds(0.5f); // Adjust as needed
            if (interactionPromptText != null)
            {
                interactionPromptText.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Activates the Vehicle HUD.
        /// </summary>
        public void ActivateVehicleHUD()
        {
            if (vehicleHUD != null)
            {
                vehicleHUD.SetActive(true);
            }
        }

        /// <summary>
        /// Deactivates the Vehicle HUD.
        /// </summary>
        public void DeactivateVehicleHUD()
        {
            if (vehicleHUD != null)
            {
                vehicleHUD.SetActive(false);
            }
        }
    }
}