using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRAPBase.Entities;
using IRAPBase.DTO;

namespace IRAPBase
{
    /// <summary>
    /// 对树的关联属性进行维护
    /// </summary>
    public  class IRAPTreeCorr
    {
        private IDbContext db = null;
        private int _corrID = 0;
        private ModelTreeCorrEntity _corrEntity = null;
        private IQueryable<ModelTreeClassfiyEntity> _classifySet = null;
        private IQueryable<ModelTreeTransient> _transientSet = null;
        private IQueryable<SysNameSpaceEntity> _namespaces = null;
        private IQueryable<ModelTreeStatus> _statusSet = null;
        /// <summary>
        /// 关联属性实体
        /// </summary>
        public ModelTreeCorrEntity CorrEntity { get { return _corrEntity; } }
      
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="corrID"></param>
        public IRAPTreeCorr(int corrID )
        {
            _corrID = -corrID;
            db = DBContextFactory.Instance.CreateContext("IRAPContext");
            _corrEntity = db.Set<ModelTreeCorrEntity>().FirstOrDefault(c => c.TreeCorrID==corrID);
            if (_corrEntity == null)
            {
                throw new Exception($"第 {corrID} 关联属性不存在！");
            }
            _classifySet = db.Set<ModelTreeClassfiyEntity>().Where(c => c.TreeID == _corrID);
            _transientSet = db.Set<ModelTreeTransient>().Where(c => c.TreeID == _corrID);
            _namespaces = db.Set<SysNameSpaceEntity>().Where(c => c.PartitioningKey == 0 && c.LanguageID == 30);
            _statusSet = db.Set<ModelTreeStatus>().Where(c => c.TreeID == _corrID);
        }

        /// <summary>
        /// 对实体CorrEntity进行修改，调用此方法保存
        /// </summary>
        public void DefineTreeCorr()
        {
            db.SaveChanges();
        }


        #region 查询分类、瞬态、状态属性清单
        /// <summary>
        /// 获取分类属性定义清单
        /// </summary>
        public List<TreeClassifyModelDTO> GetClassify()
        {
            return _classifySet.Join(_namespaces, a => a.AttrNameID, b => b.NameID, (a, b) =>
            new TreeClassifyModelDTO { AttrIndex = a.AttrIndex, AttrName = b.NameDescription, AttrTreeID = a.TreeID, SaveChangeHistory = a.SaveChangeHistory })
              .OrderBy(c => c.AttrIndex).ToList();
        }
        /// <summary>
        /// 获取瞬态属性定义
        /// </summary>
        /// <returns></returns>
        public List<TreeTransientModelDTO> GetTransient()
        {
            return _transientSet.Join(_namespaces, a => a.StatNameID, b => b.NameID, (a, b) =>
           new TreeTransientModelDTO { AttrIndex = a.StatIndex, AttrName = b.NameDescription, Protected = a.Protected, Scale = a.Scale, UnitOfMeasure = a.UnitOfMeasure })
             .OrderBy(c => c.AttrIndex).ToList();
        }

        /// <summary>
        /// 获取状态属性定义
        /// </summary>
        /// <returns></returns>
        public List<TreeStatusModelDTO> GetStatus()
        {
            var t5LeafSet = db.Set<ETreeSysLeaf>().Where(c => c.TreeID == 5);
            return _statusSet.Join(t5LeafSet, a => a.T5LeafID, b => b.LeafID, (a, b) =>
         new TreeStatusModelDTO { AttrIndex = a.StateIndex, AttrName = b.NodeName, T5LeafID = a.T5LeafID, Protect=a.Protected })
            .OrderBy(c => c.AttrIndex).ToList();
        }

        /// <summary>
        /// 获取第n状态属性定义清单
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public List<TreeStatusModelRowDTO> GetStatusList(int ordinal)
        {
            var statusList = GetStatus();
            var stateEntity = statusList.FirstOrDefault(c => c.AttrIndex == ordinal);

            if (stateEntity == null)
            {
                throw new Exception($"CorrID={_corrID} 的第{ordinal}状态属性没有定义！无法获取清单。");
            }
            var t5EntityID = stateEntity.T5LeafID;

            var list = db.Set<ModelTreeStatusList>().Where(c => c.PartitioningKey == 5 && c.EntityID == t5EntityID);

            return list.Join(_namespaces, a => a.StatusNameID, b => b.NameID, (a, b) =>
            new TreeStatusModelRowDTO { Ordinal = (byte)ordinal, StatusIndex = (byte)a.Ordinal, StatusName = b.NameDescription, Status = a.Status })
            .OrderBy(c => c.Ordinal).ThenBy(c => c.StatusIndex).ToList();
        }

        #endregion



        #region 分类属性维护：定义、修改、删除
        /// <summary>
        /// 定义分类属性
        /// </summary>
        /// <param name="attrindex">分类属性序号</param>
        /// <param name="attrTreeID">分类属性树标识</param>
        /// <param name="attrName">分类属性名称</param>
        /// <param name="forceModify">如果已存在是否强制修改</param>
        /// <returns></returns>
        public IRAPError DefineClassify(int attrindex, int attrTreeID, string attrName, bool forceModify = false)
        {
            var namespaces = IRAPNamespaceSetFactory.CreatInstance(Enums.NamespaceType.Sys);
            var nameID = namespaces.GetNameID(0, attrName);
            var entity = _classifySet.FirstOrDefault(c => c.AttrIndex == attrindex);
            if (entity != null)
            {
                if (!forceModify)
                {
                    throw new Exception($"第{attrindex}分类属性已存在！ 如需强制修改请传入forceModify=true");
                }
                else
                {
                    entity.AttrNameID = nameID;
                    entity.AttrTreeID = (short)attrTreeID;
                }
            }
            else
            {
                ModelTreeClassfiyEntity e = new ModelTreeClassfiyEntity()
                {
                    AttrIndex = (byte)attrindex,
                    AttrNameID = nameID,
                    AttrTreeID = (short)attrTreeID,
                    TreeID = (short)_corrID
                };
                db.Set<ModelTreeClassfiyEntity>().Add(e);
            }
            db.SaveChanges();
            return new IRAPError(0, $"第{attrindex}分类属性定义成功！");
        }

        /// <summary>
        /// 删除指定分类属性定义
        /// </summary>
        /// <param name="attrindex">属性序号</param>
        /// <returns></returns>
        public IRAPError DeleteClassify(int attrindex)
        {
            var entity = _classifySet.FirstOrDefault(c => c.AttrIndex == attrindex);
            if (entity != null)
            {
                db.Set<ModelTreeClassfiyEntity>().Remove(entity);
                db.SaveChanges();
                return new IRAPError(0, $"第{attrindex}分类属性删除成功！");
            }
            else
            {
                return new IRAPError(0, $"第{attrindex}分类属性不存在，无需删除！");
            }
        }

        #endregion

        #region 瞬态属性维护：定义、修改、删除
        /// <summary>
        /// 定义瞬态属性
        /// </summary>
        /// <param name="attrindex">瞬态属性序号</param>
        /// <param name="statName">瞬态属性名</param>
        /// <param name="unitOfMeasure">度量单位</param>
        /// <param name="scale">放大数量级</param>
        /// <param name="protect">属性维护时是否可修改</param>
        /// <param name="forceModify">强制修改定义</param>
        /// <returns></returns>
        public IRAPError DefineTransient(int attrindex, string statName, string unitOfMeasure, byte scale, bool protect = false, bool forceModify = false)
        {
            var namespaces = IRAPNamespaceSetFactory.CreatInstance(Enums.NamespaceType.Sys);
            var nameID = namespaces.GetNameID(0, statName);
            var entity = _transientSet.FirstOrDefault(c => c.StatIndex == attrindex);

            if (entity == null)
            {
                ModelTreeTransient e = new ModelTreeTransient()
                {
                    StatIndex = (byte)attrindex,
                    Protected = protect,
                    Scale = scale,
                    StatNameID = nameID,
                    TreeID = (short)_corrID,
                    UnitOfMeasure = unitOfMeasure
                };
                db.Set<ModelTreeTransient>().Add(e);
            }
            else
            {
                if (!forceModify)
                {
                    throw new Exception($"第{attrindex}瞬态属性已存在！ 如需强制修改请传入forceModify=true");
                }
                else
                {
                    entity.StatIndex = (byte)attrindex;
                    entity.StatNameID = nameID;
                    entity.Scale = scale;
                    entity.UnitOfMeasure = unitOfMeasure;
                    entity.Protected = protect;
                }
            }
            db.SaveChanges();
            return new IRAPError(0, $"第{attrindex}瞬态属性定义成功！");
        }

        /// <summary>
        /// 删除瞬态属性定义
        /// </summary>
        /// <param name="attrIndex">瞬态属性序号</param>
        /// <returns></returns>
        public IRAPError DeleteTransient(byte attrIndex)
        {
            var entity = _transientSet.FirstOrDefault(c => c.StatIndex == attrIndex);
            if (entity == null)
            {
                return new IRAPError(0, $"第{attrIndex}瞬态属性不存在，无需删除！");
            }
            else
            {
                db.Set<ModelTreeTransient>().Remove(entity);
                db.SaveChanges();
                return new IRAPError(0, $"第{attrIndex}瞬态属性删除成功！");
            }
        }
        #endregion

        #region 状态属性维护：定义，修改，删除
        /// <summary>
        /// 定义状态属性
        /// </summary>
        /// <param name="stateIndex">状态属性序号</param>
        /// <param name="t5LeafID">第五课树叶标识</param>
        /// <param name="protect">维护属性时是否能修改</param>
        /// <param name="forceModify">如已存在强制修改</param>
        /// <returns></returns>

        public IRAPError DefineStatus(int stateIndex, int t5LeafID, bool protect = false, bool forceModify = false)
        {
            var t5Entity = db.Set<ETreeSysLeaf>().FirstOrDefault(c => c.LeafID == t5LeafID && c.TreeID == 5);
            if (t5Entity == null)
            {
                throw new Exception("传入的参数T5LeafID在数据库中不存在！");
            }
            var entity = _statusSet.FirstOrDefault(c => c.StateIndex == stateIndex);
            if (entity == null)
            {
                ModelTreeStatus e = new ModelTreeStatus()
                {
                    TreeID = (short)_corrID,
                    Protected = protect,
                    StateIndex = (byte)stateIndex,
                    T5LeafID = t5LeafID
                };
                db.Set<ModelTreeStatus>().Add(e);
            }
            else
            {
                if (!forceModify)
                {
                    throw new Exception($"第{stateIndex}状态属性已定义，如需强制修改请传入forceModify=true");
                }
                else
                {
                    entity.Protected = protect;
                    entity.StateIndex = (byte)stateIndex;
                    entity.T5LeafID = t5LeafID;
                    entity.TreeID = (short)_corrID;
                }

            }
            db.SaveChanges();
            return new IRAPError(0, $"第{stateIndex}状态属性已定义！");
        }

        /// <summary>
        /// 删除状态属性
        /// </summary>
        /// <param name="attrIndex"></param>
        /// <returns></returns>
        public IRAPError DeleteStatus(byte attrIndex)
        {
            var entity = _statusSet.FirstOrDefault(c => c.StateIndex == attrIndex);
            if (entity == null)
            {
                return new IRAPError(0, $"第{attrIndex}状态属性不存在，无需删除！");
            }
            else
            {
                db.Set<ModelTreeStatus>().Remove(entity);
                db.SaveChanges();
                return new IRAPError(0, $"第{attrIndex}状态属性删除成功！");
            }
        }

        /// <summary>
        /// 定义状态属性清单
        /// </summary>
        /// <param name="t5LeafID">状态叶标识</param>
        /// <param name="ordinal">状态属性序号</param>
        /// <param name="list">清单(删除后重新插入)</param>
        /// <returns></returns>
        public IRAPError DefineStatusList(int t5LeafID, int ordinal, List<TreeStatusModelRowDTO> list)
        {
            var thisState = GetStatus().FirstOrDefault(c => c.AttrIndex == ordinal);
            if (thisState == null)
            {
                throw new Exception($"第{ordinal}状态属性未定义！");
            }
            IRAPTreeBase treeBase = new IRAPTreeBase(db, 0, 5, t5LeafID);
            var namespaces = IRAPNamespaceSetFactory.CreatInstance(Enums.NamespaceType.Sys);
            List<BaseRowAttrEntity> inputList = new List<BaseRowAttrEntity>();
            foreach (TreeStatusModelRowDTO r in list)
            {
                ModelTreeStatusList e = new ModelTreeStatusList()
                {
                    EntityID = t5LeafID,
                    Ordinal = r.Ordinal,
                    Status = r.Status,
                    StatusNameID = namespaces.GetNameID(0, r.StatusName)
                };
                inputList.Add(e);
            }
            return treeBase.SaveRSAttr(typeof(ModelTreeStatusList), inputList);
        }

        #endregion

       

        #region 行集属性定义
        /// <summary>
        /// 获取行集属性定义
        /// </summary>
        /// <returns></returns>
        public List<RowSetDefineDTO> GetRowSet()
        {
            var rowSet = db.Set<ModelTreeRowSet>().Where(c => c.TreeID == _corrID);
            var dto = from a in rowSet
                      join b in _namespaces on a.RSAttrNameID equals b.NameID
                      orderby a.RowSetID
                      select new RowSetDefineDTO
                      {
                          AttrIndex = a.RowSetID,
                          AttrName = b.NameDescription,
                          CurrentVersionNo = a.Version,
                          DicingFilter = a.DicingFilter,
                          ProcOnETL = a.ProcOnETL,
                          ProcOnSave = a.ProcOnSave,
                          ProcOnVersionApply = a.ProcOnVersionApply,
                          Protected = a.Protected,
                          RSAttrTBLName = a.RSAttrTBLName
                      };
            return dto.ToList();
        }
        /// <summary>
        /// 获取第n行集属性数据字典
        /// </summary>
        /// <param name="ordinal">第n行属性</param>
        /// <returns></returns>
        public List<IRAPTableColDTO> GetRowSetCols(int ordinal)
        {
            var rowSetList = GetRowSet();
            var thisRowSet = rowSetList.FirstOrDefault(c => c.AttrIndex == ordinal);
            if (thisRowSet == null)
            {
                throw new Exception($"第 {_corrID} 关联树第 {ordinal} 个行集属性未定义！");
            }

            var node = db.Set<ETreeSysDir>().FirstOrDefault(c => c.TreeID == 9 && c.Code == thisRowSet.RSAttrTBLName);
            if (node == null)
            {
                throw new Exception($"第 {_corrID} 关联树第 {ordinal} 个行集属性表在数据字典中未定义！");
            }

            var leafSet = db.Set<ETreeSysLeaf>().Where(c => c.Father == node.NodeID && c.TreeID == 9).ToList();
            var genAttrSet = db.Set<ModelTreeGeneral>().Where(c => c.PartitioningKey == 9);
            var dto = from a in leafSet
                      join b in genAttrSet on a.EntityID equals b.EntityID
                      orderby a.UDFOrdinal
                      select new IRAPTableColDTO
                      {
                          DisPlayName = a.NodeName,
                          ColName = a.Code,
                          AllowNull = b.AllowNull,
                          CollationRule = b.CollationRule   ,
                          DataPrecision = b.DataPrecision,
                          DecimalDigits = b.DecimalDigits,
                          FieldWidth = b.FieldWidth,
                          IdentityInc = b.IdentityInc,
                          IdentitySeed = b.IdentitySeed,
                          IsIdentity = b.IsIdentity,
                          IsRowGuid = b.IsRowGuid,
                          Length = b.Length,
                          MimeType = b.MimeType,
                          Type = b.Type
                      };
            return dto.ToList();

        }

        /// <summary>
        /// 定义行集属性
        /// </summary>
        /// <param name="attrIndex">行集属性序号</param>
        /// <param name="attrName">行集属性名称</param>
        /// <param name="rsAttrTBLName">行集属性表名</param>
        /// <param name="protect">是否保护(在界面中是否允许修改内容)</param>
        /// <returns></returns>
        public IRAPError DefineRowSet(byte attrIndex, string attrName, string rsAttrTBLName, bool protect)
        {
            var dbContext = db.Set<ModelTreeRowSet>();
            var rowSet = dbContext.FirstOrDefault(c => c.TreeID == _corrID && c.RowSetID == attrIndex);
            var nameID = IRAPNamespaceSetFactory.CreatInstance(Enums.NamespaceType.Sys).GetNameID(0, attrName);
            if (rowSet == null)
            {

                var entity = new ModelTreeRowSet
                {
                    RowSetID = attrIndex,
                    RSAttrNameID = nameID,
                    RSAttrTBLName = rsAttrTBLName,
                    Protected = protect,
                    TreeID = (short)_corrID,
                    LastUpdatedTime = DateTime.Now
                };
                dbContext.Add(entity);
            }
            else
            {
                rowSet.RSAttrNameID = nameID;
                rowSet.RSAttrTBLName = rsAttrTBLName;
                rowSet.Protected = protect;
            }
            db.SaveChanges();
            return new IRAPError(0, "行集属性定义成功！");
        }
        /// <summary>
        /// 删除行集属性的定义
        /// </summary>
        /// <param name="attrIndex"></param>
        /// <returns></returns>
        public IRAPError DeleteRowSetDefine(int attrIndex)
        {
            var rowSet = db.Set<ModelTreeRowSet>().FirstOrDefault(c => c.TreeID == _corrID && c.RowSetID == attrIndex);
            if (rowSet == null)
            {
                throw new Exception($"第 {attrIndex} 行集属性不存在！");
            }
            db.Set<ModelTreeRowSet>().Remove(rowSet);
            db.SaveChanges();
            return new IRAPError(0, "行集属性定义被成功删除。");
        }
        #endregion

 
    }
}
