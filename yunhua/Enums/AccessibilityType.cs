/*----------------------------------------------------------------
// Copyright © 2019 Chinairap.All rights reserved. 
// CLR版本：	4.0.30319.42000
// 类 名 称：    AccessibilityType
// 文 件 名：    AccessibilityType
// 创建者：      DUWENINK
// 创建日期：	2019/7/15 10:06:32
// 版本	日期					修改人	
// v0.1	2019/7/15 10:06:32	DUWENINK
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
    /// 创建日期： 2019/7/15 10:06:32
    /// 类名：     AccessibilityType
    /// </summary>
    [Description("可访问性")]
    public enum AccessibilityType:byte
    {
        /// <summary>
        /// 不可选
        /// </summary>
        [Description("不可选")]
        Disabled =0,
        /// <summary>
        /// 可单选
        /// </summary>
        [Description("可单选")]
        Radio =1,
        /// <summary>
        /// 可复选
        /// </summary>
        [Description("可复选")]
        CheckBox = 2

    }
}
