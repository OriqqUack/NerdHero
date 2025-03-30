using UnityEngine;
using System;
using System.Collections;
using System.Data;

public class EnergyManager : MonoSingleton<EnergyManager>, ISaveable
{
    public int maxEnergy = 30;
    public int chargeIntervalMinutes = 10;

    public int CurrentEnergy { get; private set; }
    public TimeSpan PassedTime { get; private set; }
    
    private DateTime lastChargeTime;

    void UpdateEnergyFromOffline()
    {
        if (CurrentEnergy >= maxEnergy) return;

        DateTime now = DateTime.UtcNow.AddHours(9);
        TimeSpan passed = now - lastChargeTime;
        int charges = (int)(passed.TotalMinutes / chargeIntervalMinutes);

        if (charges > 0)
        {
            CurrentEnergy = Mathf.Min(CurrentEnergy + charges, maxEnergy);
            lastChargeTime = lastChargeTime.AddMinutes(charges * chargeIntervalMinutes);
            //DataManager.Instance.DataSave();
        }
    }

    IEnumerator EnergyTick()
    {
        while (true)
        {
            if (CurrentEnergy < maxEnergy)
            {
                DateTime now = DateTime.UtcNow.AddHours(9);
                TimeSpan timeSinceLast = now - lastChargeTime;
                PassedTime = timeSinceLast;
                
                if (timeSinceLast.TotalMinutes >= chargeIntervalMinutes)
                {
                    CurrentEnergy++;
                    lastChargeTime = lastChargeTime.AddMinutes(chargeIntervalMinutes);
                    //DataManager.Instance.DataSave();
                }
            }
            else
            {
                PassedTime = TimeSpan.Zero;
            }

            yield return new WaitForSeconds(1f);
        }
    }

    // 외부에서 에너지 사용할 때 호출
    public bool UseEnergy(int amount)
    {
        if (CurrentEnergy >= amount)
        {
            if(CurrentEnergy == maxEnergy)
                lastChargeTime = DateTime.UtcNow.AddHours(9);
            
            CurrentEnergy -= amount;
            //DataManager.Instance.DataSave();
            return true;
        }
        return false;
    }

    public TimeSpan GetTimeUntilNextCharge()
    {
        if (CurrentEnergy >= maxEnergy) return TimeSpan.Zero;
        return (lastChargeTime.AddMinutes(chargeIntervalMinutes) - DateTime.UtcNow.AddHours(9));
    }

#if UNITY_EDITOR
    public void ClearEnergy()
    {
        CurrentEnergy = maxEnergy;
    }
#endif
    
    public void Save(SaveData data)
    {
        data.energyCount = CurrentEnergy;
        data.energyLastCharge = lastChargeTime.Ticks.ToString();
    }

    public void Load(SaveData data)
    {
        CurrentEnergy = data.energyCount;
        long lastTicks = Convert.ToInt64(data.energyLastCharge);
        lastChargeTime = lastTicks == 0 ? DateTime.UtcNow : new DateTime(lastTicks);

        UpdateEnergyFromOffline();
        StartCoroutine(EnergyTick());
    }
}
