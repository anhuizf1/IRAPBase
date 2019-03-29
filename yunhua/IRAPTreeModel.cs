using IRAPBase.Entities;
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
    public class IRAPTreeModel
    {
        private Repository<ModelTreeEntity> treesRepo = null;
        private IDbContext db = null;
        /// <summary>
        /// 构造函数
        /// </summary>
        public IRAPTreeModel()
        {
            db = DBContextFactory.Instance.CreateContext("IRAPContext");
            treesRepo = new Repository<ModelTreeEntity>(db);
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
        public void CreateATree(int treeID, string treeName, int leafLimit, int depthLimit, bool shareToAll, byte treeType = 2)
        {
            var treeEntity = treesRepo.Table.FirstOrDefault(c => c.TreeID == treeID);
            if (treeEntity != null)
            {
                throw new Exception($"已存在Tree={treeID}");
            }
            IIRAPNamespaceSet nameSet = IRAPNamespaceSetFactory.CreatInstance(Enums.NamespaceType.Sys);
            int nameID = nameSet.GetNameID(0, treeName, 30);
            var etree = new ModelTreeEntity()
            {
                TreeID = (short)treeID,
                NameID = nameID,
                LeafLimit = leafLimit,
                DepthLimit = depthLimit,
                ShareToAll = shareToAll,
                TreeType = treeType
            };
            treesRepo.Insert(etree);
            treesRepo.SaveChanges();
            return;
        }

        /// <summary>
        /// 获取一棵树
        /// </summary>
        /// <param name="treeID">树标识</param>
        /// <returns></returns>
        public ModelTreeEntity GetATree(int treeID)
        {
            var treeEntity = treesRepo.Table.FirstOrDefault(c => c.TreeID == treeID);
            if (treeEntity == null)
            {
                throw new Exception($"不存在Tree={treeID}");
            }
            return treeEntity;
        }


        public void SetEntityAttrTBName(int treeID,string tblName)
        {

        }

        public void SetNodeAttrTBName(int treeID,string attrTBName)
        {

        }
        /// <summary>
        /// 设置实体代码名称
        /// </summary>
        /// <param name="treeID"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public int SetEntityCodeName(int treeID, string name)
        {
            var eTree = GetATree(treeID);


            eTree.EntityCodeNameID = nameID;
            return 0;
        }

        /// <summary>
        /// 删除一棵树
        /// </summary>
        /// <param name="treeID">树标识</param>
        public void DeleteATree(int treeID)
        {
            throw new Exception("暂时不允许删除树！");
        }
        /// <summary>
        /// 修改一棵树：使用GetATree获取一棵树实体，直接对实体进行修改，调用此方法保存
        /// </summary>
        public void ModifyATree()
        {
            db.SaveChanges();
        }

    }
}
