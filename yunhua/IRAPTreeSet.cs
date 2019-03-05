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
    //树基本结构类型
    public class IRAPTreeNodes
    {

        public int NodeID { get; set; }          //  --结点标识
        public byte TreeViewType { get; set; }    // --树视图类型
        public byte NodeType { get; set; }        //  --结点类型
        public string NodeCode { get; set; }      //   --结点代码
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

    /// <summary>
    /// 访问IRAP树的通用类
    /// </summary>
    public class IRAPTreeSet
    {
        #region 初始化及构造
        private int _treeID = 0;
        private int _communityID;
        //private string dbConnectStr;
         
        private  IQueryable<TreeNodeEntity> nodes;
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
        public IQueryable<TreeNodeEntity>  NodeEntities { get { return nodes; } }

        //3 检索属性
        //4分类属性
        public IQueryable<TreeClassEntity> ClassEntities { get { return null; } }
        //5.瞬态属性
        public IQueryable<TreeTransient> TransientEntities { get { return null; } }
        //6.状态属性
        public IQueryable<TreeStatus> Status { get { return null; } }

        #endregion
        //7.一般属性
        public IQueryable<T> GenAttr<T> () where T : BaseEntity
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
        public IRAPTreeNodes TreeView()
        {
            
         
            long[] pkArray = { PK + _treeID, _treeID };
            var list = nodes.Where(r => r.TreeID == _treeID && pkArray.Contains(r.PartitioningKey)).OrderBy(r => r.NodeDepth).ThenBy(r => r.NodeID).ToList();
            var list2 = leaves.Where(r => r.TreeID == _treeID && pkArray.Contains(r.PartitioningKey)).OrderBy(r => r.UDFOrdinal).Take(50000).ToList();
            TreeNodeEntity node = list.FirstOrDefault();
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
                FindNodeAddToNode(rootNode, list, list2);
            }
            return rootNode;
        }
        //获取树视图（带权限）
        public IRAPTreeNodes AccessibleTreeView(int agencyNode, int roleNode)
        {
            IRAPGrant grant = new IRAPGrant(_communityID);
            accessibleNodes.Clear();
            List<EGrant> list = grant.GetGrantListByTree(_treeID, agencyNode, roleNode);
            IRAPTreeNodes rootTree = TreeView();
            DownTree(rootTree, list);
            foreach (var node in accessibleNodes)
            {
                UpTree(node);
            }
            //去掉不可访问的结点
            RemoveNoAccessible(rootTree);
            return rootTree;
        }

        //获取可访问子树结点集
        public List<TreeNodeEntity> AccessibleSubTreeNodes(int nodeID, long sysLogID)
        {
            return null;
        }

        //获取可访问子树叶集 
        public List<TreeLeafEntity> AccessibleSubTreeLeaves(int nodeID, long sysLogID)
        {
            return null;
        }

        // 获取子树结点集（不带权限）
        public List<TreeNodeEntity> SubTreeNodes(int nodeID)
        {
            return nodes.Where(r=>r.Father==nodeID).ToList();
        }

        //获取 子树叶集（不带权限） 
        public List<TreeLeafEntity> SubTreeLeaves(int nodeID)
        {
            return  leaves.Where(r=>r.Father==nodeID).ToList();
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
                root.Children.Add(node);

                FindNodeAddToNode(node, nodeSet, leafSet);
            }
        }
        //向树中增加叶子
        private void FindLeafAddToNode(IRAPTreeNodes root, List<TreeLeafEntity> leafSet)
        {
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
                node.NodeType = 3;
                node.CSTRoot = r.CSTRoot;
                node.UDFOrdinal = r.UDFOrdinal;
                node.TreeViewType = 2;
                node.Parent = r.Father;
                node.IconFile = r.IconID.ToString();
                node.FatherNode = root;
                node.Accessibility = 0;
                root.Children.Add(node);
            }
        }

        #endregion

 
        //新增节点
        public NewTreeNodeDTO NewNode( int fatherNode ,   string nodeCode,string nodeName, float udfOrdinal    )
        {
            NewTreeNodeDTO res = new NewTreeNodeDTO();
            try
            {
                TreeNodeEntity father = nodes.FirstOrDefault(r => r.NodeID == fatherNode);
               
                if (father == null)
                {
                    res.ErrCode = 22;
                    res.ErrText = $"结点标识：{fatherNode}不存在！";
                    return res;
                }
               
                IRAPSequence sequenceServer = new IRAPSequence();
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
                    Repository<ETreeSysDir> nodes = new Repository<ETreeSysDir>(context);
                    SequenceValueDTO dto=   sequenceServer.GetSequence("NextSysNodeID", 1);
                    if (dto.ErrCode != 0)
                    {
                        res.ErrCode = dto.ErrCode;
                        res.ErrText = dto.ErrText;
                        return res;
                    }
                    e.NodeID =(int) dto.SequenceValue;
                    nodes.Insert( e);
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
                    Repository<ETreeBizDir> nodes = new Repository<ETreeBizDir>(context);
                    SequenceValueDTO dto = sequenceServer.GetSequence("NextUserNodeID", 1);
                    if (dto.ErrCode != 0)
                    {
                        res.ErrCode = dto.ErrCode;
                        res.ErrText = dto.ErrText;
                        return res;
                    }
                    e.NodeID = (int)dto.SequenceValue;
                    res.NewNodeID = e.NodeID;
                    nodes.Insert( e);
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
        public NewTreeNodeDTO NewLeaf(int fatherNode, string nodeCode,  string alterNateCode, string nodeName, float udfOrdinal=1F)
        {

            NewTreeNodeDTO res = new NewTreeNodeDTO();
            try
            {
                TreeNodeEntity father = nodes.FirstOrDefault(r => r.NodeID == fatherNode);

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
                    e.CreatedTime = DateTime.Now;
                    Repository<ETreeSysLeaf> nodes = new Repository<ETreeSysLeaf>(context);
                    SequenceValueDTO dto = sequenceServer.GetSequence("NextSysLeafID", 1);
                    if (dto.ErrCode != 0)
                    {
                        res.ErrCode = dto.ErrCode;
                        res.ErrText = dto.ErrText;
                        return res;
                    }
                    e.LeafID = (int)dto.SequenceValue;
                    nodes.Insert(e);
                    res.NewNodeID = e.LeafID;
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
                    e.CreatedTime = DateTime.Now;
                    Repository<ETreeBizLeaf> nodes = new Repository<ETreeBizLeaf>(context);
                    SequenceValueDTO dto = sequenceServer.GetSequence("NextUserLeafID", 1);
                    if (dto.ErrCode != 0)
                    {
                        res.ErrCode = dto.ErrCode;
                        res.ErrText = dto.ErrText;
                        return res;
                    }
                    e.LeafID = (int)dto.SequenceValue;
                    res.NewNodeID = e.LeafID;
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
        //删除结点 
        public IRAPError DeleteNode( int nodeID)
        {
            return null;
        }
        //删除叶子
        public IRAPError DeleteLeaf(int  leafID)
        {
            return null;
        }
        //修改结点
        public IRAPError ModifyNode()
        {
            return null;
        }
        //修改叶子
        public IRAPError ModifyLeaf()
        {
            return null;
        }
       
         
    }
}
