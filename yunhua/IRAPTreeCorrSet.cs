using IRAPBase.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using IRAPBase.DTO;

namespace IRAPBase
{
    /// <summary>
    /// 对树关联进行管理
    /// </summary>
   public class IRAPTreeCorrSet
    {
        private static IRAPTreeCorrSet _instance = null;
        private IDbContext db = null;
        private IDbSet<ModelTreeCorrEntity> _treeCorrSet = null;
        
        /// <summary>
        /// 关联属性清单
        /// </summary>
        public List<ModelTreeCorrEntity> TreeCorrList {

            get { return _treeCorrSet.OrderBy(c=>c.TreeCorrID).ToList(); }
        }
        /// <summary>
        /// 单实例
        /// </summary>
        public static IRAPTreeCorrSet InstanceID
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new IRAPTreeCorrSet();
                }
                return _instance;
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public IRAPTreeCorrSet()
        {
            db = DBContextFactory.Instance.CreateContext("IRAPContext");
            _treeCorrSet = db.Set<ModelTreeCorrEntity>();
        }

        /// <summary>
        /// 删除树关联的定义
        /// </summary>
        /// <param name="corrID"></param>
        /// <returns></returns>
        public IRAPError DeleteTreeCorrDefine( int corrID)
        {
           var corrEntity=   _treeCorrSet.FirstOrDefault(c => c.TreeCorrID == corrID);
            if (corrEntity == null)
            {
                throw new Exception($"树关联 {corrID} 不存在！");
            }
            _treeCorrSet.Remove(corrEntity);
            db.SaveChanges();
            return new IRAPError(0, $"删除树关联 {corrID} 成功！");
        }

        /// <summary>
        /// 定义树关联
        /// </summary>
        /// <param name="treeCorrID">树关联标识，这里是正数</param>
        /// <param name="corrAttrTBLName">关联一般属性</param>
        /// <param name="corrAttrTBLNameEx">管理扩展一般属性</param>
        /// <param name="trees">关联其他树数组</param>
        /// <returns></returns>
        public IRAPError DefineTreeCorr( int treeCorrID, string corrAttrTBLName,string corrAttrTBLNameEx,params int [] trees)
        {
            if ( trees.Length>12)
            {
                throw new Exception("暂不支持这么多关联，目前不能超过12个关联树。");
            }
            //short maxID = _treeCorrSet.Max(c => c.TreeCorrID);
           var treeCorrEntity=_treeCorrSet.FirstOrDefault(c => c.TreeCorrID == treeCorrID);
            if (treeCorrEntity==null)
            {
                //新增
                ModelTreeCorrEntity e = new ModelTreeCorrEntity()
                {
                    TreeCorrID = (short)treeCorrID,
                    CorrAttrTBLName = corrAttrTBLName,
                    CorrAttrTBLNameEx = corrAttrTBLNameEx
                };
                Type Ts = e.GetType();
                for(int i=0; i<trees.Length;i++)
                {
                    int j = i + 1;
                    string fname = "TreeID" + (j < 10 ? "0" + j.ToString() : j.ToString() );
                    object v2 = Convert.ChangeType(trees[i], Ts.GetProperty(fname).PropertyType);
                    Ts.GetProperty(fname).SetValue(e, v2, null);
                }
                _treeCorrSet.Add(e);
                
            }
            else
            {
                //修改
                treeCorrEntity.CorrAttrTBLName = corrAttrTBLName;
                treeCorrEntity.CorrAttrTBLNameEx = corrAttrTBLNameEx;
                Type Ts = treeCorrEntity.GetType();
                for (int i = 0; i < trees.Length; i++)
                {
                    int j = i + 1;
                    string fname = "TreeID" + (j < 10 ? "0" + j.ToString() : j.ToString());
                    object v2 = Convert.ChangeType(trees[i], Ts.GetProperty(fname).PropertyType);
                    Ts.GetProperty(fname).SetValue(treeCorrEntity, v2, null);
                }
            }
            db.SaveChanges();
            return new IRAPError(0,"定义树关联成功！");
        }



    }
}
