using UnityEngine;
using System;

public class GrowthValidator
{
    public struct ValidationResult
    {
        public bool IsValid;
        public string Message;
    }

    public static ValidationResult ValidateGrowthState(GrowingBase plant)
    {
        if (plant.currentState == null)
            return new ValidationResult { IsValid = false, Message = "Invalid growth state: null reference" };

        if (plant.DiasPlantado < 0)
            return new ValidationResult { IsValid = false, Message = "Invalid day count: negative days" };

        if (plant.DaysDry >= plant.MaxDaysDry)
            return new ValidationResult { IsValid = false, Message = "Plant died from lack of water" };

        if (plant.DaysWithoutHarvest >= plant.MaxDaysWithoutHarvest)
            return new ValidationResult { IsValid = false, Message = "Plant died from not being harvested" };

        return new ValidationResult { IsValid = true, Message = "Valid growth state" };
    }

    public static ValidationResult ValidateComponents(GrowingBase plant)
    {
        if (plant.meshFilter == null)
            return new ValidationResult { IsValid = false, Message = "Missing MeshFilter component" };

        if (plant.meshRenderer == null)
            return new ValidationResult { IsValid = false, Message = "Missing MeshRenderer component" };

        if (!plant.isFruit && plant.meshCollider == null)
            return new ValidationResult { IsValid = false, Message = "Missing MeshCollider component" };

        return new ValidationResult { IsValid = true, Message = "Valid component setup" };
    }

    public static void HandleFailedValidation(GrowingBase plant, ValidationResult result)
    {
        Debug.LogError($"Growth validation failed for {plant.name}: {result.Message}");
        
        // Try to recover mesh state if possible
        if (plant.currentState != null)
        {
            if (plant.currentState.mesh != null && plant.meshFilter != null)
                plant.meshFilter.mesh = plant.currentState.mesh;
                
            if (plant.currentState.material != null && plant.meshRenderer != null)
                plant.meshRenderer.material = plant.currentState.material;
        }
    }
}