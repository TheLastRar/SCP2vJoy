using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace ScpPad2vJoy.VjoyEffect
{
    class FFBDevice
    {
        public volatile Boolean DeviceActive = false;
        public Dictionary<Byte, BaseEffectBlock> EffectBlocks = new Dictionary<Byte, BaseEffectBlock>();
        public volatile Single DeviceGain = 1.0F;
        public volatile Boolean DevicePaused = false;
        public Boolean[] LoadedBlocks = new Boolean[255];
        public Stopwatch timer = new Stopwatch();
        //Block ID Managment
        #region "BlockManagement"
        public void AddBlock(Byte index, BaseEffectBlock anObj)
        {
            Trace.WriteLine("Hack, Block at 1");
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
            LoadedBlocks = new Boolean[255];
            LoadedBlocks[0] = true;
        }
        public byte NextKey()
        {
            Trace.WriteLine("Hack, Setting Block to 1");
            return 1;
            Byte newKey = 0;
            for (Byte i = 1; i < 255; i++)
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
