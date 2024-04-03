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
            slider.maxValue = playerMovement.staminaTotal;
            slider.value = playerMovement.stamina;
        }
    }

    public void SetMaxStamina(int stamina)
    {
        slider.maxValue = stamina;
        slider.value = stamina;
    }

    public void SetStamina(int stamina)
    {
        slider.value = stamina;
    }    
}
