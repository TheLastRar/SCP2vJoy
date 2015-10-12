using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ScpPad2vJoy.VjoyEffect
{
    class FFBDevice
    {
        public enum DeviceState
        {
            Deactivated,
            AwaitingDeactivation,
            Active
        }
        private const Byte MAX_BLOCKS = 255;

        public volatile DeviceState DeviceActive = DeviceState.Deactivated;
        public Dictionary<Byte, BaseEffectBlock> EffectBlocks = new Dictionary<Byte, BaseEffectBlock>();
        public volatile Single DeviceGain = 1.0F;
        public volatile Boolean DevicePaused = false;
        public Boolean[] LoadedBlocks = new Boolean[MAX_BLOCKS];
        //Block ID Managment
        #region "BlockManagement"
        public void AddBlock(Byte index, BaseEffectBlock anObj)
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
        public void RemoveBlock(Byte index)
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
            LoadedBlocks = new Boolean[MAX_BLOCKS];
            LoadedBlocks[0] = true;
        }
        public byte NextKey()
        {
            Trace.WriteLine("Hack, Setting Block to 1");
            return 1;
            Byte newKey = 0;
            for (Byte i = 1; i < MAX_BLOCKS; i++)
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
