using System.Collections.Generic;

namespace lsusbnet
{
    public enum DeviceType
    {
        PER_INTERFACE = 0,         /* for DeviceClass */
        AUDIO = 1,
        COMM = 2,
        HID = 3,
        IMAGING = 6,
        PRINTER = 7,
        MASS_STORAGE = 8,
        HUB = 9,
        DATA = 0xa,
        SMARTCARD = 0xb,
        VIDEO = 0xe,
        WIRELESS = 0xe0,
        VENDOR_SPEC = 0xff,
    }
    public class Blockdevice
    {
        public string Model = "";
        public string Name = "";
        public string Fstype = "";
        public string Size = "";
        public string Label = "";
        public string Mountpoint = "";
        public string Serial = "";
        public string Path = "";


        public List<Blockdevice> Children = new List<Blockdevice>();

        public string GetMountPath(int cid)
        {
            if (Children.Count <= cid)
                return null;

            return Children[cid].Path;
        }
    }
    public class UsbDevice
    {
        public string idVendor = "";
        public string iManufacturer = "";
        public string idProduct = "";
        public string iProduct = "";
        public string iSerial = "";
        public string iDevClass = "";
        public string bcdUSB = "";
        public string busnum = "";
        public string devnum = "";
        public string devpath = "";
        public string dType = "";
        public string DeviceString => idVendor.Trim() + ":" + idProduct.Trim();//USB TYPE

        public DeviceType Type = DeviceType.VIDEO;
        public Blockdevice StorageInfo = null;

        public override string ToString()
        {
            return idProduct + "_" + iProduct + "_" + DeviceString + "_" + iSerial + "_";
        }

        public string GetDevName()
        {

            if (idProduct != null && idProduct.Length > 1)
                return idProduct;

            if (iManufacturer != null && iManufacturer.Length > 1)
                return iManufacturer;

            return "Unknown";
        }

        public bool ValidDevice()
        {
            return !string.IsNullOrEmpty(idVendor) && !string.IsNullOrEmpty(devpath);
        }

        public void ParseClass(string tr)
        {
            dType = tr;
            switch (tr)
            {
                case "00":
                    Type = DeviceType.PER_INTERFACE;
                    break;
                case "01":
                    Type = DeviceType.AUDIO;
                    break;
                case "02":
                    Type = DeviceType.COMM;
                    break;
                case "03":
                    Type = DeviceType.HID;
                    break;
                case "06":
                    Type = DeviceType.IMAGING;
                    break;
                case "07":
                    Type = DeviceType.PRINTER;
                    break;
                case "08":
                    Type = DeviceType.MASS_STORAGE;
                    break;
                case "09":
                    Type = DeviceType.HUB;
                    break;
                case "0a":
                    Type = DeviceType.DATA;
                    break;
                case "0b":
                    Type = DeviceType.SMARTCARD;
                    break;
                case "0e":
                    Type = DeviceType.VIDEO;
                    break;
                case "0e0":
                    Type = DeviceType.WIRELESS;
                    break;
                case "0ff":
                    Type = DeviceType.VENDOR_SPEC;
                    break;
            }

        }
    }
}
