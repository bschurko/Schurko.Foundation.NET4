using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace Schurko.Foundation.Utils
{
    public class HardwareInfo
    {
        public static string GetHardDisks()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_LogicalDisk");

            StringBuilder sb = new StringBuilder();

            foreach (ManagementObject wmi in searcher.Get())
            {
                try
                {
                    sb.Append(wmi.GetPropertyValue("VolumeSerialNumber").ToString());
                }
                catch
                {
                    return sb.ToString();
                }
            }

            return sb.ToString();
        }

        public static string GetBoardMaker()
        {
            ManagementObjectSearcher searcher = new
                ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BaseBoard");

            foreach (ManagementObject wmi in searcher.Get())
            {
                try
                {
                    return wmi.GetPropertyValue("Manufacturer").ToString();
                }
                catch { }
            }
            return "Not Found";
        }

        public static string GetBoardSerNo()
        {
            ManagementObjectSearcher searcher = new
                ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BaseBoard");

            foreach (ManagementObject wmi in searcher.Get())
            {
                try
                {
                    return wmi.GetPropertyValue("SerialNumber").ToString();
                }
                catch { }
            }
            return "Not Found";
        }

        public static string GetMACAddress()
        {
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();
            string MACAddress = "";
            foreach (ManagementObject mo in moc)
            {
                try
                {
                    if (MACAddress == "")  // only return MAC Address from first card
                    {
                        if ((bool)mo["IPEnabled"] == true) MACAddress = mo["MacAddress"].ToString();
                    }
                    mo.Dispose();

                    MACAddress = MACAddress.Replace(":", "");
                    return MACAddress;
                }
                catch { }
            }
            return "Not Found";
        }

        public static string GetBoardProductId()
        {
            ManagementObjectSearcher searcher = new
                ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BaseBoard");

            foreach (ManagementObject wmi in searcher.Get())
            {
                try
                {
                    return wmi.GetPropertyValue("Product").ToString();
                }
                catch { }
            }
            return "Not Found";
        }

        public static string GetCdRomDrive()
        {
            ManagementObjectSearcher searcher = new
                ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_CDROMDrive");

            foreach (ManagementObject wmi in searcher.Get())
            {
                try
                {
                    return wmi.GetPropertyValue("Drive").ToString();
                }
                catch { }
            }
            return "Not Found";
        }

        public static string GetCPUId()
        {
            string cpuInfo = string.Empty;
            ManagementClass mc = new ManagementClass("win32_processor");
            ManagementObjectCollection moc = mc.GetInstances();

            foreach (ManagementObject mo in moc)
            {
                try
                {
                    if (cpuInfo == "")
                    {
                        //Get only the first CPU's ID
                        cpuInfo = mo.Properties["processorID"].Value.ToString();
                        break;
                    }
                    return cpuInfo;
                }

                catch { }
            }
            return "Not Found";
        }

        public static string GetCurrentUser()
        {
            ManagementObjectSearcher searcher = new
                ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_UserAccount");

            foreach (ManagementObject wmi in searcher.Get())
            {
                try
                {
                    return wmi.GetPropertyValue("Name").ToString();
                }
                catch { }
            }
            return "Not Found";
        }

        public static string GetBIOScaption()
        {
            ManagementObjectSearcher searcher = new
                ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BIOS");

            foreach (ManagementObject wmi in searcher.Get())
            {
                try
                {
                    return wmi.GetPropertyValue("Caption").ToString();
                }
                catch { }
            }
            return "Not Found";
        }

        public static string GetBIOSmaker()
        {
            ManagementObjectSearcher searcher = new
                ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BIOS");

            foreach (ManagementObject wmi in searcher.Get())
            {
                try
                {
                    return wmi.GetPropertyValue("Manufacturer").ToString();
                }
                catch { }
            }
            return "Not Found";
        }

        public static string GetBIOSserNo()
        {
            ManagementObjectSearcher searcher = new
                ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BIOS");

            foreach (ManagementObject wmi in searcher.Get())
            {
                try
                {
                    return wmi.GetPropertyValue("SerialNumber").ToString();
                }
                catch { }
            }
            return "Not Found";
        }
    }
}
