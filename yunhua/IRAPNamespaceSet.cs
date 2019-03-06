using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase
{
    public class IRAPNamespaceSet
    {
        #region 单例类

        private static IRAPNamespaceSet _instance = null;

        public static IRAPNamespaceSet Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new IRAPNamespaceSet();
                }
                return _instance;
            }
        }

        #endregion

        /// <summary>
        /// 查找指定名称的名称标识
        /// </summary>
        /// <param name="communityID">社区标识</param>
        /// <param name="nameDescription">名称</param>
        /// <returns>名称标识</returns>
        private int GetNameID(int communityID, string nameDescription)
        {
            return 0;
        }

        /// <summary>
        /// 新增一个名称
        /// </summary>
        /// <param name="communityID">社区标识</param>
        /// <param name="nameDescription">名称</param>
        /// <returns>名称标识</returns>
        public int Add(int communityID, string nameDescription)
        {
            int nameID = GetNameID(communityID, nameDescription);
            if (nameID != 0)
            {
                return nameID;
            }

            return 0;
        }
    }
}
