using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IRAPCommon
{
  
    public class GetMachineInfo
    {
        public static string GetCpuID()
        {
            try
            {
                ManagementClass mc = new ManagementClass("Win32_Processor");
                ManagementObjectCollection moc = mc.GetInstances();
                string strCpuID = null;
                foreach (ManagementObject mo in moc)
                {
                    strCpuID = mo.Properties["ProcessorId"].Value.ToString();
                    break;
                }

                return strCpuID;
            }
            catch
            {
                return "";
            }
        }

        //BIOS
        public static string GetBiosInfo()
        {
            string resultStr = "";
            try
            {

                ManagementObjectSearcher searcher =

                  new ManagementObjectSearcher("Select * From Win32_BIOS");

                foreach (ManagementObject mo in searcher.Get())
                {
                    if (mo["Manufacturer"].ToString() == string.Empty)
                    {
                        continue;
                    }
                    resultStr = mo["Manufacturer"].ToString();
                    // mo["SerialNumber"]
                }
                return resultStr;
            }
            catch
            {
                return "BIOS-BIOS-BIOS";
            }
        }
        /// <summary>
        /// 适合联网情况下读取Mac地址
        /// </summary>
        /// <returns></returns>
        public static List<string> GetMacByWMI()
        {
            List<string> macs = new List<string>();
            try
            {
                string mac = "";
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    if ((bool)mo["IPEnabled"])
                    {
                        mac = mo["MacAddress"].ToString().Replace(':', '-');

                        string[] addresses = (string[])mo["IPAddress"];

                        foreach (string ipaddress in addresses)
                        {
                            // MessageBox.Show(string.Format("IP Address: {0}", ipaddress));
                        }

                        macs.Add(mac);
                    }
                }
                moc = null;
                mc = null;
            }
            catch (Exception err)
            {
                macs.Add("00-00-00-00-00-00");
                ///MessageBox.Show("取mac地址失败：" + err.Message);
            }
            return macs;
        }


        //返回描述本地计算机上的网络接口的对象(网络接口也称为网络适配器)。
        public static NetworkInterface[] NetCardInfo()
        {
            return NetworkInterface.GetAllNetworkInterfaces();
        }
        /// <summary>
        /// 通过NetworkInterface读取网卡Mac,适合不联网情况下读取Mac地址
        /// </summary>
        /// <returns></returns>
        public static List<string> GetMacByNetworkInterface()
        {
            List<string> macs = new List<string>();
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface ni in interfaces)
            {

                if (ni.GetPhysicalAddress().ToString() == string.Empty)
                {
                    continue;
                }
                if (macs.Contains(ni.GetPhysicalAddress().ToString()))
                {
                    continue;
                }
                macs.Add(ni.GetPhysicalAddress().ToString());
            }
            return macs;
        }
    }
}
