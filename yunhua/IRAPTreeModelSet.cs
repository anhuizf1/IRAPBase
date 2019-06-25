using IRAPBase.DTO;
using IRAPBase.Entities;
using IRAPBase.Serialize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase
{

    /// <summary>
    /// 树建模，属性建模
    /// </summary>
    public class IRAPTreeModelSet
    {
        private Repository<ModelSysTreeEntity> treesRepoSys = null;
        private Repository<ModelBizTreeEntity> treesRepoBiz = null;
        private IDbContext db = null;
        private IDbContext mdmdb = null;
        /// <summary>
        /// 构造函数
        /// </summary>
        public IRAPTreeModelSet()
        {
            db = DBContextFactory.Instance.CreateContext("IRAPContext");
            mdmdb = DBContextFactory.Instance.CreateContext("IRAPMDMContext");
            treesRepoSys = new Repository<ModelSysTreeEntity>(db);
            treesRepoBiz = new Repository<ModelBizTreeEntity>(mdmdb);
        }

        /// <summary>
        /// 构造函数（带数据库上下文)以便调用者控制SaveChange()
        /// </summary>
        /// <param name="db"></param>
        public IRAPTreeModelSet(IDbContext db)
        {
            this.db = db;
            treesRepoSys = new Repository<ModelSysTreeEntity>(db);
            mdmdb = DBContextFactory.Instance.CreateContext("IRAPMDMContext");
            treesRepoBiz = new Repository<ModelBizTreeEntity>(mdmdb);
        }

        /// <summary>
        /// 获取NameID
        /// </summary>
        /// <param name="nameDesc"></param>
        /// <returns></returns>
        private int GetNameID(string nameDesc)
        {
            IIRAPNamespaceSet nameSet = IRAPNamespaceSetFactory.CreatInstance(Enums.NamespaceType.Sys);
            return nameSet.GetNameID(0, nameDesc, 30);
        }
        /// <summary>
        /// 创建一颗树
        /// </summary>
        /// <param name="treeID">树标识</param>
        /// <param name="treeName">树名称</param>
        /// <param name="leafLimit">叶数量上限</param>
        /// <param name="depthLimit">深度上限</param>
        /// <param name="shareToAll">是否为共享树</param>
        /// <param name="treeType">树类型默认：2=一般树</param>
        /// <param name="entityAttrTBName">一般属性表</param>
        /// <param name="nodeAttrTBLName">结点属性表</param>
        /// <param name="orderByMode">排序规则1-代码2-名称3-自定义</param>
        /// <param name="entityCodeName">实体代码名称</param>
        /// <param name="alternateCodeName">替代代码名称</param>
        /// <param name="uniqueEntityCode">实体代码是否唯一</param>
        /// <param name="uniqueNodeCode">结点代码是否唯一</param>
        /// <param name="autoCodeGenerating">是否自动编码</param>
        /// <param name="communityIndependent">社区独立性</param>
        /// <param name="exclusiveLevel">是否排他</param>
        public void CreateATree(int treeID, string treeName, int leafLimit, int depthLimit, bool shareToAll, byte treeType,
           string entityAttrTBName, string nodeAttrTBLName, int orderByMode, string entityCodeName, string alternateCodeName,
           bool uniqueEntityCode, bool uniqueNodeCode, bool autoCodeGenerating, bool communityIndependent, byte exclusiveLevel)
        {
            ModelTreeEntity treeEntity = null;
            if (treeID > 100)
            {
                treeEntity = treesRepoBiz.Table.FirstOrDefault(c => c.TreeID == treeID);
            }
            else
            {
                treeEntity = treesRepoSys.Table.FirstOrDefault(c => c.TreeID == treeID);
            }
            if (treeEntity != null)
            {
                throw new Exception($"已存在Tree={treeID}");
            }

            int nameID = GetNameID(treeName);
            int entityNameID = GetNameID(entityCodeName);
            int alternateNameID = GetNameID(alternateCodeName);
            var etree = new ModelTreeEntity()
            {
                TreeID = (short)treeID,
                NameID = nameID,
                LeafLimit = leafLimit,
                DepthLimit = depthLimit,
                ShareToAll = shareToAll,
                TreeType = treeType,
                LastUpdatedTime = DateTime.Now,
                EntityAttrTBName = entityAttrTBName,
                NodeAttrTBName = nodeAttrTBLName,
                ExclusiveLevel = exclusiveLevel,
                CommunityIndependent = communityIndependent,
                AlternateCodeNameID = alternateNameID,
                AutoCodeGenerating = autoCodeGenerating,
                OrderByMode = (byte)orderByMode,
                EntityCodeNameID = entityNameID
            };

            if (treeID > 100)
            {
                ModelBizTreeEntity e = etree.CopyTo<ModelBizTreeEntity>();
                treesRepoBiz.Insert(e);
                treesRepoBiz.SaveChanges();
            }
            else
            {
                ModelSysTreeEntity e = etree.CopyTo<ModelSysTreeEntity>();
                treesRepoSys.Insert(e);
                treesRepoSys.SaveChanges();
            }

            return;
        }
        /// <summary>
        /// 获取一棵树
        /// </summary>
        /// <param name="treeID">树标识</param>
        /// <returns></returns>
        public ModelTreeEntity GetATree(int treeID)
        {
            ModelTreeEntity treeEntity = null;

            if (treeID > 100)
            {
                treeEntity = treesRepoBiz.Table.FirstOrDefault(c => c.TreeID == treeID);
            }
            else
            {
                treeEntity = treesRepoSys.Table.FirstOrDefault(c => c.TreeID == treeID);
            }
            if (treeEntity == null)
            {
                throw new Exception($"不存在Tree={treeID}");
            }
            return treeEntity;
        }

        /// <summary>
        /// 设置一般属性表名
        /// </summary>
        /// <param name="treeID"></param>
        /// <param name="tblName"></param>
        public void SetEntityAttrTBName(int treeID, string tblName)
        {
            var eTree = GetATree(treeID);
            eTree.EntityAttrTBName = tblName;
            eTree.LastUpdatedTime = DateTime.Now;
            if (treeID > 100)
            {
                mdmdb.SaveChanges();
            }
            else
            {
                db.SaveChanges();
            }

        }
        /// <summary>
        /// 设置结点属性表名
        /// </summary>
        /// <param name="treeID"></param>
        /// <param name="attrTBName"></param>
        public void SetNodeAttrTBName(int treeID, string attrTBName)
        {
            var eTree = GetATree(treeID);
            eTree.NodeAttrTBName = attrTBName;
            eTree.LastUpdatedTime = DateTime.Now;
            if (treeID > 100)
            {
                mdmdb.SaveChanges();
            }
            else
            {
                db.SaveChanges();
            }
        }
        /// <summary>
        /// 设置实体代码名称
        /// </summary>
        /// <param name="treeID">树标识</param>
        /// <param name="name">实体名称</param>
        /// <returns></returns>
        public int SetEntityCodeName(int treeID, string name)
        {
            var eTree = GetATree(treeID);
            int nameID = GetNameID(name);
            eTree.EntityCodeNameID = nameID;
            eTree.LastUpdatedTime = DateTime.Now;
            if (treeID > 100)
            {
                mdmdb.SaveChanges();
            }
            else
            {
                db.SaveChanges();
            }
            return nameID;
        }
        /// <summary>
        /// 删除一棵树
        /// </summary>
        /// <param name="treeID">树标识</param>
        public IRAPError DeleteATree(int treeID)
        {

            if (treeID > 100)
            {
                var treeEntity = treesRepoBiz.Table.FirstOrDefault(c => c.TreeID == treeID);
                treesRepoBiz.Entities.Remove(treeEntity);
                mdmdb.SaveChanges();
            }
            else
            {
                var treeEntity = treesRepoSys.Table.FirstOrDefault(c => c.TreeID == treeID);
                treesRepoSys.Entities.Remove(treeEntity);
                db.SaveChanges();
            }
            return new IRAPError(0, "删除成功！");
        }
        /// <summary>
        /// 修改一棵树：使用GetATree获取一棵树实体，直接对实体进行修改，调用此方法保存
        /// </summary>
        public void ModifyATree(int treeID)
        {
            if (treeID > 100)
            {
                mdmdb.SaveChanges();
            }
            else
            {
                db.SaveChanges();
            }
            
        }

        /// <summary>
        /// 获取所有树清单
        /// </summary>
        /// <returns></returns>
        public List<TreeModelDTO> GetAllTrees()
        {
            var trees = db.Set<ModelSysTreeEntity>();
            var namespaces = db.Set<SysNameSpaceEntity>().Where(c => c.PartitioningKey == 0 && c.LanguageID == 30);

            

            var treeList = from a in trees
                           join b in namespaces on a.NameID equals b.NameID
                           join c in namespaces on a.EntityCodeNameID equals c.NameID
                           join d in namespaces on a.AlternateCodeNameID equals d.NameID
                           orderby a.TreeID
                           select new TreeModelDTO
                           {
                               TreeID = a.TreeID,
                               TreeName = b.NameDescription,
                               TreeType = a.TreeType,
                               LeafLimit = a.LeafLimit,
                               GenAttrTBLName = a.EntityAttrTBName,
                               NodeAttrTBName = a.NodeAttrTBName,
                               PrimaryCodeName = c.NameDescription,
                               AlternateCodeName = d.NameDescription,
                               ShareToAll = a.ShareToAll,
                               UniqueEntityCode = a.UniqueEntityCode,
                               UniqueNodeCode = a.UniqueNodeCode,
                               OrderByMode = a.OrderByMode,
                               LastUpdatedTime = a.LastUpdatedTime,
                               AutoCodeGenerating = a.AutoCodeGenerating,
                               CommunityIndependent = a.CommunityIndependent,
                               DepthLimit = a.DepthLimit,
                               ExclusiveLevel = a.ExclusiveLevel
                           };

            var list = treeList.ToList();
            //再找大于100的树
            var trees2 = mdmdb.Set<ModelBizTreeEntity>();
            var namespaces2 = mdmdb.Set<SysNameSpaceMDMEntity>().Where(c => c.PartitioningKey == 0 && c.LanguageID == 30);
            var treeList2 = from a in trees2
                            join b in namespaces2 on a.NameID equals b.NameID
                            join c in namespaces2 on a.EntityCodeNameID equals c.NameID
                            join d in namespaces2 on a.AlternateCodeNameID equals d.NameID
                            orderby a.TreeID
                            select new TreeModelDTO
                            {
                                TreeID = a.TreeID,
                                TreeName = b.NameDescription,
                                TreeType = a.TreeType,
                                LeafLimit = a.LeafLimit,
                                GenAttrTBLName = a.EntityAttrTBName,
                                NodeAttrTBName = a.NodeAttrTBName,
                                PrimaryCodeName = c.NameDescription,
                                AlternateCodeName = d.NameDescription,
                                ShareToAll = a.ShareToAll,
                                UniqueEntityCode = a.UniqueEntityCode,
                                UniqueNodeCode = a.UniqueNodeCode,
                                OrderByMode = a.OrderByMode,
                                LastUpdatedTime = a.LastUpdatedTime,
                                AutoCodeGenerating = a.AutoCodeGenerating,
                                CommunityIndependent = a.CommunityIndependent,
                                DepthLimit = a.DepthLimit,

                            };
            foreach (var item in treeList2.ToList())
            {
                list.Add(item);
            }
            return list;
        }

    }
}
