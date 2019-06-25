using IRAPBase.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRAPBase.Entities;

namespace IRAPBase
{
    /// <summary>
    /// 业务模型
    /// </summary>
    public class IRAPBizModel
    {
        private int _opID;
        private IDbContext _db = null;
        private IRAPTreeBase _t4treeBase = null;
        private TreeLeafEntity _leaf = null;
        private IQueryable<SysNameSpaceEntity> _namespaces = null;
        /// <summary>
        /// 业务操作实体
        /// </summary>
        public TreeLeafEntity OperEntity
        {
            get { return _leaf; }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="opID"></param>
        public IRAPBizModel(int opID)
        {
            _opID = opID;
            _db = DBContextFactory.Instance.CreateContext("IRAPContext");
            _t4treeBase = new IRAPTreeBase(_db, 0, 4, opID);
            _leaf = _t4treeBase.Leaf;
            _namespaces = _db.Set<SysNameSpaceEntity>().Where(c => c.PartitioningKey == 0 && c.LanguageID == 30);
        }

        /// <summary>
        /// 带数据库上下文的构造函数
        /// </summary>
        /// <param name="db"></param>
        /// <param name="opID"></param>
        public IRAPBizModel(IDbContext db, int opID)
        {
            _opID = opID;
            this._db = db;
            _t4treeBase = new IRAPTreeBase(_db, 0, 4, opID);
            _leaf = _t4treeBase.Leaf;
            _namespaces = _db.Set<SysNameSpaceEntity>().Where(c => c.PartitioningKey == 0 && c.LanguageID == 30);
        }

        /// <summary>
        /// 获取一般属性
        /// </summary>
        /// <returns></returns>
        public GenAttr_T4 GetGenAttr()
        {
            return (GenAttr_T4)_t4treeBase.GetGenAttr<GenAttr_T4>();
        }
        /// <summary>
        /// 获取操作类型清单
        /// </summary>
        /// <returns></returns>
        public List<OpTypeDTO> GetOptypes()
        {
            GenAttr_T4 genAttr = GetGenAttr();
            IRAPTreeSet t18Set = new IRAPTreeSet(_db, 0, 18);
            var t4RowSet = _t4treeBase.GetRSAttr<RowSet_T4R1>().Cast<RowSet_T4R1>();

            var list = from a in t4RowSet
                       join b in t18Set.LeafEntities on a.T18LeafID equals b.LeafID
                       select new OpTypeDTO
                       {
                           AuxFactTBLName = genAttr.AuxTranTBLName,
                           AuxTranTBLName = genAttr.AuxTranTBLName,
                           AuthorizingCondition = a.AuthorizingCondition,
                           OLTPFixedFactTBLName = genAttr.OLTPFixedFactTBLName,
                           OLTPTempFactTBLName = genAttr.OLTPTempFactTBLName,
                           OpType = (byte)a.OpType,
                           OpTypeCode = b.Code,
                           OpTypeName = b.NodeName,
                           Ordinal = a.Ordinal
                       };
            return list.ToList();
        }

        /// <summary>
        /// 获取维度定义清单
        /// </summary>
        /// <returns></returns>
        public List<BizDimDTO> GetDimMatrix()
        {

            var rst4r3 = _t4treeBase.GetRSAttr<RowSet_T4R3>().Cast<RowSet_T4R3>();

            var dto = from a in rst4r3
                      join b in _namespaces on a.DimNameID equals b.NameID
                      orderby a.Ordinal
                      select new BizDimDTO { DimName = b.NameDescription, DimTreeID = a.TreeID, Ordinal = a.Ordinal };
            return dto.ToList();
        }

        /// <summary>
        /// 获取度量指标定义
        /// </summary>
        /// <returns></returns>
        public List<BizMetDTO> GetMetMaxtric()
        {
            var rst4r2 = _t4treeBase.GetRSAttr<RowSet_T4R2>().Cast<RowSet_T4R2>();
            var dto = from a in rst4r2
                      join b in _namespaces on  a.NameID equals b.NameID  
                      orderby a.Ordinal
                      select new BizMetDTO { MetricName = b.NameDescription, Ordinal = a.Ordinal, Scale = (byte)a.Scale, UnitOfMeasure = a.UnitOfMeasure };
            return dto.ToList();
        }

        /// <summary>
        /// 获取交易表的定义
        /// </summary>
        /// <returns></returns>
        public List<IRAPTableColDTO> GetTransactCols()
        {
            List<IRAPTableColDTO> list = new List<IRAPTableColDTO>();
            list.Add(new IRAPTableColDTO { ColName = "TransactNo", DisPlayName = "交易号", Type = "bigint" });
            list.Add(new IRAPTableColDTO { ColName = "PartitioningKey", DisPlayName = "分区键", Type = "bigint" });
            list.Add(new IRAPTableColDTO { ColName = "OperTime", DisPlayName = "操作时间", Type = "datetime" });
            list.Add(new IRAPTableColDTO { ColName = "OkayTime", DisPlayName = "复核时间", Type = "datetime" });
            list.Add(new IRAPTableColDTO { ColName = "RevokeTime", DisPlayName = "撤销时间", Type = "datetime" });
            list.Add(new IRAPTableColDTO { ColName = "AgencyLeaf1", DisPlayName = "机构1", Type = "int" });
            list.Add(new IRAPTableColDTO { ColName = "AgencyLeaf2", DisPlayName = "机构2", Type = "int" });
            list.Add(new IRAPTableColDTO { ColName = "AgencyLeaf3", DisPlayName = "机构3", Type = "int" });
            list.Add(new IRAPTableColDTO { ColName = "Operator", DisPlayName = "操作员代码", Type = "varchar" });
            list.Add(new IRAPTableColDTO { ColName = "Checked", DisPlayName = "复核人员代码", Type = "varchar" });
            list.Add(new IRAPTableColDTO { ColName = "Revoker", DisPlayName = "撤销人员代码", Type = "varchar" });
            list.Add(new IRAPTableColDTO { ColName = "StationID", DisPlayName = "站点Mac地址", Type = "varchar" });
            list.Add(new IRAPTableColDTO { ColName = "IPAddress", DisPlayName = "IP地址", Type = "varchar" });
            list.Add(new IRAPTableColDTO { ColName = "OpNodes", DisPlayName = "操作类型", Type = "varchar" });
            list.Add(new IRAPTableColDTO { ColName = "VoucherNo", DisPlayName = "票据号", Type = "varchar" });
            list.Add(new IRAPTableColDTO { ColName = "VoucherNoEx", DisPlayName = "站点号", Type = "varchar" });
            list.Add(new IRAPTableColDTO { ColName = "Attached1", DisPlayName = "附件1", Type = "int" });
            list.Add(new IRAPTableColDTO { ColName = "Attached2", DisPlayName = "附件2", Type = "int" });
            list.Add(new IRAPTableColDTO { ColName = "T16LeafID", DisPlayName = "工作流", Type = "varchar" });
            list.Add(new IRAPTableColDTO { ColName = "WFInstanceID", DisPlayName = "工作流实例", Type = "varchar" });
            list.Add(new IRAPTableColDTO { ColName = "LinkedTransactNo", DisPlayName = "链接交易号", Type = "bigint" });
            list.Add(new IRAPTableColDTO { ColName = "Status", DisPlayName = "状态", Type = "int" });
            list.Add(new IRAPTableColDTO { ColName = "Remark", DisPlayName = "备注", Type = "varchar" });
            return list;
        }

        /// <summary>
        /// 辅助交易表
        /// </summary>
        /// <returns></returns>
        public List<IRAPTableColDTO> GetAuxTranCols()
        {

            //找一般属性表名
            var treeModel = new IRAPTreeModelSet();
            var genAttrT4 = GetGenAttr();
            if (genAttrT4.AuxTranTBLName == null || genAttrT4.AuxTranTBLName == string.Empty)
            {
                throw new Exception($"业务操作：{_opID}的辅助交易表未定义。");
            }
            var node = _db.Set<ETreeSysDir>().FirstOrDefault(c => c.TreeID == 9 && c.Code == genAttrT4.AuxTranTBLName);
            if (node == null)
            {
                throw new Exception($"业务操作：{_opID}的辅助交易表在数据字典中未定义！");
            }
            var leafSet = _db.Set<ETreeSysLeaf>().Where(c => c.Father == node.NodeID && c.TreeID == 9);
            var genAttrSet = _db.Set<ModelTreeGeneral>().Where(c => c.PartitioningKey == 9);
            var dto = from a in leafSet
                      join b in genAttrSet on a.EntityID equals b.EntityID
                      orderby a.UDFOrdinal
                      select new IRAPTableColDTO
                      {
                          DisPlayName = a.NodeName,
                          ColName = a.Code,
                          AllowNull = b.AllowNull,
                          CollationRule = b.CollationRule,
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
        /// 返回行集事实的定义
        /// </summary>
        /// <returns></returns>
        public List<BizRSFactDTO> GetRowFact()
        {
            var rst4r4 = _t4treeBase.GetRSAttr<RowSet_T4R4>().Cast<RowSet_T4R4>();

            var dto = from a in rst4r4
                      join b in _namespaces on a.RSFactNameID equals b.NameID
                      orderby a.Ordinal
                      select new BizRSFactDTO
                      {
                          Ordinal = a.Ordinal,
                          RSFactName = b.NameDescription,
                          RSFactTBLName = a.RSFactTBLName,
                          ProcOnRSFactAppend = a.ProcOnRSFactAppend,
                          ProcOnRSFactDelete = a.ProcOnRSFactDelete,
                          ProcOnRSFactSave = a.ProcOnRSFactSave
                      };
            return dto.ToList();
        }

        /// <summary>
        /// 定义操作类型
        /// </summary>
        /// <param name="opType">操作类型序号</param>
        /// <param name="optypeName">操作类型名</param>
        /// <param name="optypeCode">操作类型代码</param>
        /// <param name="auxFactTBLName">一般属性表</param>
        /// <param name="auxTranTBLName">辅助交易表</param>
        /// <param name="oltpTempFactTBLName">临时业务流水表</param>
        /// <param name="oltpFixedFactTBLName">固化业务流水表</param>
        /// <param name="isValid">是否有效</param>
        /// <returns></returns>
        public IRAPError DefineOptypes(short opType, string optypeName, string optypeCode, string auxFactTBLName, string auxTranTBLName,
            string oltpTempFactTBLName, string oltpFixedFactTBLName, bool isValid)
        {
            GenAttr_T4 genAttr = GetGenAttr();
            IRAPTreeSet t18Set = new IRAPTreeSet(_db, 0, 18);
          //  var t4RowSet = _t4treeBase.GetRSAttr<RowSet_T4R1>().Cast<RowSet_T4R1>();
            var t4RowSet = _db.Set<RowSet_T4R1>().Where(c => c.PartitioningKey == 4 && c.EntityID == _opID);
            var nameSpaces = IRAPNamespaceSetFactory.CreatInstance(Enums.NamespaceType.Sys);
            var optypeEntity = t4RowSet.FirstOrDefault(c => c.Ordinal == opType);
            if (optypeEntity == null)
            {  //新增
                int nameID = nameSpaces.GetNameID(0, optypeName);
                var res = t18Set.NewTreeNode(4, 1018, optypeName, optypeCode, "Admin");

                if (res.ErrCode != 0)
                {
                    return new IRAPError { ErrCode = res.ErrCode, ErrText = res.ErrText };
                }
                int t18leafEntity = res.NewEntityID;
                RowSet_T4R1 row = new RowSet_T4R1()
                {
                    EntityID = _opID,
                    OpType = opType,
                    Ordinal = opType,
                    AuxFactTBLName = auxFactTBLName,
                    T18LeafID = t18leafEntity,
                    IsValid = isValid,
                    PartitioningKey = 4,
                    StateExclCtrlStr = "",
                    VersionLE = 2147483647,
                    AuthorizingCondition = "",
                    ComplementaryRule = "",
                    BusinessDateIsValid = false
                };
                _db.Set<RowSet_T4R1>().Add(row);
            }
            else
            {
                //修改
                optypeEntity.AuxFactTBLName = auxFactTBLName;
                optypeEntity.IsValid = isValid;
                IRAPTreeBase tree18 = new IRAPTreeBase(_db, 0, 18, optypeEntity.T18LeafID);
                if (tree18.Leaf.NodeName != optypeName)
                {
                    tree18.Leaf.NodeName = optypeName;
                    tree18.Leaf.Code = optypeCode;
                }

            }
            _db.SaveChanges();
            return new IRAPError(0, "定义操作类型成功！");
        }

        /// <summary>
        /// 操作类型
        /// </summary>
        /// <param name="opType"></param>
        /// <returns></returns>
        public IRAPError DeleteOpTypes(short opType)
        {
            //var t4RowSet = _t4treeBase.GetRSAttr<RowSet_T4R1>().Cast<RowSet_T4R1>();
            var t4RowSet = _db.Set<RowSet_T4R1>().Where(c => c.PartitioningKey == 4 && c.EntityID == _opID);
            var nameSpaces = IRAPNamespaceSetFactory.CreatInstance(Enums.NamespaceType.Sys);
            var optypeEntity = t4RowSet.FirstOrDefault(c => c.Ordinal == opType);
            if (optypeEntity == null)
            {
                throw new Exception($"业务操作 {OperEntity.NodeName}[{_opID}] 的操作类型 {opType} 不存在！");
            }
            _db.Set<RowSet_T4R1>().Remove(optypeEntity);
            _db.SaveChanges();
            return new IRAPError(0, "删除成功！");
        }

        /// <summary>
        /// 定义事实表维度
        /// </summary>
        /// <param name="ordinal">维度序号</param>
        /// <param name="dimTreeID">维度树标识</param>
        /// <param name="dimName">维度名称（可为空，为空则用树的名字)</param>
        /// <returns></returns>
        public IRAPError DefineDimMatrix(int ordinal, short dimTreeID, string dimName="")
        {
          //  var t4RowSet = _t4treeBase.GetRSAttr<RowSet_T4R3>().Cast<RowSet_T4R3>();
            var t4RowSet = _db.Set<RowSet_T4R3>().Where(c => c.PartitioningKey == 4 && c.EntityID == _opID);
            var obj = t4RowSet.FirstOrDefault(c => c.Ordinal == ordinal);
            //检查dimTreeID参数是否存在
            if (dimName== "")
            {
                IRAPTreeModelSet modelTree = new IRAPTreeModelSet(_db);
                var treeEntity=  modelTree.GetAllTrees().FirstOrDefault(c=>c.TreeID== dimTreeID);
                if ( treeEntity==null)
                {
                    throw new Exception("输入参数dimTreeID不存在！");
                }
                dimName = treeEntity.TreeName;
            }
            int nameID = IRAPNamespaceSetFactory.CreatInstance(Enums.NamespaceType.Sys).GetNameID(0, dimName, 30);
            if (obj == null) //新增
            {
                RowSet_T4R3 row = new RowSet_T4R3()
                {
                    EntityID = _t4treeBase.Leaf.EntityID,
                    Ordinal = ordinal,
                    DimNameID = nameID,
                    PartitioningKey = 4,
                    SrcFieldName = "",
                    TreeID = dimTreeID,
                    VersionLE = 2147483647
                };
                _db.Set<RowSet_T4R3>().Add(row);
            }
            else //修改
            {
                obj.DimNameID = nameID;
                obj.TreeID = dimTreeID;
            }
            _db.SaveChanges();
            return new IRAPError(0, $"业务:{OperEntity.NodeName}[{_opID}] 第 {ordinal} 维度定义成功！");
        }
        /// <summary>
        /// 删除维度定义
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public IRAPError DeleteDimMatrix(int ordinal)
        {

            var t4RowSet = _db.Set<RowSet_T4R3>().Where(c => c.PartitioningKey == 4 && c.EntityID == _opID);
            var obj = t4RowSet.FirstOrDefault(c => c.Ordinal == ordinal);
            if (obj == null) //新增
            {
                throw new Exception($"业务:{OperEntity.NodeName}[{_opID}] 第 {ordinal} 维度不存在，无需删除！");
            }
            _db.Set<RowSet_T4R3>().Remove(obj);
            _db.SaveChanges();
            return new IRAPError(0, $"业务:{OperEntity.NodeName}[{_opID}] 第 {ordinal} 维度 删除成功！");
        }
        /// <summary>
        /// 定义度量
        /// </summary>
        /// <param name="ordinal"> 序号</param>
        /// <param name="MatName">名称</param>
        /// <param name="scale">精度（放大数量级）</param>
        /// <param name="unitOfMeasure">单位</param>
        /// <returns></returns>
        //定义事实表度量
        public IRAPError DefineMetMatric(int ordinal,string MatName, byte scale, string unitOfMeasure)
        {
          //  var t4RowSet = _t4treeBase.GetRSAttr<RowSet_T4R2>().Cast<RowSet_T4R2>();
            var t4RowSet = _db.Set<RowSet_T4R2>().Where(c => c.PartitioningKey == 4 && c.EntityID == _opID);
            int nameID = IRAPNamespaceSetFactory.CreatInstance(Enums.NamespaceType.Sys).GetNameID(0, MatName);
            var obj = t4RowSet.FirstOrDefault(c => c.Ordinal == ordinal);
            if (obj == null) //新增
            {
                RowSet_T4R2 row = new RowSet_T4R2()
                {
                    EntityID = _t4treeBase.Leaf.EntityID,
                    NameID = nameID,
                    Ordinal = ordinal,
                    PartitioningKey = 4,
                    Scale = scale,
                    UnitOfMeasure = unitOfMeasure,
                    VersionLE = 2147483647
                };
                _db.Set<RowSet_T4R2>().Add(row);
            }
            else
            {
                obj.Scale = scale;
                obj.NameID = nameID;
                obj.UnitOfMeasure = unitOfMeasure;
            }
            _db.SaveChanges();
            return new IRAPError(0,"定义度量成功！");
        }

        /// <summary>
        /// 删除度量定义
        /// </summary>
        /// <param name="ordinal">度量序号</param>
        /// <returns></returns>
        public IRAPError DeleteMetMatric(int ordinal)
        {
           // var t4RowSet = _t4treeBase.GetRSAttr<RowSet_T4R2>().Cast<RowSet_T4R2>();
            var t4RowSet = _db.Set<RowSet_T4R2>().Where(c => c.PartitioningKey == 4 && c.EntityID == _opID);
            var obj = t4RowSet.FirstOrDefault(c => c.Ordinal == ordinal);
            if (obj == null) //新增
            {
                throw new Exception($"业务操作:{OperEntity.NodeName}[{_opID}] 第 {ordinal} 度量不存在，无需删除！");
            }
            _db.Set<RowSet_T4R2>().Remove(obj);
            _db.SaveChanges();
            return new IRAPError(0, $"业务操作:{OperEntity.NodeName}[{_opID}] 第 {ordinal} 度量 删除成功！");
        }

        //对业务定义进行增改，删除放在IRAPBizModelSet中
    }
}
