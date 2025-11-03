using System;
using DragonPlus;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;

namespace Screw.UserData
{
    public class EnergyData : Manager<EnergyData>,IRunOnce
    {
        private StorageScrew storageScrew
        {
            get
            {
                return  StorageManager.Instance.GetStorage<StorageScrew>();
            }
        }
        
        private StorageEnergy storageEnergy
        {
            get
            {
                return storageScrew.Energy;
            }
        }

        public bool EnterGameEnergyInfiniteState => false;
        
        public void OnRunOnce()
        {
            RunOnce.OnRunOnce("EnergyData_RunOnceKey", ExecutionRunOnce);
        }
        
        private void ExecutionRunOnce()
        {
            AddEnergy(DragonPlus.Config.Screw.GameConfigManager.Instance.TableGlobalList[0].MaxUserEnergy, new GameBIManager.ItemChangeReasonArgs(){});
        }
        
        public int GetEnergy()
        {
            return UserData.Instance.GetRes(ResType.Energy);
        }

        public void CostEnergy(int count, GameBIManager.ItemChangeReasonArgs reason)
        {
            if (IsInfiniteEnergy())
                return;
            
            var oldEnergyFull = IsEnergyFull();

            if (oldEnergyFull && !IsEnergyFull())
            {
                StorageAutoAddEnergyTime((long)APIManager.Instance.GetServerTime());
            }
            UserData.Instance.ConsumeRes(ResType.Energy, count, reason);
        }

        public void AddEnergy(int count, GameBIManager.ItemChangeReasonArgs reason)
        {
            if (IsInfiniteEnergy())
                return;
            
            var energyCount = GetEnergy();
            
            var oldEnergyFull = IsEnergyFull();

            if (energyCount + count >= GetMaxEnergy())
            {
                count = GetMaxEnergy() - energyCount;
            }
            
            if(count == 0)
                return;
            
            if (oldEnergyFull && !IsEnergyFull())
                StorageAutoAddEnergyTime((long)APIManager.Instance.GetServerTime());
            
            UserData.Instance.AddRes(ResType.Energy, count, reason);
        }
        
        public long GetEnergyInfinityLeftTime()
        {
            long leftTime = 0;
            if (storageScrew.Buff != null && storageScrew.Buff.ContainsKey((int)ResType.EnergyInfinity))
            {
                leftTime = storageScrew.Buff[(int)ResType.EnergyInfinity] * 1000 - (long)APIManager.Instance.GetServerTime();
            }
            return leftTime > 0 ? leftTime : 0;
        }

        public long GetBuffLeftTime(ResType type)
        {
            long leftTime = 0;
            if (storageScrew.Buff != null && storageScrew.Buff.ContainsKey((int)type))
            {
                leftTime = storageScrew.Buff[(int)type] * 1000 - (long)APIManager.Instance.GetServerTime();
            }
            return leftTime > 0 ? leftTime : 0;
        }
        
        public void AddInfinityLeftTime(int infinityTime) //秒
        {
            int type = (int)ResType.EnergyInfinity;
            var leftTime = GetEnergyInfinityLeftTime()/(long)XUtility.Second;
            if (storageScrew.Buff.ContainsKey(type))
            {
                if (leftTime > 0)
                {
                    storageScrew.Buff[type] += infinityTime;
                }
                else
                {
                    storageScrew.Buff[type] = (long)((leftTime + infinityTime) + (long)APIManager.Instance.GetServerTime()/(long)XUtility.Second);
                }
            }
            else
            {
                storageScrew.Buff.Add(type, (long)((leftTime + infinityTime) + (long)APIManager.Instance.GetServerTime()/(long)XUtility.Second));
            }
        }
        public int GetMaxEnergy()
        {
            var now = (long)APIManager.Instance.GetServerTime();
            if (now < storageEnergy.MaxEnergyEndTime)
            {
                return storageEnergy.MaxEnergy;
            }
            return DragonPlus.Config.Screw.GameConfigManager.Instance.TableGlobalList[0].MaxUserEnergy;
        }
        
        public bool IsEnergyFull()
        {
            return GetEnergy() >= GetMaxEnergy();
        }
        
        public bool IsEnergyEmpty()
        {
            return GetEnergy() <= 0 && !IsInfiniteEnergy();
        }
        
        public bool IsInfiniteEnergy()
        {
            return GetEnergyInfinityLeftTime() > 0;
        }
        
        public void Update()
        {
            if (!IsEnergyFull() && IsTimeToAutoAddEnergy())
            {
                AutoAddEnergy();
            }
        }
        
        void AutoAddEnergy()
        {
            long currentTime = (long)APIManager.Instance.GetServerTime();
            long intervalTime = currentTime - storageEnergy.LastAddEnergyTime;
            long addEnergyInterval = DragonPlus.Config.Screw.GameConfigManager.Instance.TableGlobalList[0].EnergyRefillTime * 1000;
            long n = intervalTime / addEnergyInterval;
            long leftTime = intervalTime - addEnergyInterval * n;
            if (n > 0)
            {
                long addNum = DragonPlus.Config.Screw.GameConfigManager.Instance.TableGlobalList[0].EnergyRefillAmount * n;
                if (addNum + GetEnergy() >= GetMaxEnergy())
                {
                    addNum = GetMaxEnergy() - GetEnergy();
                    StorageAutoAddEnergyTime(currentTime);
                }
                else
                {
                    StorageAutoAddEnergyTime(currentTime - leftTime);
                }
                
                AddEnergy((int)addNum, new GameBIManager.ItemChangeReasonArgs(){});
            }
        }
        void StorageAutoAddEnergyTime(long timeStamp)
        {
            storageEnergy.LastAddEnergyTime = timeStamp;
        }
        
        bool IsTimeToAutoAddEnergy()
        {
            return storageEnergy.LastAddEnergyTime >= 0L && LeftAutoAddEnergyTime() <= 0L;
        }
        
        public long LeftAutoAddEnergyTime()
        {
            long leftTime = storageEnergy.LastAddEnergyTime + DragonPlus.Config.Screw.GameConfigManager.Instance.TableGlobalList[0].EnergyRefillTime * 1000 - (long)APIManager.Instance.GetServerTime();
            return leftTime < 0L ? 0L : leftTime;
        }
    }
}