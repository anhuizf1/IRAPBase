using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRAPBase;
using IRAPBase.Entities;
using IRAPBase.DTO;
using System.Reflection;
using IRAPBase.Serialize;
using System.Data.SqlClient;
using IRAPBase.Enums;

namespace IRAPBase
{
    /// <summary>
    /// 访问IRAP树的通用类
    /// </summary>
    public class IRAPTreeSet
    {

        private int _treeID = 0;
        private int _communityID;
        private IQueryable<TreeNodeEntity> nodes;
        private IQueryable<TreeLeafEntity> leaves;
        private IDbContext context = null;
        private List<IRAPTreeNodes> accessibleNodes;
        private IQueryable<TreeClassEntity> _treeClass;
        private IQueryable<TreeTransient> _treeTrans;
        private IQueryable<TreeStatus> _treeStatus;
        private IQueryable<NameSpaceEntity> _nameSpace;
        private long[] _PKDict;
        protected List<TreeClassifyModelDTO> _treeClassModel = null;
        protected List<TreeTransientModelDTO> _treeTransientModel = null;
        protected List<TreeStatusModelDTO> _treeStatusModel = null;
        protected IRAPTreeModel _treeModel = null;
        /// <summary>
        /// 树标识
        /// </summary>
        public int TreeID { get { return _treeID; } }
        /// <summary>
        /// 分区键
        /// </summary>
        public long PK { get { return _communityID * 10000L + _treeID; } }

        /// <summary>
        /// 模型
        /// </summary>
        public IRAPTreeModel TreeModel { get { return _treeModel; } }
        #region 属性
        /// <summary>
        /// 1.(标识)获取叶子集//叶子
        /// </summary>
        public IQueryable<TreeLeafEntity> LeafEntities { get { return leaves; } }
        /// <summary>
        /// 2.(标识/目录)获取节点集合 
        /// </summary>
        public IQueryable<TreeNodeEntity> NodeEntities { get { return nodes; } }
        /// <summary>
        /// 3检索属性
        /// </summary>
        public IQueryable<NameSpaceEntity> NameSpaceEntities { get { return _nameSpace; } }
        /// <summary>
        /// 4分类属性
        /// </summary>
        public IQueryable<TreeClassEntity> ClassEntities { get { return _treeClass; } }

        /// <summary>
        /// 5.瞬态属性
        /// </summary>
        public IQueryable<TreeTransient> TransientEntities { get { return _treeTrans; } }

        /// <summary>
        /// 6.状态属性
        /// </summary>
        public IQueryable<TreeStatus> Status { get { return _treeStatus; } }

        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="communityID">社区标识</param>
        /// <param name="treeID">树标识</param>
        public IRAPTreeSet(int communityID, int treeID)
        {
            _treeID = treeID;
            string dbConnectStr = string.Empty;
            if (_treeID <= 100)
            {
                dbConnectStr = "IRAPContext";
            }
            else
            {
                dbConnectStr = "IRAPMDMContext";
            }
            context = new IRAPSqlDBContext(dbConnectStr);
            Init(context, communityID, treeID);
        }
        /// <summary>
        /// 有数据库上下文的构造函数
        /// </summary>
        /// <param name="db"></param>
        /// <param name="communityID"></param>
        /// <param name="treeID"></param>
        public IRAPTreeSet(IDbContext db, int communityID, int treeID)
        {
            Init(db, communityID, treeID);
        }
        private void Init(IDbContext db, int communityID, int treeID)
        {
            _treeID = treeID;
            _communityID = communityID;
            _PKDict = new long[] { PK, _treeID };
            accessibleNodes = new List<IRAPTreeNodes>();
            context = db;
            if (_treeID <= 100)
            {
                nodes = new Repository<ETreeSysDir>(context).Table.Where(c => _PKDict.Contains(c.PartitioningKey) && c.TreeID == _treeID);
                leaves = new Repository<ETreeSysLeaf>(context).Table.Where(c => _PKDict.Contains(c.PartitioningKey) && c.TreeID == _treeID);
                //加载分类属性
                _treeClass = context.Set<ETreeSysClass>().Where(c => c.PartitioningKey == PK);
                //加载瞬态属性
                _treeTrans = context.Set<ETreeSysTran>().Where(c => c.PartitioningKey == PK);
                //加载状态属性
                _treeStatus = context.Set<ETreeSysStatus>().Where(c => c.PartitioningKey == PK);
                //检索属性
                _nameSpace = context.Set<SysNameSpaceEntity>().Where(c => c.PartitioningKey == PK);
            }
            else
            {
                if (_treeID > 100 && _treeID <= 1000)
                {
                    leaves = new Repository<ETreeBizLeaf>(context).Table.Where(c => _PKDict.Contains(c.PartitioningKey) && c.TreeID == _treeID);
                }
                else
                {
                    leaves = new Repository<ETreeRichLeaf>(context).Table.Where(c => _PKDict.Contains(c.PartitioningKey) && c.TreeID == _treeID);
                }
                nodes = new Repository<ETreeBizDir>(context).Table.Where(c => _PKDict.Contains(c.PartitioningKey) && c.TreeID == _treeID);

                //加载分类属性
                _treeClass = context.Set<ETreeBizClass>().Where(c => c.PartitioningKey == PK);
                //加载瞬态属性
                _treeTrans = context.Set<ETreeBizTran>().Where(c => c.PartitioningKey == PK);
                //加载状态属性
                _treeStatus = context.Set<ETreeBizStatus>().Where(c => c.PartitioningKey == PK);
                //检索属性
                _nameSpace = context.Set<BizNameSpaceEntity>().Where(c => c.PartitioningKey == PK);
            }
            //加载树模型
            _treeModel = new IRAPTreeModel(_treeID);
        }
        /// <summary>
        /// 获取数据库表集
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        protected DbSet TableSet(BaseEntity t)
        {
            return context.GetSet(t.GetType());
        }
        //7.一般属性
        /// <summary>
        /// 此树的所有一般属性集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IQueryable<T> GenAttr<T>() where T : BaseGenAttrEntity
        {
            return new Repository<T>(context).Table.Where(c => c.PartitioningKey == PK);
        }
        //8. 行集属性

        /// <summary>
        ///此树的第n个行集属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IQueryable<T> RowSet<T>() where T : BaseRowAttrEntity
        {
            return new Repository<T>(context).Table.Where(c => c.PartitioningKey == PK);
        }
        #region 查询
        /// <summary>
        /// 获取树视图（不带权限）
        /// </summary>
        /// <returns></returns>
        public List<TreeViewDTO> TreeView(int entryNode = 0, bool includeLeaves = true)
        {
            List<TreeViewDTO> resList = GetPlainTreeData(TreeViewData(entryNode, includeLeaves));
            return resList;
        }
        //动态创建泛型实例举例
        //public void Test()
        //{
        //    Type type = Type.GetType("IRAPBase.Entities.ERS_T1R1", true, true);
        //    var repositoryType = typeof(Repository<>);
        //   // IRepository  test;
        //    var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(type), context);  
        //    Console.WriteLine(type.FullName);
        //}
        /// <summary>
        /// 返回树的数据，支持懒加载 
        /// </summary>
        /// <returns></returns>
        private IRAPTreeNodes TreeViewData(int entryNode = 0, bool includingLeaves = true)
        {
            //long[] pkArray = { PK + _treeID, _treeID };
            int errCode = 0;
            string errText = "获取成功！";
            List<TreeNodeEntity> newNodelist = nodes.Where(r => r.TreeID == _treeID && _PKDict.Contains(r.PartitioningKey)).OrderBy(r => r.NodeDepth).ThenBy(r => r.NodeID).ToList();
            List<TreeNodeEntity> nodeSet = new List<TreeNodeEntity>();
            if (entryNode > 0)
            {
                TreeNodeEntity thisNode = newNodelist.FirstOrDefault(c => c.NodeID == entryNode);
                if (thisNode == null)
                {
                    errCode = 2;
                    errText = "入口结点无效！";
                    throw new Exception($"{errCode}-{errText}");
                }
                nodeSet = newNodelist.Where(c => c.NodeDepth > thisNode.NodeDepth).OrderBy(r => r.NodeDepth).ThenBy(r => r.NodeID).ToList();
                nodeSet.Insert(0, thisNode);
            }
            else
            {
                nodeSet = newNodelist;
            }

            List<TreeLeafEntity> leafSet = null;
            if (includingLeaves)
            {
                leafSet = leaves.Where(r => r.TreeID == _treeID && _PKDict.Contains(r.PartitioningKey)).OrderBy(r => r.UDFOrdinal).Take(50000).ToList();
            }

            TreeNodeEntity node = nodeSet.FirstOrDefault();
            IRAPTreeNodes rootNode = new IRAPTreeNodes();
            if (node != null)
            {
                rootNode.NodeID = node.NodeID;
                rootNode.NodeCode = node.Code;
                rootNode.NodeName = node.NodeName;
                rootNode.NodeDepth = node.NodeDepth;
                rootNode.NodeStatus = node.NodeStatus;
                rootNode.NodeType = 3;
                rootNode.CSTRoot = node.CSTRoot;
                rootNode.UDFOrdinal = node.UDFOrdinal;
                rootNode.TreeViewType = 2;
                rootNode.Parent = node.Father;
                rootNode.IconFile = node.IconID.ToString();
                rootNode.Accessibility = 0;
                FindNodeAddToNode(rootNode, nodeSet, leafSet);
            }
            return rootNode;
        }
        /// <summary>
        /// 获取树视图（带权限）
        /// </summary>
        /// <param name="agencyNode">机构标识（仅支持叶子）</param>
        /// <param name="roleNode">角色标识（</param>
        /// <returns></returns>
        public List<TreeViewDTO> AccessibleTreeView(int agencyNode, int roleNode)
        {
            IRAPGrant grant = new IRAPGrant(_communityID);
            accessibleNodes.Clear();
            List<EGrant> list = grant.GetGrantListByTree(_treeID, agencyNode, roleNode);

            IRAPTreeNodes rootTree = TreeViewData();

            DownTree(rootTree, list);
            foreach (var node in accessibleNodes)
            {
                UpTree(node);
            }
            //去掉不可访问的结点
            RemoveNoAccessible(rootTree);
            return GetPlainTreeData(rootTree);
        }

        /// <summary>
        /// 获取树视图（带权限带入口结点）
        /// </summary>
        /// <param name="entryNode"></param>
        /// <param name="agencyNode"></param>
        /// <param name="roleNode"></param>
        /// <returns></returns>
        public List<TreeViewDTO> AccessibleTreeView(int entryNode, int agencyNode, int roleNode)
        {
            IRAPGrant grant = new IRAPGrant(_communityID);
            accessibleNodes.Clear();
            List<EGrant> list = grant.GetGrantListByTree(_treeID, agencyNode, roleNode);

            IRAPTreeNodes rootTree = TreeViewData(entryNode);

            DownTree(rootTree, list);
            foreach (var node in accessibleNodes)
            {
                UpTree(node);
            }
            //去掉不可访问的结点
            RemoveNoAccessible(rootTree);
            return GetPlainTreeData(rootTree);
        }

        public List<AccessibleCSTDTO> AccessibleCSTs(string access_token, int treeID, int scenarioIndex)
        {
            //1- 查stb020
            throw new NotImplementedException();

            //向上追溯
            //向下追溯
            //合并结果
        }

        /// <summary>
        /// 获取树视图（根据令牌）
        /// </summary>
        /// <param name="access_token">登录token</param>
        /// <returns></returns>
        public List<TreeViewDTO> AccessibleTreeView(string access_token)
        {

            IRAPLog log = new IRAPLog();
            LoginEntity logE = log.GetLogIDByToken(access_token);
            if (logE == null)
            {
                int errCode = 99;
                string errText = "无法访问树数据，令牌无效！";
                throw new Exception($"{errCode}-{errText}");
            }
            return AccessibleTreeView(logE.AgencyLeaf, logE.RoleLeaf);
        }

        private List<TreeViewDTO> GetPlainTreeData(IRAPTreeNodes rootNode)
        {
            List<TreeViewDTO> rows = new List<TreeViewDTO>();
            TreeViewDTO item = new TreeViewDTO()
            {
                Accessibility = rootNode.Accessibility,
                CSTRoot = rootNode.CSTRoot,
                HelpMemoryCode = rootNode.HelpMemoryCode,
                IconFile = rootNode.IconFile,
                IconImage = rootNode.IconImage,
                NodeCode = rootNode.NodeCode,
                AlternateCode = rootNode.AlternateCode,
                NodeDepth = rootNode.NodeDepth,
                NodeID = rootNode.NodeID,
                NodeName = rootNode.NodeName,
                NodeStatus = rootNode.NodeStatus,
                NodeType = rootNode.NodeType,
                Parent = rootNode.Parent,
                SearchCode1 = rootNode.SearchCode1,
                SearchCode2 = rootNode.SearchCode2,
                SelectStatus = rootNode.SelectStatus,
                TreeViewType = rootNode.TreeViewType,
                UDFOrdinal = rootNode.UDFOrdinal
            };
            rows.Add(item);
            if (rootNode.Children == null)
            {
                return rows;
            }
            //if (rootNode.FatherNode == null)
            //{
            //    Console.WriteLine("{0}[{1}]", rootNode.NodeName, rootNode.NodeID);
            //}
            foreach (var r in rootNode.Children)
            {
                // Console.WriteLine(new string('-', r.NodeDepth * 2) + (r.NodeID < 0 ? "-" : "+") + "{0}[{1}] ", r.NodeName, r.NodeID);
                List<TreeViewDTO> list = GetPlainTreeData(r);
                foreach (TreeViewDTO c in list)
                {
                    TreeViewDTO item2 = new TreeViewDTO()
                    {
                        Accessibility = c.Accessibility,
                        CSTRoot = c.CSTRoot,
                        HelpMemoryCode = c.HelpMemoryCode,
                        IconFile = c.IconFile,
                        IconImage = c.IconImage,
                        NodeCode = c.NodeCode,
                        AlternateCode = c.AlternateCode,
                        NodeDepth = c.NodeDepth,
                        NodeID = c.NodeID,
                        NodeName = c.NodeName,
                        NodeStatus = c.NodeStatus,
                        NodeType = c.NodeType,
                        Parent = c.Parent,
                        SearchCode1 = c.SearchCode1,
                        SearchCode2 = c.SearchCode2,
                        SelectStatus = c.SelectStatus,
                        TreeViewType = c.TreeViewType,
                        UDFOrdinal = c.UDFOrdinal
                    };
                    rows.Add(c);
                }
            }
            return rows;
        }

        /*
        //获取可访问子树结点集
        public List<TreeNodeEntity> AccessibleSubTreeNodes(int nodeID, long sysLogID)
        {
            return null;
        }
        //获取可访问子树叶集 
        public List<TreeLeafEntity> AccessibleSubTreeLeaves(int nodeID, long sysLogID)
        {
            return null;
        }*/

        /// <summary>
        /// 获取指定结点的结点集（不带权限）用于呈现树的懒加载
        /// </summary>
        /// <param name="nodeID"></param>
        /// <returns></returns>
        public List<TreeNodeEntity> SubTreeNodes(int nodeID)
        {
            return nodes.Where(r => r.Father == nodeID).ToList();
        }

        /// <summary>
        /// 获取指定结点下面的叶集（不带权限） 用于呈现树的懒加载
        /// </summary>
        /// <param name="nodeID">入口结点</param>
        /// <returns>叶清单</returns>
        public List<TreeLeafEntity> SubTreeLeaves(int nodeID)
        {
            return leaves.Where(r => r.Father == nodeID).ToList();
        }

        #endregion
        #region 根据权限呈现树的代码
        //权限向下追溯
        private void DownTree(IRAPTreeNodes rootNode, List<EGrant> grantList)
        {
            //Console.WriteLine("根节点：{0}-{1}", rootNode.NodeID, rootNode.NodeName);
            if (rootNode.FatherNode == null)
            {
                if (grantList.Any(c => c.CSTRoot == rootNode.CSTRoot))
                {
                    rootNode.Accessibility = AccessibilityType.Radio ;
                    accessibleNodes.Add(rootNode);
                }
            }
            if (rootNode.Children == null)
            {
                //if (grantList.Any(c => c.CSTRoot == rootNode.CSTRoot))
                //{
                //    rootNode.Accessibility = 1;
                //    accessibleNodes.Add(rootNode);
                //}
                return;
            }
            bool subAccessible = false;
            if (rootNode.Accessibility ==  AccessibilityType.Radio)
            {
                subAccessible = true;
                accessibleNodes.Add(rootNode);
            }
            foreach (var r in rootNode.Children)
            {
                if (subAccessible)
                {
                    r.Accessibility =  AccessibilityType.Radio;
                }
                else
                {
                    if (grantList.Any(c => c.CSTRoot == r.CSTRoot))
                    {
                        r.Accessibility =  AccessibilityType.Radio;
                    }
                }
                DownTree(r, grantList);
                if (r.Accessibility ==  AccessibilityType.Radio)
                {
                    accessibleNodes.Add(r);
                }
            }
        }

        //移除看不到的树节点
        private void RemoveNoAccessible(IRAPTreeNodes rootNode)
        {
            //Console.WriteLine("根节点：{0}-{1}", rootNode.NodeID, rootNode.NodeName);

            if (rootNode.Children == null || rootNode.Children.Count == 0)
            {
                return;
            }
            var list = rootNode.Children;
            //var q = from c in list.ToArray() where c.Accessibility == 0 select c;
            //foreach (var ei in q)
            //{
            //    list.Remove(ei);       //删除了
            //    Console.WriteLine("节点{0}无权访问已删除！", ei.NodeID);
            //}
            list.RemoveAll(s => s.Accessibility == 0);
            var q2 = from c in list where c.Accessibility ==  AccessibilityType.Radio select c;
            foreach (var ei in q2)
            {
                RemoveNoAccessible(ei);
            }
        }
        //权限向上追溯
        private void UpTree(IRAPTreeNodes fatherNode)
        {
            if (fatherNode == null)
            {
                return;
            }
            if (fatherNode.Accessibility !=  AccessibilityType.Radio)
            {
                fatherNode.Accessibility =  AccessibilityType.Radio;
            }
            UpTree(fatherNode.FatherNode);
        }

        //向树中增加节点
        private void FindNodeAddToNode(IRAPTreeNodes root, List<TreeNodeEntity> nodeSet, List<TreeLeafEntity> leafSet)
        {
            var subList = nodeSet.Where(c => c.Father == root.NodeID).ToList();
            //bool exists = false;
            root.Children = new List<IRAPTreeNodes>();
            FindLeafAddToNode(root, leafSet);
            foreach (var r in subList)
            {
                // exists = true;
                IRAPTreeNodes node = new IRAPTreeNodes();
                node.NodeID = r.NodeID;
                node.NodeCode = r.Code;
                node.NodeName = r.NodeName;
                node.NodeDepth = r.NodeDepth;
                node.NodeStatus = r.NodeStatus;
                node.NodeType = 3;
                node.CSTRoot = r.CSTRoot;
                node.UDFOrdinal = r.UDFOrdinal;
                node.TreeViewType = 2;
                node.Parent = r.Father;
                node.IconFile = r.IconID.ToString();
                node.FatherNode = root;
                node.Accessibility =  AccessibilityType.Radio;
                node.AlternateCode = "";
                root.Children.Add(node);

                FindNodeAddToNode(node, nodeSet, leafSet);
            }
        }
        //向树中增加叶子
        private void FindLeafAddToNode(IRAPTreeNodes root, List<TreeLeafEntity> leafSet)
        {
            if (leafSet == null)
            {
                return;
            }
            var subLeaves = leafSet.Where(c => c.Father == root.NodeID).ToList();
            if (root.Children == null)
            {
                root.Children = new List<IRAPTreeNodes>();
            }
            foreach (var r in subLeaves)
            {
                // exists = true;
                IRAPTreeNodes node = new IRAPTreeNodes();
                node.NodeID = -r.LeafID;
                node.NodeCode = r.Code;
                node.NodeName = r.NodeName;
                node.NodeDepth = r.NodeDepth;
                node.NodeStatus = (byte)(r.LeafStatus > 0 ? 1 : 0);
                node.NodeType = 4;
                node.CSTRoot = r.CSTRoot;
                node.UDFOrdinal = r.UDFOrdinal;
                node.TreeViewType = 2;
                node.Parent = r.Father;
                node.IconFile = r.IconID.ToString();
                node.FatherNode = root;
                node.Accessibility =  AccessibilityType.Radio;
                node.AlternateCode = r.AlternateCode;
                root.Children.Add(node);
            }
        }

        #endregion

        /*
        /// <summary>
        /// 新增树结点
        /// </summary>
        /// <param name="nodeType">结点类型:3-结点 4-叶子</param>
        /// <param name="fatherNode">父结点</param>
        /// <param name="englishName">英文名称</param>
        ///  <param name="createBy">创建人代码</param>
        /// <param name="nodeCode">结点代码</param>
        /// <param name="alterNateCode">替代代码</param>
        /// <param name="nodeName">结点名称</param>
        /// <param name="udfOrdinal">结点所在位置序号</param>
        /// <returns></returns>
        public NewTreeNodeDTO AddNode(int nodeType, int fatherNode, string englishName, string createBy, string nodeCode, string alterNateCode,
            string nodeName, float udfOrdinal = 0F)
        {
            fatherNode = Math.Abs(fatherNode);
            IRAPTreeBase tree = new IRAPTreeBase(context, _communityID, _treeID);
            return tree.NewTreeNode(nodeType, fatherNode, nodeName, nodeCode, createBy, alterNateCode, englishName, udfOrdinal);
        }
        /// <summary>
        /// 删除树结点 
        /// </summary>
        /// <param name="nodeType">结点类型：3-结点 4-叶子</param>
        /// <param name="nodeID">结点标识</param>
        /// <returns></returns>
        public IRAPError DeleteNode(int nodeType, int nodeID)
        {
            IRAPTreeBase tree = new IRAPTreeBase(context, _communityID, _treeID);
            return tree.DeleteTreeNode(nodeType, nodeID);
        }
        /// <summary>
        /// 修改树结点
        /// </summary>
        /// <param name="nodeType">结点类型：3-结点 4-叶子</param>
        /// <param name="nodeID">结点标识</param>
        /// <param name="modifiedBy">修改人代码</param>
        /// <param name="englishName">英文描述</param>
        /// <param name="nodeCode">结点代码</param>
        /// <param name="nodeName">结点名称</param>
        /// <param name="alternateCode">替代代码</param>
        /// <param name="udfOrdinal">位置序号</param>
        /// <returns></returns>
        public IRAPError ModifyNode(int nodeType, int nodeID, string modifiedBy, string englishName, string nodeCode, string nodeName, string alternateCode, float udfOrdinal = 0F)
        {
            IRAPTreeBase tree = new IRAPTreeBase(context, _communityID, _treeID);
            return tree.ModifyTreeNode(nodeType, nodeID, nodeName, nodeCode, englishName, alternateCode, modifiedBy, udfOrdinal);
        }
        */

        #region 新增或修改属性
        /// <summary>
        /// 新增节点或叶子
        /// </summary>
        /// <param name="fatherNode">父结点</param>
        /// <param name="englishName">英文名称</param>
        /// <param name="createBy">创建人</param>
        /// <param name="nodeCode">结点代码</param>
        /// <param name="alterNateCode">替代代码</param>
        /// <param name="nodeName">结点名称</param>
        /// <param name="udfOrdinal">位置序号</param>
        /// <returns></returns>
        private NewTreeNodeDTO NewNode(int fatherNode, string englishName, string createBy,
            string nodeCode, string alterNateCode, string nodeName, float udfOrdinal = 0F)
        {
            NewTreeNodeDTO res = new NewTreeNodeDTO();

            if (nodeName.Trim() == string.Empty)
            {
                res.ErrCode = 22;
                res.ErrText = "结点名称不能为空！";
                return res;
            }
            long[] pkDict = new long[2] { PK, _treeID };
            res.PartitioningKey = PK;
            try
            {
                TreeNodeEntity father = null;
                if (fatherNode == 0)
                {
                    father = nodes.FirstOrDefault(r => pkDict.Contains(r.PartitioningKey));
                    if (father != null)
                    {
                        res.ErrCode = 93;
                        res.ErrText = "根节点已存在节点不允许创建根节点！FatherNode不能为0";
                        return res;
                    }
                }
                else
                {
                    father = nodes.FirstOrDefault(r => r.NodeID == fatherNode);
                }
                if (father == null && fatherNode != 0)
                {
                    res.ErrCode = 22;
                    res.ErrText = $"传入的父结点：{fatherNode}不存在！";
                    return res;
                }
                int nodeDepth = father == null ? 0 : father.NodeDepth + 1;

                //实体代码唯一性防错
                if (_treeModel.TreeEntity.UniqueNodeCode)
                {
                    if (nodes.Any(i => i.Code == nodeCode))
                    {
                        throw new Exception($"此树的结点代码不允许重复，代码 {nodeCode} 已经存在！");
                    }
                }

                TreeNodeEntity e = new TreeNodeEntity
                {
                    PartitioningKey = PK,
                    NameID = 0,
                    IconID = 0,
                    NodeDepth = (byte)nodeDepth,
                    NodeID = 0, //申请
                    NodeName = nodeName,
                    NodeStatus = 0,
                    TreeID = (short)_treeID,
                    UDFOrdinal = udfOrdinal,
                    Code = nodeCode,
                    CSTRoot = 0,
                    Father = fatherNode,
                    DescInEnglish = englishName,
                    CreatedBy = createBy,
                    CreatedOn = DateTime.Now,
                    ModifiedBy = "",
                    ModifiedOn = DateTime.Now,
                };
                string seqName = "NextSysNodeID";
                if (_treeID > 100)
                {
                    seqName = "NextUserNodeID";
                }
                SequenceValueDTO dto = IRAPSequence.GetSequence(seqName, 1);
                if (dto.ErrCode != 0)
                {
                    res.ErrCode = dto.ErrCode;
                    res.ErrText = dto.ErrText;
                    return res;
                }
                e.NodeID = (int)dto.SequenceValue;
                res.NewNodeID = e.NodeID;
                res.NewEntityID = 0;
                res.PartitioningKey = PK;
                if (father != null)
                {
                    e.CSTRoot = father.NodeID == 0 ? e.NodeID : father.CSTRoot;
                }
                else
                {
                    e.CSTRoot = e.NodeID;
                }

                TreeNodeEntity c;
                if (_treeID <= 100)
                {
                    c = e.CopyTo<ETreeSysDir>();
                }
                else
                {
                    c = e.CopyTo<ETreeBizDir>();
                }
                TableSet(c).Add(c);
                context.SaveChanges();
                res.ErrCode = 0;
                res.ErrText = "结点创建成功！ ";
                return res;
            }
            catch (Exception err)
            {
                //_db.RollBack();
                res.ErrCode = 9999;
                res.ErrText = $"创建结点发生错误:{err.Message}";
                return res;
            }
        }

        //新增叶子
        private NewTreeNodeDTO NewLeaf(int fatherNode, string englishName, string createBy,
            string nodeCode, string alterNateCode, string nodeName, float udfOrdinal = 0F)
        {
            NewTreeNodeDTO res = new NewTreeNodeDTO();
            //long[] pkDict = new long[2] { PK, _treeID };
            string seqLeafName = "NextSysLeafID";
            string seqEntityName = "NextSysEntityID";

            //实体代码唯一性防错
            if (_treeModel.TreeEntity.UniqueEntityCode)
            {
                if (leaves.Any(c => c.Code == nodeCode))
                {
                    throw new Exception($"实体代码不允许重复，代码 {nodeCode} 已经存在！");
                }
            }
            try
            {
                TreeNodeEntity father = nodes.FirstOrDefault(r => r.NodeID == fatherNode);
                if (father == null)
                {
                    res.ErrCode = 22;
                    res.ErrText = $"父结点：{fatherNode}不存在！";
                    return res;
                }
                TreeLeafEntity e = new TreeLeafEntity
                {
                    PartitioningKey = PK,
                    NameID = 0,
                    IconID = 0,
                    AlternateCode = alterNateCode,
                    NodeDepth = (byte)(father.NodeDepth + 1),
                    LeafID = 0, //申请
                    NodeName = nodeName,
                    LeafStatus = 0,
                    TreeID = (short)_treeID,
                    UDFOrdinal = udfOrdinal,
                    Code = nodeCode,
                    CSTRoot = father.CSTRoot,
                    Father = fatherNode,
                    DescInEnglish = englishName,
                    CreatedBy = createBy,
                    CreatedOn = DateTime.Now,
                    ModifiedBy = createBy,
                    ModifiedOn = DateTime.Now
                };
                if (_treeID > 100)
                {
                    seqLeafName = "NextUserLeafID";
                    seqEntityName = "NextUserEntityID";
                }
                SequenceValueDTO dto = IRAPSequence.GetSequence(seqLeafName, 1);
                if (dto.ErrCode != 0)
                {
                    res.ErrCode = dto.ErrCode;
                    res.ErrText = dto.ErrText;
                    return res;
                }
                e.LeafID = (int)dto.SequenceValue;
                SequenceValueDTO dto2 = IRAPSequence.GetSequence(seqEntityName, 1);
                //为了保证LeafID与EntityID绝对一致性，用同一个序列值
                e.EntityID = e.LeafID; // (int)dto2.SequenceValue;
                TreeLeafEntity c;
                if (_treeID <= 100)
                {
                    c = e.CopyTo<ETreeSysLeaf>();
                }
                else if (_treeID > 100 && _treeID <= 1000)
                {
                    c = e.CopyTo<ETreeBizLeaf>();
                }
                else
                {
                    c = e.CopyTo<ETreeRichLeaf>();
                }

                TableSet(c).Add(c);
                //初始化分类属性清单
                if (_treeClassModel == null)
                {
                    _treeClassModel = _treeModel.GetClassify();
                }
                foreach (var r in _treeClassModel)
                {
                    TreeClassEntity t4Attr = new TreeClassEntity
                    {
                        AttrTreeID = (short)r.AttrTreeID,
                        Ordinal = r.AttrIndex,
                        LeafID = e.LeafID,
                        PartitioningKey = PK,
                        VersionLE = (int)(Math.Pow(2, 31) - 1),
                        TransactNoLE = 9223372036854775807,
                        A4NameID = 0
                    };
                    TreeClassEntity t4e;
                    TreeClassEntity t4eh;
                    if (_treeID <= 100)
                    {
                        t4e = t4Attr.CopyTo<ETreeSysClass>();
                        t4eh = t4Attr.CopyTo<ETreeSysClass_H>();
                    }
                    else
                    {
                        t4e = t4Attr.CopyTo<ETreeBizClass>();
                        t4eh = t4Attr.CopyTo<ETreeBizClass_H>();
                    }
                    TableSet(t4e).Add(t4e);
                    TableSet(t4eh).Add(t4eh);
                }
                //初始化瞬态属性
                if (_treeTransientModel == null)
                {
                    _treeTransientModel = _treeModel.GetTransient();
                }
                foreach (var r in _treeTransientModel)
                {
                    TreeTransient t6Attr = new TreeTransient
                    {
                        AttrValue = 0,
                        EntityID = e.EntityID,
                        Scale = r.Scale,
                        UnitOfMeasure = r.UnitOfMeasure,
                        VersionLE = (int)(Math.Pow(2, 31) - 1),
                        Ordinal = (byte)r.AttrIndex,
                        PartitioningKey = PK
                    };
                    TreeTransient t6e;
                    TreeTransient t6eh;
                    if (_treeID <= 100)
                    {
                        t6e = t6Attr.CopyTo<ETreeSysTran>();
                        t6eh = t6Attr.CopyTo<ETreeSysTran_H>();
                    }
                    else
                    {
                        t6e = t6Attr.CopyTo<ETreeBizTran>();
                        t6eh = t6Attr.CopyTo<ETreeBizTran_H>();
                    }
                    TableSet(t6e).Add(t6e);
                    TableSet(t6eh).Add(t6eh);
                }
                //初始化状态属性
                if (_treeStatusModel == null)
                {
                    _treeStatusModel = _treeModel.GetStatus();
                }
                foreach (var r in _treeStatusModel)
                {
                    TreeStatus eAttr = new TreeStatus
                    {
                        EntityID = e.EntityID,
                        T5LeafID = r.T5LeafID,
                        StatusValue = 0,
                        Ordinal = (byte)r.AttrIndex,
                        PartitioningKey = PK
                    };
                    TreeStatus t5e;
                    TreeStatus t5eh;
                    if (_treeID <= 100)
                    {
                        t5e = eAttr.CopyTo<ETreeSysStatus>();
                        t5eh = eAttr.CopyTo<ETreeSysStatus_H>();
                    }
                    else
                    {
                        t5e = eAttr.CopyTo<ETreeBizStatus>();
                        t5eh = eAttr.CopyTo<ETreeBizStatus_H>();
                    }
                    TableSet(t5e).Add(t5e);
                    TableSet(t5eh).Add(t5eh);
                }
                //初始化一般属性
                DbContextTransaction tran = null;
                bool isOutTran = false;
                if (context.DataBase.CurrentTransaction == null)
                {
                    tran = context.DataBase.BeginTransaction();
                }
                else
                {
                    isOutTran = true;
                    tran = context.DataBase.CurrentTransaction;
                }
                //using (var dbContextTransaction = context.DataBase.BeginTransaction())
                // {
                try
                {
                    context.SaveChanges();
                    if (_treeModel.AttrTBLName.Length > 3)
                    {
                        //这里一段怀疑是与SQLServer数据库相关，如果切换其他类型数据库可能会有问题。
                        context.DataBase.ExecuteSqlCommand($"insert into {_treeModel.AttrTBLName}(PartitioningKey ,EntityID) values(@p1,@p2)"
                            , new SqlParameter("@p1", PK), new SqlParameter("@p2", e.EntityID));
                    }
                    if (!isOutTran)
                    {
                        tran.Commit();
                    }
                    res.NewNodeID = e.LeafID;
                    res.NewEntityID = e.LeafID;
                    res.PartitioningKey = PK;
                    res.ErrCode = 0;
                    res.ErrText = $"新增叶结点成功！叶结点标识：{e.LeafID} ";
                }
                catch (Exception err)
                {
                    tran.Rollback();
                    res.ErrCode = 9999;
                    res.ErrText = $"新增叶结点失败！原因是：{err.Message} ";
                }
                // }
                return res;
            }
            catch (Exception err)
            {
                res.ErrCode = 9999;
                res.ErrText = $"创建叶子时发生错误:{err.Message}";
                return res;
            }
        }

        /// <summary>
        /// 新增结点或叶
        /// </summary>
        /// <param name="nodeType">结点类型：3-结点 4-叶子</param>
        /// <param name="fatherNode">父结点标识</param>
        /// <param name="nodeName">结点(叶)名称</param>
        /// <param name="nodeCode">结点(叶)代码</param>
        /// <param name="createBy">创建人代码</param>
        /// <param name="alterNateCode">替代代码</param>
        /// <param name="englishName">英文名称</param>
        /// <param name="udfOrdinal">插入位置序号</param>
        /// <returns></returns>
        public NewTreeNodeDTO NewTreeNode(int nodeType, int fatherNode,
            string nodeName, string nodeCode, string createBy, string alterNateCode = "", string englishName = "", float udfOrdinal = 0F)
        {
            fatherNode = Math.Abs(fatherNode);
            NewTreeNodeDTO res = new NewTreeNodeDTO();
            if (nodeType == 3)
            {
                return NewNode(fatherNode, englishName, createBy, nodeCode, alterNateCode, nodeName, udfOrdinal);
            }
            else if (nodeType == 4)
            {
                return NewLeaf(fatherNode, englishName, createBy, nodeCode, alterNateCode, nodeName, udfOrdinal);
            }
            else
            {
                res.ErrCode = 91;
                res.ErrText = "您传入的参数NodeType不正确，3=结点 4=叶子，其他类型不支持！";
                res.NewNodeID = 0;
            }
            return res;
        }

        /// <summary>
        /// 修改结点或叶子
        /// </summary>
        /// <param name="nodeType">结点类型：3-结点 4-叶子</param>
        /// <param name="nodeID">结点标识</param>
        /// <param name="nodeName">名称</param>
        /// <param name="nodeCode">代码</param>
        /// <param name="englishName">英文名称</param>
        /// <param name="alternateCode">替代代码</param>
        /// <param name="modifiedBy">修改人代码</param>
        /// <param name="udfOrdinal">序号</param>
        /// <returns></returns>
        public IRAPError ModifyTreeNode(int nodeType, int nodeID, string nodeName, string nodeCode = "", string englishName = "", string alternateCode = "", string modifiedBy = "", float udfOrdinal = 1F)
        {
            IRAPError error = new IRAPError();
            try
            {
                if (nodeName == string.Empty)
                {
                    error.ErrCode = 23;
                    error.ErrText = $"结点名称不能为空！";
                    return error;
                }
                if (nodeID < 0)
                {
                    nodeID = -nodeID;
                }
                if (nodeType == 3)
                {
                    var thisNode = nodes.FirstOrDefault(r => r.NodeID == nodeID);
                    if (thisNode == null)
                    {
                        error.ErrCode = 22;
                        error.ErrText = $"结点标识：{nodeID}不存在或无权限修改！";
                        return error;
                    }
                    if (_treeModel.TreeEntity.UniqueNodeCode)
                    {
                        if (nodes.Any(i => i.Code == nodeCode && i.NodeID != nodeID))
                        {
                            throw new Exception($"此树的结点代码不允许重复，代码 {nodeCode} 已经存在！");
                        }
                    }
                    thisNode.Code = nodeCode;
                    thisNode.NodeName = nodeName;
                    thisNode.UDFOrdinal = udfOrdinal;
                    thisNode.DescInEnglish = englishName;
                    thisNode.ModifiedOn = DateTime.Now;
                    thisNode.ModifiedBy = modifiedBy;
                    context.SaveChanges();
                }
                else if (nodeType == 4)
                {
                    var thisNode = leaves.FirstOrDefault(r => r.LeafID == nodeID);
                    if (thisNode == null)
                    {
                        error.ErrCode = 22;
                        error.ErrText = $"结点标识：{nodeID}不存在！";
                        return error;
                    }

                    if (_treeModel.TreeEntity.UniqueEntityCode)
                    {
                        if (leaves.Any(i => i.Code == nodeCode && i.LeafID != nodeID))
                        {
                            throw new Exception($"此树的实体代码不允许重复，代码 {nodeCode} 已经存在！");
                        }
                    }

                    thisNode.Code = nodeCode;
                    thisNode.AlternateCode = alternateCode;
                    thisNode.NodeName = nodeName;
                    thisNode.UDFOrdinal = udfOrdinal;
                    thisNode.DescInEnglish = englishName;
                    thisNode.ModifiedBy = modifiedBy;
                    thisNode.ModifiedOn = DateTime.Now;

                    context.SaveChanges();
                }
                else
                {
                    error.ErrCode = 91;
                    error.ErrText = "您输入的参数NodeType仅支持3=结点 4=叶子，不支持的其他类型！";
                    return error;
                }
                error.ErrCode = 0;
                error.ErrText = "修改结点成功！";
                return error;
            }
            catch (Exception err)
            {
                error.ErrCode = 9999;
                error.ErrText = $"修改结点失败：{err.Message}";
                return error;
            }
        }
        /// <summary>
        /// 删除结点或叶子
        /// </summary>
        /// <param name="nodeType">结点类型：3-结点 4-叶子</param>
        /// <param name="nodeID">结点标识</param>
        /// <returns></returns>
        public IRAPError DeleteTreeNode(int nodeType, int nodeID)
        {
            IRAPError error = new IRAPError();
            // long[] pkDict = new long[2] { PK, _treeID };
            if (nodeID < 0)
            {
                nodeID = -nodeID;
            }
            try
            {
                if (nodeType == 3)
                {
                    var thisNode = nodes.FirstOrDefault(r => r.NodeID == nodeID);
                    if (thisNode == null)
                    {
                        error.ErrCode = 22;
                        error.ErrText = $"结点标识：{nodeID}不存在！";
                        return error;
                    }
                    if (thisNode.PartitioningKey == _treeID)
                    {
                        error.ErrCode = 22;
                        error.ErrText = $"结点标识：{nodeID}属于系统结点暂不允许删除！";
                        return error;
                    }
                    var tempNode = nodes.FirstOrDefault(c => c.Father == nodeID);
                    if (tempNode != null)
                    {
                        error.ErrCode = 23;
                        error.ErrText = $"结点“{tempNode.NodeName}”下面已有结点，请先删除子结点！";
                        return error;
                    }

                    var templeafSet = leaves.FirstOrDefault(c => c.Father == nodeID);
                    if (templeafSet != null)
                    {
                        error.ErrCode = 25;
                        error.ErrText = $"结点“{tempNode.NodeName}”下面已有叶结点，请先删除叶结点再删除此结点！";
                        return error;
                    }

                    TableSet(thisNode).Remove(thisNode);
                    context.SaveChanges();
                }
                else if (nodeType == 4)
                {
                    var thisNode = leaves.FirstOrDefault(r => r.LeafID == nodeID);
                    if (thisNode == null)
                    {
                        error.ErrCode = 22;
                        error.ErrText = $"叶结点标识：{nodeID}不存在！";
                        return error;
                    }
                    //防错，如果被其他分类属性引用则报错
                    var obj = context.DataBase.SqlQuery<TreeClassEntity>("select *from IRAP..stb197 where A4LeafID=@NodeID " +
                        "union all select * from IRAPMDM..stb198 where A4LeafID=@NodeID", new SqlParameter("@NodeID", nodeID)).ToList();


                    if (obj.Count > 0)
                    {
                        var obj2 = obj[0];
                        error.ErrCode = 22;
                        error.ErrText = $"LeafID={nodeID}被系统分类属性 {obj2.LeafID} 引用无法删除！";
                        return error;
                    }
                    TableSet(thisNode).Remove(thisNode);
                    context.SaveChanges();
                }
                else
                {
                    error.ErrCode = 91;
                    error.ErrText = "输入参数NodeType只允许=3 结点 =4 叶子 不支持其他的类型！";
                    return error;
                }
                error.ErrCode = 0;
                error.ErrText = "删除结点成功！";
                return error;
            }
            catch (Exception err)
            {
                error.ErrCode = 9999;
                error.ErrText = $"删除结点失败：{err.Message}";
                return error;
            }
        }

        #endregion 
        /// <summary>
        /// 根据指定分类属性获取叶子清单
        /// </summary>
        /// <param name="ordinal"></param>
        /// <param name="cleafID"></param>
        /// <returns></returns>
        public IQueryable<TreeLeafEntity> GetLeafSetByClassify(int ordinal, int cleafID)
        {
            IQueryable<TreeClassEntity> treeClass = _treeClass.Where(c => c.A4LeafID == cleafID && c.Ordinal == ordinal);
            return treeClass.Join(leaves, a => a.LeafID, b => b.LeafID, (a, b) => b);
        }

        /// <summary>
        /// 根据叶子清单获取分类属性集合
        /// </summary>
        /// <param name="attrIndex">属性序号</param>
        /// <param name="leafSet">叶子集合</param>
        /// <returns></returns>
        public IQueryable<TreeClassEntity> GetClassifySet(byte attrIndex, List<int> leafSet)
        {
            return _treeClass.Where(c => c.Ordinal == attrIndex && leafSet.Contains(c.LeafID));
        }

        public List<TreeClassifyRowDTO> GetClassifySet(List<int> leafSet)
        {
            var list2 = from s in _treeClass.Where(c => leafSet.Contains(c.LeafID))
                        //where s.Ordinal == (int)attrIndex
                        group s by s.LeafID into g
                        select new { g.Key, Rows = g };
            var resList = new List<TreeClassifyRowDTO>();
            list2.ToList().ForEach(x =>
            {
                TreeClassifyRowDTO r = new TreeClassifyRowDTO()
                {
                    TreeID= _treeID,
                    LeafID = x.Key,
                    Leaf01 = x.Rows.FirstOrDefault(c => c.Ordinal == 1) == null ? 0 : x.Rows.FirstOrDefault(c => c.Ordinal == 1).A4LeafID,
                    Leaf02 = x.Rows.FirstOrDefault(c => c.Ordinal == 2) == null ? 0 : x.Rows.FirstOrDefault(c => c.Ordinal == 2).A4LeafID,
                    Leaf03 = x.Rows.FirstOrDefault(c => c.Ordinal == 3) == null ? 0 : x.Rows.FirstOrDefault(c => c.Ordinal == 3).A4LeafID,
                    Leaf04 = x.Rows.FirstOrDefault(c => c.Ordinal == 4) == null ? 0 : x.Rows.FirstOrDefault(c => c.Ordinal == 4).A4LeafID,
                    Leaf05 = x.Rows.FirstOrDefault(c => c.Ordinal == 5) == null ? 0 : x.Rows.FirstOrDefault(c => c.Ordinal == 5).A4LeafID,
                    Leaf06 = x.Rows.FirstOrDefault(c => c.Ordinal == 6) == null ? 0 : x.Rows.FirstOrDefault(c => c.Ordinal == 6).A4LeafID,
                    Leaf07 = x.Rows.FirstOrDefault(c => c.Ordinal == 7) == null ? 0 : x.Rows.FirstOrDefault(c => c.Ordinal == 7).A4LeafID,
                    Leaf08 = x.Rows.FirstOrDefault(c => c.Ordinal == 8) == null ? 0 : x.Rows.FirstOrDefault(c => c.Ordinal == 8).A4LeafID,
                    Leaf09 = x.Rows.FirstOrDefault(c => c.Ordinal == 9) == null ? 0 : x.Rows.FirstOrDefault(c => c.Ordinal == 9).A4LeafID,
                    Leaf10 = x.Rows.FirstOrDefault(c => c.Ordinal == 10) == null ? 0 : x.Rows.FirstOrDefault(c => c.Ordinal == 10).A4LeafID,
                    Leaf11 = x.Rows.FirstOrDefault(c => c.Ordinal == 11) == null ? 0 : x.Rows.FirstOrDefault(c => c.Ordinal == 11).A4LeafID,
                    Leaf12 = x.Rows.FirstOrDefault(c => c.Ordinal == 12) == null ? 0 : x.Rows.FirstOrDefault(c => c.Ordinal == 12).A4LeafID,
                };
                resList.Add(r);

            });
            return resList;
        }
        /// <summary>
        /// 根据社区号,树标识，叶标识查询一个实体对象，找不到返回null
        /// </summary>
        /// <param name="communityID">社区编号</param>
        /// <param name="treeID">树标识</param>
        /// <param name="leafID">叶标识</param>
        /// <returns></returns>

        public static TreeLeafEntity GetLeafEntity(int communityID, int treeID, int leafID)
        {
            long pk = communityID * 10000L + treeID;
            long[] pkDict = new long[] { pk, treeID };
            IDbContext db;
            if (treeID <= 100)
            {
                db = DBContextFactory.Instance.CreateContext("IRAPContext");
                return db.Set<ETreeSysLeaf>().FirstOrDefault(c => pkDict.Contains(c.PartitioningKey) && (short)treeID == c.TreeID && leafID == c.LeafID);
            }
            else if (treeID > 100 && treeID <= 1000)
            {
                db = DBContextFactory.Instance.CreateContext("IRAPMDMContext");
                return db.Set<ETreeBizLeaf>().FirstOrDefault(c => pkDict.Contains(c.PartitioningKey) && (short)treeID == c.TreeID && leafID == c.LeafID);
            }
            else
            {
                db = DBContextFactory.Instance.CreateContext("IRAPMDMContext");
                return db.Set<ETreeRichLeaf>().FirstOrDefault(c => pkDict.Contains(c.PartitioningKey) && (short)treeID == c.TreeID && leafID == c.LeafID);
            }
        }


        /// <summary>
        /// 根据Code模糊找出叶子清单
        /// </summary>
        /// <param name="communityID">社区标识</param>
        /// <param name="treeID">树标识</param>
        /// <param name="code">代码</param>
        /// <returns></returns>
        public static IQueryable<TreeLeafEntity> GetLeafSetByCode(int communityID, int treeID, string code)
        {
            IDbContext db;
            long pk = communityID * 10000L + treeID;
            if (treeID <= 100)
            {
                db = DBContextFactory.Instance.CreateContext("IRAPContext");
                return db.Set<ETreeSysLeaf>().Where(c => c.PartitioningKey == pk && (short)treeID == c.TreeID && c.Code.Contains(code));
            }
            else if (treeID > 100 && treeID <= 1000)
            {
                db = DBContextFactory.Instance.CreateContext("IRAPMDMContext");
                return db.Set<ETreeBizLeaf>().Where(c => c.PartitioningKey == pk && (short)treeID == c.TreeID && c.Code.Contains(code));
            }
            else
            {
                db = DBContextFactory.Instance.CreateContext("IRAPMDMContext");
                return db.Set<ETreeRichLeaf>().Where(c => c.PartitioningKey == pk && (short)treeID == c.TreeID && c.Code.Contains(code));
            }
        }

        public static List<TreeClassEntity> GetLeafSetByDimCode(int communityID, int treeID, string dimCode, List<int> listSet)
        {


            //string dimCode = "01020309";
            //解析完整的分类属性链
            int len = dimCode.Length;
            Int16 trueTreeID = (Int16)treeID;
            Dictionary<byte, TreeDimDTO> treeList = new Dictionary<byte, TreeDimDTO>();
            byte j = 0;
            for (int i = 0; i < len; i += 2)
            {

                IRAPTreeModel treeModel = new IRAPTreeModel(trueTreeID);
                var list = treeModel.GetClassify();
                byte index = byte.Parse(dimCode.Substring(i, 2));
                trueTreeID = (Int16)(list[index].AttrTreeID);
                j++;
                TreeDimDTO dim = new TreeDimDTO() { Index = index, TreeID = trueTreeID };
                treeList.Add(j, dim);
            }
            //对分类属性进行倒叙过滤
            j = 0;
            var newList = treeList.OrderByDescending(c => c.Key).ToList();
            List<TreeClassEntity> lastSet = null;
            foreach (var r in newList)
            {
                j++;
                var db = DBContextFactory.Instance.CreateContext("IRAPContext");
                if (j == 1)
                {
                    lastSet = db.Set<ETreeSysClass>().Cast<TreeClassEntity>()
                        .Where(c => c.AttrTreeID == r.Value.TreeID && c.Ordinal == r.Value.Index && listSet.Contains(c.A4LeafID)).ToList();
                }
                else
                {
                    var thisList = db.Set<ETreeSysClass>().Cast<TreeClassEntity>()
                        .Where(c => c.AttrTreeID == r.Value.TreeID && c.Ordinal == r.Value.Index).ToList();

                    lastSet = (from a in lastSet
                               join b in thisList on new { LeafID = a.A4LeafID } equals new { b.LeafID }
                               where a.AttrTreeID == r.Value.TreeID
                               select a).ToList();
                }
            }
            return lastSet;
            //  throw new NotImplementedException();

        }

        /// <summary>
        /// 根据叶标识查询实体
        /// </summary>
        /// <param name="leafID">叶标识</param>
        /// <returns></returns>
        public static TreeLeafEntity GetLeafEntity(int leafID)
        {
            IDbContext db = DBContextFactory.Instance.CreateContext("IRAPMDMContext");
            var leafEntity = db.Set<ETreeBizLeaf>().FirstOrDefault(c => leafID == c.LeafID);
            if (leafEntity == null)
            {
                db = DBContextFactory.Instance.CreateContext("IRAPContext");
                return db.Set<ETreeSysLeaf>().FirstOrDefault(c => leafID == c.LeafID);
            }
            else
            {
                return leafEntity;
            }
        }


    }

    /// <summary>
    /// 树基本数据结构
    /// </summary>
    public class IRAPTreeNodes
    {
        /// <summary>
        /// 结点标识
        /// </summary>
        public int NodeID { get; set; }          //  --结点标识
        /// <summary>
        /// 树视图类型
        /// </summary>
        public byte TreeViewType { get; set; }    // --树视图类型
        /// <summary>
        /// 结点类型3-结点 4-叶子
        /// </summary>
        public byte NodeType { get; set; }        //  --结点类型
        /// <summary>
        /// 结点代码
        /// </summary>
        public string NodeCode { get; set; }      //   --结点代码
        /// <summary>
        /// 替代代码
        /// </summary>
        public string AlternateCode { get; set; }
        /// <summary>
        /// 结点名称
        /// </summary>
        public string NodeName { get; set; }       //  --结点名称
        /// <summary>
        /// 父结点标识
        /// </summary>
        public int Parent { get; set; }            //  --
        /// <summary>
        /// 结点深度
        /// </summary>
        public byte NodeDepth { get; set; }         // --结点深度
        /// <summary>
        /// 权限控制点
        /// </summary>
        public int CSTRoot { get; set; }
        /// <summary>
        /// 自定义遍历序
        /// </summary>
        public float UDFOrdinal { get; set; }
        /// <summary>
        /// 结点状态-- =0 正常  =1 失效 小于0 未生效
        /// </summary>
        public byte NodeStatus { get; set; }
        /// <summary>
        /// 可访问性  0=不可选  1=可单选  2=可复选
        /// </summary>
        public AccessibilityType Accessibility { get; set; }
        /// <summary>
        /// 选中状态  0=未选中  1=已选中
        /// </summary>
        public SelectStatusType SelectStatus { get; set; }       // 
        /// <summary>
        /// 第一检索码
        /// </summary>
        public string SearchCode1 { get; set; }
        /// <summary>
        /// 第二检索码
        /// </summary>
        public string SearchCode2 { get; set; }
        /// <summary>
        /// 助记码
        /// </summary>
        public string HelpMemoryCode { get; set; }
        /// <summary>
        /// 图标文件名
        /// </summary>
        public string IconFile { get; set; }
        /// <summary>
        /// 图标图像流
        /// </summary>
        public byte[] IconImage { get; set; }
        /// <summary>
        /// 父结点（遍历用）
        /// </summary>

        public IRAPTreeNodes FatherNode { get; set; }
        /// <summary>
        /// 子结点清单(遍历用）
        /// </summary>
        public List<IRAPTreeNodes> Children { get; set; }
    }
}
