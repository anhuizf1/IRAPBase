using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase.DTO
{
    public class SystemDTO
    {
        public int SystemID { get; set; }                    //       --系统标识
        public string SystemName { get; set; }              //--系统名称
        public string VersionNo { get; set; }                //  --版本号
        public string IconFile { get; set; }       //         --图标文件
        public byte[] IconImage { get; set; }                 //       --图标图像
        public string ProductNo { get; set; }   // --软件产品编号
        public string GAYearMonth { get; set; } //             --发布年月
        public string Author { get; set; }     //  --著作权人
        public string Coauthor { get; set; } //     --共有著作权人
        public string LogoPicPath { get; set; }             //   --Logo图片文件路径
        public byte[] LogoPic { get; set; }                //       --Logo图片图像
        public string BGPicPath { get; set; }              //  --背景图片文件路径
        public byte[] BackgroundPic { get; set; }         //    --背景图片图像
        public byte MenuStyle { get; set; }            //   --菜单风格（左右+上下）
        public byte MenuShowCtrl { get; set; }          //     --菜单显示控制值
        public bool AddToolBar { get; set; }                        //   --显示工具条
        public byte ScreenResolution { get; set; }                 //        --屏幕分辨率
        public string KeyStrokeStream { get; set; }                   //--按键流
        public string Accessible { get; set; }                         //       --是否有权访问
        public Int16 DataSrcLinkID { get; set; }                         //  --默认数据源连接
        public string Application { get; set; }               // --应用服务器地址
    }
}
