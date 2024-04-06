using UnityEngine;
using UnityEngine.UI;


public class StaminaBar : MonoBehaviour
{
    public Slider slider;
    public Abigail.Movement playerMovement;
    void Update()
    {
        if (playerMovement != null)
        {
            if (slider.maxValue != playerMovement.staminaTotal)
            {
                slider.maxValue = playerMovement.staminaTotal;
            }
        slider.value = playerMovement.stamina;
        }
    }

    public void SetMaxStamina(float stamina) // Changed from int to float
    {
        slider.maxValue = stamina;
        slider.value = stamina;
    }

    public void SetStamina(int stamina)
    {
        slider.value = stamina;
    }    
}
