using UnityEngine;
using System;
using System.Collections;
using System.Data;

public class EnergyManager : ISaveable
{
    public delegate void EnergyChangeDelegate(int energy);
    public event EnergyChangeDelegate OnEnergyChange;
    
    public int maxEnergy = 100;
    public int chargeIntervalMinutes = 1;

    public int CurrentEnergy { get; private set; }
    public TimeSpan PassedTime { get; private set; }
    
    private DateTime lastChargeTime;

    public void Init()
    {
        Managers.DataManager.AddSaveable(this);
    }
    
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
                    OnEnergyChange?.Invoke(CurrentEnergy);
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
            OnEnergyChange?.Invoke(CurrentEnergy);
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
        data.CurrencyData.energyCount = CurrentEnergy;
        data.CurrencyData.energyLastCharge = lastChargeTime.Ticks.ToString();
    }

    public void Load(SaveData data)
    {
        CurrentEnergy = data.CurrencyData.energyCount;
        long lastTicks = 0;
        if (!string.IsNullOrEmpty(data.CurrencyData.energyLastCharge))
        {
            bool success = long.TryParse(data.CurrencyData.energyLastCharge, out lastTicks);
            if (!success)
            {
                Debug.LogWarning("EnergyManager: 저장된 lastChargeTime 포맷 이상. 기본값으로 초기화.");
                lastTicks = 0;
            }
        }

        lastChargeTime = lastTicks == 0 ? DateTime.UtcNow : new DateTime(lastTicks);

        UpdateEnergyFromOffline();
        Managers.Instance.StartManagedCoroutine(EnergyTick());
    }
}
