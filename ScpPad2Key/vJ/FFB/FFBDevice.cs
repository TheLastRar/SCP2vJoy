using ScpPad2vJoy.vJ.FFB.Effect;
using System.Collections.Generic;
using System.Diagnostics;

namespace ScpPad2vJoy.vJ.FFB
{
    class FFBDevice
    {
        public enum DeviceState
        {
            Deactivated,
            AwaitingDeactivation,
            Active
        }
        private const byte MAX_BLOCKS = 255;//(2^7) - 1; //7bits for EBI

        public volatile DeviceState DeviceActive = DeviceState.Deactivated;
        public Dictionary<byte, BaseEffectBlock> EffectBlocks = new Dictionary<byte, BaseEffectBlock>();
        public volatile float DeviceGain = 1.0F;
        public volatile bool DevicePaused = false;
        public bool[] LoadedBlocks = new bool[MAX_BLOCKS];
        //Block ID Managment
        #region "BlockManagement"
        public void AddBlock(byte index, BaseEffectBlock anObj)
        {
            if (EffectBlocks.ContainsKey(1))
            {
                Trace.WriteLine("HACK, EFFECT OVERWRITTEN");
                EffectBlocks.Remove(1);
            }
            LoadedBlocks[index] = true;
            EffectBlocks.Add(index, anObj);
        }
        //public int AddNext(EffectBlock anObj)
        //{
        //    Byte newKey = 0;
        //    for (Byte i = 1; i < 255; i++)
        //    {
        //        if (!LoadedBlocks[i])
        //        {
        //            newKey = i;
        //            break;
        //        }
        //    }
        //    if (newKey > 0)
        //    {
        //        LoadedBlocks[newKey] = true;
        //        EffectBlocks.Add(newKey, anObj);
        //    }
        //    return newKey;
        //}
        public void RemoveBlock(byte index)
        {
            if (EffectBlocks.ContainsKey((byte)index))
            {
                EffectBlocks.Remove((byte)index);
            }
            LoadedBlocks[index] = false;
        }
        public void ClearBlocks()
        {
            EffectBlocks.Clear();
            LoadedBlocks = new bool[MAX_BLOCKS];
            LoadedBlocks[0] = true; //Never use 0 as it is for failed allocations
        }
        public byte NextKey()
        {
            Trace.WriteLine("Hack, Setting Block to 1");
            return 1;
            byte newKey = 0;
            for (byte i = 1; i < MAX_BLOCKS; i++)
            {
                if (!LoadedBlocks[i])
                {
                    newKey = i;
                    break;
                }
            }
            return newKey;
        }
        #endregion
    }
}
