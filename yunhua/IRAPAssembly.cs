using IRAPBase.Entities;
using IRAPBase.Serialize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase
{
   /// <summary>
   /// 分析IRAPBase以及业务程序集的类库
   /// </summary>
   public class IRAPAssembly
    {

        private   List<Type> genAttrClassList = new List<Type>();
        private    Dictionary<string, Type> genAttrTBL = new Dictionary<string, Type>();

        /// <summary>
        /// 获取一般属性类
        /// </summary>
        /// <returns></returns>
        public   List<Type> GetGenAttrClass()
        {
            if (genAttrClassList.Count > 0)
            {
                return genAttrClassList;
            }
            
            var list= Assembly.GetExecutingAssembly().GetTypes();
            foreach (Type r in list)
            {
                var baseType = r.BaseType;
                if (baseType == typeof(BaseGenAttrEntity))
                {
                    genAttrClassList.Add(r);
                } 
            }
            //外部的接口
            var list2 = Assembly.GetEntryAssembly().GetTypes();
            foreach (Type r in list2)
            {
                var baseType = r.BaseType;
                if (baseType == typeof(BaseGenAttrEntity))
                {
                    genAttrClassList.Add(r);
                }
            }
            return genAttrClassList;
        }


        /// <summary>
        /// 获取一般属性表名
        /// </summary>
        /// <returns></returns>
        public   Dictionary<string,Type> GetGenAttrDict()
        {
            if (genAttrTBL.Count>0)
            {
                return genAttrTBL;
            }
            List<Type> list = GetGenAttrClass();
            foreach(Type r in list)
            {
                string tblName = ObjectCopy.GetTBLName(r);
                if (tblName != "")
                {
                    if (!genAttrTBL.ContainsKey(tblName))
                    {
                        genAttrTBL.Add(tblName, r);
                    }
                }
            }
            return genAttrTBL; 
        }
    }
}
