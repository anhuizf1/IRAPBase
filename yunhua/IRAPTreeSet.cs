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
        #region 初始化及构造
        private int _treeID = 0;
        private int _communityID;
        private IQueryable<TreeNodeEntity> nodes;
        private IQueryable<TreeLeafEntity> leaves;
        private IDbContext context = null;
        private List<IRAPTreeNodes> accessibleNodes;
        public int TreeID { get { return _treeID; } }
        public long PK { get { return _communityID * 10000L; } }
        public IRAPTreeSet(int communityID, int treeID)
        {
            _treeID = treeID;
            _communityID = communityID;
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
                nodes = new Repository<ETreeSysDir>(context).Table;
                leaves = new Repository<ETreeSysLeaf>(context).Table;
            }
            else
            {
                nodes = new Repository<ETreeBizDir>(context).Table;
                leaves = new Repository<ETreeBizLeaf>(context).Table;
            }
        }

        #endregion
        #region 属性
        //1.(标识)获取叶子集//叶子
        public IQueryable<TreeLeafEntity> LeafEntities { get { return leaves; } }
        //2.(标识/目录)获取节点集合 
        public IQueryable<TreeNodeEntity> NodeEntities { get { return nodes; } }

        //3 检索属性
        //4分类属性
        public IQueryable<TreeClassEntity> ClassEntities { get { return null; } }
        //5.瞬态属性
        public IQueryable<TreeTransient> TransientEntities { get { return null; } }
        //6.状态属性
        public IQueryable<TreeStatus> Status { get { return null; } }

        #endregion
        //7.一般属性
        public IQueryable<T> GenAttr<T>() where T : BaseEntity
        {
            return new Repository<T>(context).Table;
        }
        //8. 行集属性
        public IQueryable<BaseEntity> RowSet(int ordinal)
        {
            //根据数据字典生成实例
            return new Repository<BaseEntity>(context).Table;
        }
        #region 查询
        /// <summary>
        /// 获取树视图（不带权限）
        /// </summary>
        /// <returns></returns>
        public List<TreeViewDTO> TreeView(out int errCode, out string errText, int entryNode = 0, bool includeLeaves = true)
        {

            List<TreeViewDTO> resList = GetPlainTreeData(TreeViewData(out errCode, out errText, entryNode, includeLeaves));
            return resList;
        }


        public void Test()
        {

            Type type = Type.GetType("IRAPBase.Entities.ERS_T1R1", true, true);

            var repositoryType = typeof(Repository<>);

           // IRepository  test;
            
            var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(type), context);
            
            Console.WriteLine(type.FullName);

        }
        /// <summary>
        /// 返回树的数据，支持懒加载 
        /// </summary>
        /// <returns></returns>
        private IRAPTreeNodes TreeViewData(out int errCode, out string errText, int entryNode = 0, bool includingLeaves = true)
        {
            long[] pkArray = { PK + _treeID, _treeID };
            errCode = 0;
            errText = "获取成功！";
            List<TreeNodeEntity> newNodelist = nodes.Where(r => r.TreeID == _treeID && pkArray.Contains(r.PartitioningKey)).OrderBy(r => r.NodeDepth).ThenBy(r => r.NodeID).ToList();
            List<TreeNodeEntity> nodeSet = new List<TreeNodeEntity>();
            if (entryNode > 0)
            {
                TreeNodeEntity thisNode = newNodelist.FirstOrDefault(c => c.NodeID == entryNode);
                if (thisNode == null)
                {
                    errCode = 2;
                    errText = "入口结点无效！";
                    return null;
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
                leafSet = leaves.Where(r => r.TreeID == _treeID && pkArray.Contains(r.PartitioningKey)).OrderBy(r => r.UDFOrdinal).Take(50000).ToList();
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
        //获取树视图（带权限）
        public List<TreeViewDTO> AccessibleTreeView(int agencyNode, int roleNode, out int errCode, out string errText)
        {
            IRAPGrant grant = new IRAPGrant(_communityID);
            accessibleNodes.Clear();
            List<EGrant> list = grant.GetGrantListByTree(_treeID, agencyNode, roleNode);

            IRAPTreeNodes rootTree = TreeViewData(out errCode, out errText);
            if (errCode != 0)
            {
                return new List<TreeViewDTO>();
            }
            DownTree(rootTree, list);
            foreach (var node in accessibleNodes)
            {
                UpTree(node);
            }
            //去掉不可访问的结点
            RemoveNoAccessible(rootTree);
            return GetPlainTreeData(rootTree);
        }

        public List<TreeViewDTO> AccessibleTreeView(string access_token,out int errCode,out string errText)
        {
           
            IRAPLog log = new IRAPLog();
            LoginEntity logE = log.GetLogIDByToken(access_token);
            if (logE == null)
            {
                errCode = 99;
                errText = "无法访问树数据，令牌无效！";
                return null;
            }
            return AccessibleTreeView(logE.AgencyLeaf, logE.RoleLeaf, out errCode,out errText);
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
                AlternateCode= rootNode.AlternateCode,
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
                        AlternateCode=c.AlternateCode,
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

        // 获取子树结点集（不带权限）
        public List<TreeNodeEntity> SubTreeNodes(int nodeID)
        {
            return nodes.Where(r => r.PartitioningKey == PK + _treeID && r.TreeID == _treeID && r.Father == nodeID).ToList();
        }

        //获取 子树叶集（不带权限） 
        public List<TreeLeafEntity> SubTreeLeaves(int nodeID)
        {
            return leaves.Where(r =>r.PartitioningKey==PK+_treeID && r.TreeID==_treeID &&  r.Father == nodeID).ToList();
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
                node.AlternateCode= r.AlternateCode;
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
            NewTreeNodeDTO res = new NewTreeNodeDTO();
            if (nodeType == 3)
            {
                return NewNode(fatherNode, nodeCode, nodeName, englishName,createBy,alterNateCode, udfOrdinal);
            }
            else if (nodeType == 4)
            {
                return NewLeaf(fatherNode, englishName, createBy, nodeCode, alterNateCode, nodeName, udfOrdinal);
            }
            else
            {
                res.ErrCode = 91;
                res.ErrText = "不支持的结点类型！";
                res.NewNodeID = 0;
            }
            return res;
        }
        //新增节点
        private NewTreeNodeDTO NewNode(int fatherNode, string nodeCode, string nodeName,
            string englishName, string createBy,string alterNateCode,float udfOrdinal = 0F)
        {
            NewTreeNodeDTO res = new NewTreeNodeDTO();
            long[] pkDict = new long[2] { PK + _treeID, _treeID };
            res.PartitioningKey = PK + _treeID;
            try
            {
                TreeNodeEntity father = null;
                if (fatherNode == 0)
                {
                    father = nodes.FirstOrDefault(r => r.PartitioningKey == PK + _treeID);
                    if (father == null)
                    {
                        father = new TreeNodeEntity();
                        father.NodeID = 0;
                        father.CSTRoot = 0;
                        father.NodeDepth = 1;
                    }
                    else
                    {
                        res.ErrCode = 93;
                        res.ErrText = "已存在节点不允许在创建根节点！NodeID不能为0";
                        return res;
                    }
                }
                else
                {
                    father = nodes.FirstOrDefault(r => r.NodeID == fatherNode && pkDict.Contains(r.PartitioningKey));
                }


                if (father == null)
                {
                    res.ErrCode = 22;
                    res.ErrText = $"结点标识：{fatherNode}不存在！";
                    return res;
                }

              
                if (_treeID <= 100)
                {
                    ETreeSysDir e = new ETreeSysDir();
                    e.PartitioningKey = PK + _treeID;
                    e.NameID = 0;
                    e.IconID = 0;
                    e.NodeDepth = (byte)(father.NodeDepth + 1);
                    e.NodeID = 0; //申请
                    e.NodeName = nodeName;
                    e.NodeStatus = 0;
                    e.TreeID = (short)_treeID;
                    e.UDFOrdinal = udfOrdinal;
                    e.Code = nodeCode;
                    e.CSTRoot = father.CSTRoot;
                    e.Father = fatherNode;
                    e.CreatedOn = DateTime.Now;
                    e.CreatedBy = createBy;
                    e.DescInEnglish = englishName;
                    e.ModifiedBy = "";
                    e.ModifiedOn = DateTime.Now;
                    Repository<ETreeSysDir> nodes = new Repository<ETreeSysDir>(context);
                    SequenceValueDTO dto = IRAPSequence.GetSequence("NextSysNodeID", 1);
                    if (dto.ErrCode != 0)
                    {
                        res.ErrCode = dto.ErrCode;
                        res.ErrText = dto.ErrText;
                        return res;
                    }
                    e.NodeID = (int)dto.SequenceValue;
                    e.CSTRoot = father.NodeID == 0 ? e.NodeID : father.CSTRoot;
                    nodes.Insert(e);
                    res.NewNodeID = e.NodeID;

                    nodes.SaveChanges();
                }
                else
                {
                    ETreeBizDir e = new ETreeBizDir();
                    e.PartitioningKey = PK + _treeID;
                    e.NameID = 0;
                    e.IconID = 0;
                    e.NodeDepth = (byte)(father.NodeDepth + 1);
                    e.NodeID = 0; //申请
                    e.NodeName = nodeName;
                    e.NodeStatus = 0;
                    e.TreeID = (short)_treeID;
                    e.UDFOrdinal = udfOrdinal;
                    e.Code = nodeCode;
                    e.CSTRoot = father.CSTRoot;
                    e.Father = fatherNode;
                    e.CreatedOn = DateTime.Now;
                    e.CreatedBy = createBy;
                    e.DescInEnglish = englishName;
                    e.ModifiedBy = "";
                    e.ModifiedOn = DateTime.Now;
                    Repository<ETreeBizDir> nodes = new Repository<ETreeBizDir>(context);
                    SequenceValueDTO dto = IRAPSequence.GetSequence("NextUserNodeID", 1);
                    if (dto.ErrCode != 0)
                    {
                        res.ErrCode = dto.ErrCode;
                        res.ErrText = dto.ErrText;
                        return res;
                    }
                    e.NodeID = (int)dto.SequenceValue;
                    res.NewNodeID = e.NodeID;
                    res.NewEntityID = 0;
                    res.PartitioningKey = PK + _treeID;
                    nodes.Insert(e);
                    nodes.SaveChanges();
                }

                res.ErrCode = 0;
                res.ErrText = "结点创建成功！";

                return res;
            }
            catch (Exception err)
            {
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
            long[] pkDict = new long[2] { PK + _treeID, _treeID };
            try
            {
                TreeNodeEntity father = nodes.FirstOrDefault(r => r.NodeID == fatherNode && pkDict.Contains(r.PartitioningKey));

                if (father == null)
                {
                    res.ErrCode = 22;
                    res.ErrText = $"父结点标识：{fatherNode}不存在！";
                    return res;
                }

                IRAPSequence sequenceServer = new IRAPSequence();
                if (_treeID <= 100)
                {
                    ETreeSysLeaf e = new ETreeSysLeaf();
                    e.PartitioningKey = PK + _treeID;
                    e.NameID = 0;
                    e.IconID = 0;
                    e.AlternateCode = alterNateCode;
                    e.NodeDepth = (byte)(father.NodeDepth + 1);
                    e.LeafID = 0; //申请
                    e.NodeName = nodeName;
                    e.LeafStatus = 0;
                    e.TreeID = (short)_treeID;
                    e.UDFOrdinal = udfOrdinal;
                    e.Code = nodeCode;
                    e.CSTRoot = father.CSTRoot;
                    e.Father = fatherNode;
                    e.DescInEnglish = englishName;
                    e.CreatedBy = createBy;
                    e.CreatedOn = DateTime.Now;
                    e.ModifiedBy = "";
                    e.ModifiedOn = DateTime.Now;
                    Repository<ETreeSysLeaf> nodes = new Repository<ETreeSysLeaf>(context);
                    SequenceValueDTO dto = IRAPSequence.GetSequence("NextSysLeafID", 1);
                    if (dto.ErrCode != 0)
                    {
                        res.ErrCode = dto.ErrCode;
                        res.ErrText = dto.ErrText;
                        return res;
                    }
                    SequenceValueDTO dto2 = IRAPSequence.GetSequence("NextSysEntityID", 1);
                    e.LeafID = (int)dto.SequenceValue;
                    e.EntityID = (int)dto2.SequenceValue;
                    nodes.Insert(e);
                    res.NewNodeID = -e.LeafID;
                    nodes.SaveChanges();
                }
                else
                {
                    ETreeBizLeaf e = new ETreeBizLeaf();
                    e.PartitioningKey = PK + _treeID;
                    e.NameID = 0;
                    e.IconID = 0;
                    e.AlternateCode = alterNateCode;
                    e.NodeDepth = (byte)(father.NodeDepth + 1);
                    e.LeafID = 0; //申请
                    e.NodeName = nodeName;
                    e.LeafStatus = 0;
                    e.TreeID = (short)_treeID;
                    e.UDFOrdinal = udfOrdinal;
                    e.Code = nodeCode;
                    e.CSTRoot = father.CSTRoot;
                    e.Father = fatherNode;
                    e.DescInEnglish = englishName;
                    e.CreatedOn = DateTime.Now;
                    e.CreatedBy = createBy;
                    e.ModifiedBy = "";
                    e.ModifiedOn = DateTime.Now;

                    Repository<ETreeBizLeaf> nodes = new Repository<ETreeBizLeaf>(context);
                    SequenceValueDTO dto = IRAPSequence.GetSequence("NextUserLeafID", 1);
                    if (dto.ErrCode != 0)
                    {
                        res.ErrCode = dto.ErrCode;
                        res.ErrText = dto.ErrText;
                        return res;
                    }
                    e.LeafID = (int)dto.SequenceValue;
                    SequenceValueDTO dto2 = IRAPSequence.GetSequence("NextUserEntityID", 1);
                    e.EntityID = (int)dto2.SequenceValue;
                    res.NewNodeID = e.LeafID;
                    res.NewEntityID = e.EntityID;
                    res.PartitioningKey = PK + _treeID;
                    nodes.Insert(e);
                    nodes.SaveChanges();
                }

                res.ErrCode = 0;
                res.ErrText = "叶子创建成功！";

                return res;
            }
            catch (Exception err)
            {
                res.ErrCode = 9999;
                res.ErrText = $"创建叶子时发生错误:{err.Message}";
                return res;
            }
        }

        //新增分类属性
        //新增瞬态属性
        /// <summary>
        /// 删除树结点 
        /// </summary>
        /// <param name="nodeType">结点类型：3-结点 4-叶子</param>
        /// <param name="nodeID">结点标识</param>
        /// <returns></returns>
        public IRAPError DeleteNode(int nodeType, int nodeID)
        {
            IRAPError error = new IRAPError();
            long[] pkDict = new long[2] { PK + _treeID, _treeID };
            try
            {
                if (nodeType == 3)
                {
                    if (_treeID <= 100)
                    {
                        Repository<ETreeSysDir> nodes = new Repository<ETreeSysDir>(context);
                        ETreeSysDir thisNode = nodes.Table.FirstOrDefault(r => r.NodeID == nodeID && pkDict.Contains( r.PartitioningKey ) );
                        if (thisNode == null)
                        {
                            error.ErrCode = 22;
                            error.ErrText = $"结点标识：{nodeID}不存在！";
                            return error;
                        }
                        if (thisNode.PartitioningKey == _treeID)
                        {
                            error.ErrCode = 22;
                            error.ErrText = $"结点标识：{nodeID}属于系统结点请从后台删除！";
                            return error;
                        }
                        var tempNode = nodes.Table.FirstOrDefault(c => c.Father == nodeID && pkDict.Contains(c.PartitioningKey));
                        if (tempNode != null)
                        {
                            error.ErrCode = 23;
                            error.ErrText = $"结点“{tempNode.NodeName}”下面已有结点，请先删除子结点！";
                            return error;
                        }
                        nodes.Delete(thisNode);
                        nodes.SaveChanges();
                    }
                    else
                    {
                        Repository<ETreeBizDir> nodes = new Repository<ETreeBizDir>(context);
                        ETreeBizDir thisNode = nodes.Table.FirstOrDefault(r => r.NodeID == nodeID && pkDict.Contains( r.PartitioningKey )  );
                        if (thisNode == null)
                        {
                            error.ErrCode = 22;
                            error.ErrText = $"结点标识：{nodeID}不存在！";
                            return error;
                        }
                        if (thisNode.PartitioningKey == _treeID)
                        {
                            error.ErrCode = 22;
                            error.ErrText = $"结点标识：{nodeID}属于系统结点请从后台删除！";
                            return error;
                        }
                        var tempNode = nodes.Table.FirstOrDefault(c => c.Father == nodeID && pkDict.Contains(c.PartitioningKey));
                        if (tempNode != null)
                        {
                            error.ErrCode = 23;
                            error.ErrText = $"结点“{tempNode.NodeName}”下面已有结点，请先删除子结点！";
                            return error;
                        }
                        nodes.Delete(thisNode);
                        nodes.SaveChanges();
                    }
                }
                else if (nodeType == 4)
                {
                    if (_treeID <= 100)
                    {
                        Repository<ETreeSysLeaf> nodes = new Repository<ETreeSysLeaf>(context);
                        ETreeSysLeaf thisNode = nodes.Table.FirstOrDefault(r => r.LeafID == nodeID && r.PartitioningKey == PK + _treeID);
                        if (thisNode == null)
                        {
                            error.ErrCode = 22;
                            error.ErrText = $"结点标识：{nodeID}不存在！";
                            return error;
                        }
                        nodes.Delete(thisNode);
                        nodes.SaveChanges();
                    }
                    else
                    {
                        Repository<ETreeBizLeaf> nodes = new Repository<ETreeBizLeaf>(context);
                        ETreeBizLeaf thisNode = nodes.Table.FirstOrDefault(r => r.LeafID == nodeID && r.PartitioningKey == PK + _treeID);
                        if (thisNode == null)
                        {
                            error.ErrCode = 22;
                            error.ErrText = $"结点标识：{nodeID}不存在！";
                            return error;
                        }
                        nodes.Delete(thisNode);
                        nodes.SaveChanges();
                    }
                }
                else
                {
                    error.ErrCode = 91;
                    error.ErrText = "不支持的NodeType类型！";
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

        /// <summary>
        /// 修改树结点
        /// </summary>
        /// <param name="nodeType">结点类型：3-结点 4-叶子</param>
        /// <param name="nodeID">结点标识</param>
        /// <param name="nodeCode">结点代码</param>
        /// <param name="nodeName">结点名称</param>
        /// <param name="alternateCode">替代代码</param>
        /// <param name="udfOrdinal">结点序号</param>
        /// <returns></returns>
        public IRAPError ModifyNode(int nodeType, int nodeID, string modifiedBy, string englishName, string nodeCode, string nodeName, string alternateCode, float udfOrdinal = 0F)
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

                if (nodeType == 3)
                {
                    if (_treeID <= 100)
                    {
                        Repository<ETreeSysDir> nodes = new Repository<ETreeSysDir>(context);
                        ETreeSysDir thisNode = nodes.Table.FirstOrDefault(r => r.NodeID == nodeID && r.PartitioningKey == PK + _treeID);
                        if (thisNode == null)
                        {
                            error.ErrCode = 22;
                            error.ErrText = $"结点标识：{nodeID}不存在！";
                            return error;
                        }
                        
                        thisNode.Code = nodeCode;
                        thisNode.NodeName = nodeName;
                        thisNode.UDFOrdinal = udfOrdinal;
                        nodes.Update(thisNode);
                        nodes.SaveChanges();
                    }
                    else
                    {
                        Repository<ETreeBizDir> nodes = new Repository<ETreeBizDir>(context);
                        ETreeBizDir thisNode = nodes.Table.FirstOrDefault(r => r.NodeID == nodeID && r.PartitioningKey == PK + _treeID);
                        if (thisNode == null)
                        {
                            error.ErrCode = 22;
                            error.ErrText = $"结点标识：{nodeID}不存在！";
                            return error;
                        }
                        thisNode.Code = nodeCode;
                        thisNode.NodeName = nodeName;
                        thisNode.UDFOrdinal = udfOrdinal;
                         
                        nodes.Update(thisNode);
                        nodes.SaveChanges();
                    }
                }
                else if (nodeType == 4)
                {
                    if (_treeID <= 100)
                    {
                        Repository<ETreeSysLeaf> nodes = new Repository<ETreeSysLeaf>(context);
                        ETreeSysLeaf thisNode = nodes.Table.FirstOrDefault(r => r.LeafID == nodeID && r.PartitioningKey == PK + _treeID);
                        if (thisNode == null)
                        {
                            error.ErrCode = 22;
                            error.ErrText = $"结点标识：{nodeID}不存在！";
                            return error;
                        }
                        thisNode.Code = nodeCode;
                        thisNode.AlternateCode = alternateCode;
                        thisNode.NodeName = nodeName;
                        thisNode.UDFOrdinal = udfOrdinal;
                        thisNode.DescInEnglish = englishName;
                        thisNode.ModifiedBy = modifiedBy;
                        thisNode.ModifiedOn = DateTime.Now;
                        thisNode.AlternateCode = alternateCode;
                        nodes.Update(thisNode);
                        nodes.SaveChanges();

                    }
                    else
                    {
                        Repository<ETreeBizLeaf> nodes = new Repository<ETreeBizLeaf>(context);
                        ETreeBizLeaf thisNode = nodes.Table.FirstOrDefault(r => r.LeafID == nodeID && r.PartitioningKey == PK + _treeID);
                        if (thisNode == null)
                        {
                            error.ErrCode = 22;
                            error.ErrText = $"结点标识：{nodeID}不存在！";
                            return error;
                        }
                        thisNode.Code = nodeCode;
                        thisNode.AlternateCode = alternateCode;
                        thisNode.NodeName = nodeName;
                        thisNode.UDFOrdinal = udfOrdinal;
                        thisNode.DescInEnglish = englishName;
                        thisNode.ModifiedBy = modifiedBy;
                        thisNode.ModifiedOn = DateTime.Now;
                        thisNode.AlternateCode = alternateCode;
                        nodes.Update(thisNode);
                        nodes.SaveChanges();
                    }
                }
                else
                {
                    error.ErrCode = 91;
                    error.ErrText = "不支持的NodeType类型！";
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

    }


    //树基本结构类型
    public class IRAPTreeNodes
    {

        public int NodeID { get; set; }          //  --结点标识
        public byte TreeViewType { get; set; }    // --树视图类型
        public byte NodeType { get; set; }        //  --结点类型
        public string NodeCode { get; set; }      //   --结点代码
        public string AlternateCode { get; set; }
        public string NodeName { get; set; }       //  --结点名称
        public int Parent { get; set; }            //  --父结点标识
        public byte NodeDepth { get; set; }         // --结点深度
        public int CSTRoot { get; set; }            //--权限控制点
        public float UDFOrdinal { get; set; }       // --自定义遍历序
        public byte NodeStatus { get; set; }         // --结点状态
        public byte Accessibility { get; set; }      // --可访问性  0=不可选  1=可单选  2=可复选
        public byte SelectStatus { get; set; }       // --选中状态  0=未选中  1=已选中
        public string SearchCode1 { get; set; }      //--第一检索码
        public string SearchCode2 { get; set; }      //--第二检索码
        public string HelpMemoryCode { get; set; }   //--助记码
        public string IconFile { get; set; }        // --图标文件
        public byte[] IconImage { get; set; }       // --图标图像

        public IRAPTreeNodes FatherNode { get; set; }
        public List<IRAPTreeNodes> Children { get; set; }
    }
}
