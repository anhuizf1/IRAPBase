/*----------------------------------------------------------------
// Copyright © 2019 Chinairap.All rights reserved. 
// CLR版本：	4.0.30319.42000
// 类 名 称：    SelectStatusType
// 文 件 名：    SelectStatusType
// 创建者：      DUWENINK
// 创建日期：	2019/7/15 10:13:11
// 版本	日期					修改人	
// v0.1	2019/7/15 10:13:11	DUWENINK
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase.Enums
{
    /// <summary>
    /// 命名空间： IRAPBase.Enums
    /// 创建者：   DUWENINK
    /// 创建日期： 2019/7/15 10:13:11
    /// 类名：     SelectStatusType
    /// </summary>
    [Description("选中状态")]
    public enum SelectStatusType
    {
        /// <summary>
        /// 未选中
        /// </summary>
        [Description("未选中")]
        UnChecked =0,
        /// <summary>
        /// 已选中
        /// </summary>
        [Description("已选中")]
        Checked =1
    }
}
