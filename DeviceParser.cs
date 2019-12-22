using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;

namespace lsusbnet
{

    public class DeviceParser
    {
        bool LocalTest = false;
        public List<UsbDevice> devices = new List<UsbDevice>();
        StorageDevices storageDevices = new StorageDevices();
        public void GetDeviceList(bool IsGetStorage = false)
        {
            string output = "";
            string[] filePaths;
            if (LocalTest)
            {
                output = File.ReadAllText(@"C:\Users\IcyTeck\source\repos\BlockBox\Tests\out\TTTT");
                filePaths = Directory.GetDirectories(@"C:\Users\IcyTeck\source\repos\lsusbnet\lsusbnet\test");
            }
            else
            {
                if(IsGetStorage)
                    output = ExecuteComand("lsblk -J -m -o model,name,fstype,size,label,mountpoint,serial,path");
                filePaths = Directory.GetDirectories("/sys/bus/usb/devices");
            }
            if (IsGetStorage)
                storageDevices = JsonConvert.DeserializeObject<StorageDevices>(output);
            else
                storageDevices = new StorageDevices();

            foreach (string str in filePaths)
            {
                var usb = GetDevice(str);
                if (usb.ValidDevice())
                    devices.Add(usb);
            }

        }
        UsbDevice GetDevice(string path)
        {
            var data = new UsbDevice();
            string[] filePaths = Directory.GetFiles(path);
            if (filePaths == null || filePaths.Length < 8)
                return data;

            foreach (string str in filePaths)
            {
                var file = Path.GetFileNameWithoutExtension(str).ToLower();
                switch (file)
                {
                    case "busnum":
                        data.busnum = File.ReadAllText(str).Trim();
                        break;
                    case "devnum":
                        data.devnum = File.ReadAllText(str).Trim();
                        break;
                    case "idvendor":
                        data.idVendor = File.ReadAllText(str).Trim();
                        break;
                    case "idproduct":
                        data.idProduct = File.ReadAllText(str).Trim();
                        break;
                    case "manufacturer":
                        data.iManufacturer = File.ReadAllText(str).Trim();
                        break;
                    case "product":
                        data.iProduct = File.ReadAllText(str).Trim();
                        break;
                    case "serial":
                        data.iSerial = File.ReadAllText(str).Trim();
                        break;
                    case "version":
                        data.bcdUSB = File.ReadAllText(str).Trim();
                        break;
                    case "devpath":
                        data.devpath = File.ReadAllText(str).Trim();
                        break;
                    case "bdeviceclass":
                        var x = File.ReadAllText(str).Trim();
                        switch (x)
                        {
                            case "00": x = ">ifc"; break;
                            case "01": x = "audio"; break;
                            case "02": x = "commc"; break;
                            case "03": x = "HID"; break;
                            case "05": x = "PID"; break;
                            case "06": x = "still"; break;
                            case "07": x = "print"; break;
                            case "08": x = "stor."; break;
                            case "09": x = "hub"; break;
                            case "0a": x = "data"; break;
                            case "0b": x = "scard"; break;
                            case "0d": x = "c-sec"; break;
                            case "0e": x = "video"; break;
                            case "0f": x = "perhc"; break;
                            case "dc": x = "diagd"; break;
                            case "e0": x = "wlcon"; break;
                            case "ef": x = "misc"; break;
                            case "fe": x = "app."; break;
                            case "ff": x = "vend."; break;
                            case "*": x = "unk."; break;
                        }
                        data.iDevClass = x;
                        break;

                }

            }
            if (data.ValidDevice())
            {

                string x1 = path + "/" + data.busnum + "-" + data.devpath + ":1.0/bInterfaceClass";
                if (LocalTest)
                {
                    x1 = path + "/test/bInterfaceClass";
                }
                string x2 = path + "/" + data.busnum + "-" + data.devpath + ":2.0/bInterfaceClass";
                string x3 = path + "/" + data.busnum + "-" + data.devpath + ":3.0/bInterfaceClass";
                if (File.Exists(x1))
                {
                    var read = File.ReadAllText(x1).Trim();
                    data.ParseClass(read);
                }
                else if (File.Exists(x2))
                {
                    var read = File.ReadAllText(x2).Trim();
                    data.ParseClass(read);
                }
                else if (File.Exists(x3))
                {
                    var read = File.ReadAllText(x3).Trim();
                    data.ParseClass(read);
                }
            }
            if (data.Type == DeviceType.MASS_STORAGE)
                data.StorageInfo = UpdateDisk(data);
            return data;
        }

        Blockdevice UpdateDisk(UsbDevice device)
        {
            Blockdevice dev = GetSDevice(device.iSerial);
            if (dev == null)
                dev = GetDeviceByModel(device.idProduct);

            return dev;
        }

        Blockdevice GetSDevice(string serialId)
        {
            Blockdevice x = null;
            lock (storageDevices)
            {
                x = storageDevices.GetDevice(serialId);
            }
            return x;
        }

        Blockdevice GetDeviceByModel(string serialId)
        {
            Blockdevice x = null;
            lock (storageDevices)
            {
                x = storageDevices.GetDeviceByModel(serialId);
            }
            return x;
        }

        string ExecuteComand(string cmd)
        {
            string result = "";

            using (var process = new Process())
            {
                try
                {
                    process.StartInfo = new ProcessStartInfo
                    {
                        FileName = "/bin/bash",
                        Arguments = "-c \"" + cmd + "\"",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        //CreateNoWindow = true,
                    };
                    process.Start();
                    result = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                }
                catch (Exception e) { result = e.ToString(); }
            }
            return result;
        }
    }
}
