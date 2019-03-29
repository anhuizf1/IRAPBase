using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRAPBase;
using IRAPBase.Entities;
using IRAPBase.DTO;

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
 
        /// <summary>
        /// 树标识
        /// </summary>
        public int TreeID { get { return _treeID; } }
        /// <summary>
        /// 分区键
        /// </summary>
        public long PK { get { return _communityID * 10000L; } }

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
            _communityID = communityID;
            _PKDict = new long[] { PK + _treeID, _treeID };
            string dbConnectStr = string.Empty;
            if (_treeID <= 100)
            {
                dbConnectStr = "IRAPContext";
            }
            else
            {
                dbConnectStr = "IRAPMDMContext";
            }
            accessibleNodes = new List<IRAPTreeNodes>();
            context = new IRAPSqlDBContext(dbConnectStr);
            if (_treeID <= 100)
            {
                nodes = new Repository<ETreeSysDir>(context).Table.Where(c => _PKDict.Contains(c.PartitioningKey) && c.TreeID == _treeID);
                leaves = new Repository<ETreeSysLeaf>(context).Table.Where(c => c.PartitioningKey == PK + _treeID && c.TreeID == _treeID);
                //加载分类属性
                _treeClass = context.Set<ETreeSysClass>().Where(c => c.PartitioningKey == PK + _treeID);
                //加载瞬态属性
                _treeTrans = context.Set<ETreeSysTran>().Where(c => c.PartitioningKey == PK + _treeID);
                //加载状态属性
                _treeStatus = context.Set<ETreeSysStatus>().Where(c => c.PartitioningKey == PK + _treeID);
                //检索属性
                _nameSpace = context.Set<SysNameSpaceEntity>().Where(c => c.PartitioningKey == PK);
            }
            else
            {
                nodes = new Repository<ETreeBizDir>(context).Table.Where(c => _PKDict.Contains(c.PartitioningKey) && c.TreeID == _treeID);
                leaves = new Repository<ETreeBizLeaf>(context).Table.Where(c => c.PartitioningKey == PK + _treeID && c.TreeID == _treeID);
                //加载分类属性
                _treeClass = context.Set<ETreeBizClass>().Where(c => c.PartitioningKey == PK + _treeID);
                //加载瞬态属性
                _treeTrans = context.Set<ETreeBizTran>().Where(c => c.PartitioningKey == PK + _treeID);
                //加载状态属性
                _treeStatus = context.Set<ETreeBizStatus>().Where(c => c.PartitioningKey == PK + _treeID);
                //检索属性
                _nameSpace = context.Set<BizNameSpaceEntity>().Where(c => c.PartitioningKey == PK);
            }
        }


        //7.一般属性
        /// <summary>
        /// 此树的所有一般属性集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IQueryable<T> GenAttr<T>() where T : BaseGenAttrEntity
        {
            return new Repository<T>(context).Table.Where(c => c.PartitioningKey == PK + _treeID);
        }
        //8. 行集属性

        /// <summary>
        ///此树的第n个行集属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IQueryable<T> RowSet<T>() where T : BaseRowAttrEntity
        {
            return new Repository<T>(context).Table.Where(c => c.PartitioningKey == PK + _treeID);
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
                    rootNode.Accessibility = 1;
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
            if (rootNode.Accessibility == 1)
            {
                subAccessible = true;
                accessibleNodes.Add(rootNode);
            }
            foreach (var r in rootNode.Children)
            {
                if (subAccessible)
                {
                    r.Accessibility = 1;
                }
                else
                {
                    if (grantList.Any(c => c.CSTRoot == r.CSTRoot))
                    {
                        r.Accessibility = 1;
                    }
                }
                DownTree(r, grantList);
                if (r.Accessibility == 1)
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
            var q2 = from c in list where c.Accessibility == 1 select c;
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
            if (fatherNode.Accessibility != 1)
            {
                fatherNode.Accessibility = 1;
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
                node.Accessibility = 0;
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
                node.NodeStatus = 0;
                node.NodeType = 4;
                node.CSTRoot = r.CSTRoot;
                node.UDFOrdinal = r.UDFOrdinal;
                node.TreeViewType = 2;
                node.Parent = r.Father;
                node.IconFile = r.IconID.ToString();
                node.FatherNode = root;
                node.Accessibility = 0;
                node.AlternateCode = r.AlternateCode;
                root.Children.Add(node);
            }
        }

        #endregion
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
        /// <summary>
        /// 根据指定分类属性获取叶子清单
        /// </summary>
        /// <param name="ordinal"></param>
        /// <param name="cleafID"></param>
        /// <returns></returns>
        public IQueryable<TreeLeafEntity> GetLeafSetByClassify(int ordinal, int cleafID)
        {
            IQueryable<TreeClassEntity> treeClass = _treeClass.Where(c =>  c.A4LeafID == cleafID && c.Ordinal == ordinal);
            return treeClass.Join(leaves, a => a.LeafID, b => b.LeafID, (a, b) => b);
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
            IDbContext db;
            if (treeID <= 100)
            {
                db = DBContextFactory.Instance.CreateContext("IRAPContext");
                return db.Set<ETreeSysLeaf>().FirstOrDefault(c => c.PartitioningKey == pk && (short)treeID == c.TreeID && leafID == c.LeafID);
            }
            else
            {
                db = DBContextFactory.Instance.CreateContext("IRAPMDMContext");
                return db.Set<ETreeBizLeaf>().FirstOrDefault(c => c.PartitioningKey == pk && (short)treeID == c.TreeID && leafID == c.LeafID);
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
            else
            {
                db = DBContextFactory.Instance.CreateContext("IRAPMDMContext");
                return db.Set<ETreeBizLeaf>().Where(c => c.PartitioningKey == pk && (short)treeID == c.TreeID && c.Code.Contains(code));
            }
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
        /// 结点状态
        /// </summary>
        public byte NodeStatus { get; set; }
        /// <summary>
        /// 可访问性  0=不可选  1=可单选  2=可复选
        /// </summary>
        public byte Accessibility { get; set; }
        /// <summary>
        /// 选中状态  0=未选中  1=已选中
        /// </summary>
        public byte SelectStatus { get; set; }       // 
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
