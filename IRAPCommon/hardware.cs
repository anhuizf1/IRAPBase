using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Management;


namespace IRAPCommon
{

    /// Computer Information 
    public class Computer
    {
        public string CpuID; //1.cpu序列号
        public string MacAddress; //2.mac序列号
        public string DiskID; //3.硬盘id
        public string IpAddress; //4.ip地址
        public string LoginUserName; //5.登录用户名
        public string ComputerName; //6.计算机名
        public string SystemType; //7.系统类型
        public string TotalPhysicalMemory; //8.内存量 单位：M

        public Computer()
        {
            CpuID = GetCpuID();
            MacAddress = GetMacAddress();
            DiskID = GetDiskID();
            IpAddress = GetIPAddress();
            LoginUserName = GetUserName();
            SystemType = GetSystemType();
            TotalPhysicalMemory = GetTotalPhysicalMemory();
            ComputerName = GetComputerName();
        }
        //1.获取CPU序列号代码 

        string GetCpuID()
        {
            try
            {
                string cpuInfo = "";//cpu序列号 
                ManagementClass mc = new ManagementClass("Win32_Processor");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    cpuInfo = mo.Properties["ProcessorId"].Value.ToString();
                }
                moc = null;
                mc = null;
                return cpuInfo;
            }
            catch
            {
                return "unknow";
            }
            finally
            {
            }

        }

        //2.获取网卡硬件地址 

        string GetMacAddress()
        {
            try
            {
                StringBuilder mac = new StringBuilder();
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    if ((bool)mo["IPEnabled"] == true)
                    {
                        mac.Append(mo["MacAddress"].ToString()+";");
                       // break;
                    }
                }
                moc = null;
                mc = null;
                return mac.ToString();
            }
            catch
            {
                return "unknow";
            }
            finally
            {
            }
        }

        //3.获取硬盘ID 
        string GetDiskID()
        {
            //try
            //{
            //    String HDid = "";
            //    ManagementClass mc = new ManagementClass("Win32_DiskDrive");
            //    ManagementObjectCollection moc = mc.GetInstances();
            //    foreach (ManagementObject mo in moc)
            //    {
            //        HDid = (string)mo.Properties["Model"].Value;
            //    }
            //    moc = null;
            //    mc = null;
            //    return HDid;
            //}
            //catch
            //{
            //    return "unknow";
            //}
            //finally
            //{
            //}

            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMedia");
                String strHardDiskID = null;
                foreach (ManagementObject mo in searcher.Get())
                {
                    strHardDiskID = mo["SerialNumber"].ToString().Trim();
                    break;
                }
                return strHardDiskID;
            }
            catch
            {
                return "";
            }   

        }


        //4.获取IP地址 

        string GetIPAddress()
        {
            try
            {
                //string st = "";
                StringBuilder st = new StringBuilder();
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    if ((bool)mo["IPEnabled"] == true)
                    {
                       
                        System.Array ar;
                        ar = (System.Array)(mo.Properties["IpAddress"].Value);
                        st.Append( ar.GetValue(0).ToString()+";");
                        //break;
                    }
                }
                moc = null;
                mc = null;
                
                return st.ToString();
            }
            catch
            {
                return "unknow";
            }
            finally
            {
            }

        }



        /// 5.操作系统的登录用户名 
        string GetUserName()
        {
            try
            {
                string st = "";
                st = Environment.UserName;
                return st;
            }
            catch
            {
                return "unknow";
            }
            finally
            {
            }
        }
        //6.获取计算机名
        string GetComputerName()
        {
            try
            {
                return System.Environment.MachineName;
            }
            catch
            {
                return "unknow";
            }
            finally
            {
            }
        }
        ///7 PC类型 
        string GetSystemType()
        {
            try
            {
                string st = "";
                ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    st = mo["SystemType"].ToString();
                }
                moc = null;
                mc = null;
                return st;
            }
            catch
            {
                return "unknow";
            }
            finally
            {
            }
        }
        /// 8.物理内存 
        string GetTotalPhysicalMemory()
        {
            try
            {
                string st = "";
                ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    st = mo["TotalPhysicalMemory"].ToString();
                }
                moc = null;
                mc = null;
                return st;
            }
            catch (Exception err)
            {
                return string.Empty;
            }
        }
    }
}
