using System.Collections.Generic;

namespace lsusbnet
{
    public class StorageDevices
    {

        public List<Blockdevice> Blockdevices = new List<Blockdevice>(); 

        public Blockdevice GetDevice(string serialId)
        {
            Blockdevice r = null;
            foreach (Blockdevice d in Blockdevices.ToArray())
            {
               if(serialId == d.Serial)
                {
                    r = d;
                    break;
                }
            }
            return r;
        }

        public Blockdevice GetDeviceByModel(string modelName)
        {
            Blockdevice r = null;
            foreach (Blockdevice d in Blockdevices.ToArray())
            {
                if (modelName == d.Model)
                {
                    r = d;
                    break;
                }
            }
            return r;
        }
    }
}
