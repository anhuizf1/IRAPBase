using IRAPBase.DTO;
using IRAPBase.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace IRAPBase
{
    /// <summary>
    /// 对系统进行管理类
    /// </summary>
    public class IRAPSystem
    {

        private IDbContext _db = null;
        private Repository<SystemEntity> _systems;
        private IDbSet<SystemExEntity> _systemEx1;

        private Repository<SystemEntity> Repository { get { return _systems; } }

        /// <summary>
        /// 构造函数
        /// </summary>
        public IRAPSystem()
        {
            _db = new IRAPSqlDBContext("IRAPContext");
            _systems = new Repository<SystemEntity>(_db);
            _systemEx1 = _db.Set<SystemExEntity>();
        }
        /// <summary>
        /// 获取所有系统清单(不带权限)
        /// </summary>
        /// <returns></returns>
        public List<SystemDTO> GetSystemList(short progLangID)
        {
            IQueryable<ETreeSysDir> dirs = new Repository<ETreeSysDir>(_db).Table.Where(r => r.TreeID == 3);
            IQueryable<SystemExEntity> sysEx = new Repository<SystemExEntity>(_db).Table.Where(c => c.ProgLanguageID == progLangID);

            var query = _systems.Table.Join(dirs, sy => sy.SystemID, np => np.NodeID, (sy, np) => new { sy, np }).
                Join(sysEx, i => i.sy.SystemID, d => d.SystemID, (s, p) => new SystemDTO
                {
                    SystemID = s.sy.SystemID,
                    Accessible = "1",
                    AddToolBar = false,
                    Application = s.sy.DefaultAppServer,
                    Author = s.sy.Author,
                    BackgroundPic = s.sy.BackgroundPic,
                    BGPicPath = s.sy.BGPicPath,
                    Coauthor = s.sy.Coauthor,
                    DataSrcLinkID = (short)s.sy.DefaultDataSrcLinkID,
                    GAYearMonth = s.sy.GAYearMonth,
                    IconFile = s.sy.LogoPicPath,
                    IconImage = null,
                    KeyStrokeStream = "",
                    LogoPic = s.sy.LogoPic,
                    LogoPicPath = s.sy.LogoPicPath,
                    MenuShowCtrl = p.MenuShowCtrl,
                    MenuStyle = p.MenuStyle1,
                    ProductNo = s.sy.ProductNo,
                    ScreenResolution = 1,
                    SystemName = s.np.NodeName,
                    VersionNo = s.sy.VersionNo
                }).ToList();

            return query.ToList();
        }

        /// <summary>
        /// 获取可用系统（带权限）
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="progLangID"></param>
        /// <returns></returns>
        public List<SystemDTO> GetAvailableSystems(string access_token, short progLangID)
        {
            IRAPLog log = new IRAPLog();
            LoginEntity logEntity = log.GetLogIDByToken(access_token);
            int communityID = (int)(logEntity.PartitioningKey / 10000L);
            var list = GetSystemList(progLangID);
            IRAPTreeSet treeSet = new IRAPTreeSet(communityID, 3);
            List<SystemDTO> resList = new List<SystemDTO>();
            foreach (var r in list)
            {
                List<TreeViewDTO> accessList = treeSet.AccessibleTreeView(r.SystemID, logEntity.AgencyLeaf, logEntity.RoleLeaf);
                if (accessList.Count < 2)
                {
                    continue;
                }
                resList.Add(r);
            }
            return resList;
        }
        /// <summary>
        /// 新增一个系统
        /// </summary>
        /// <param name="SystemID">系统标识</param>
        /// <param name="ProductNo">产品名(英文)</param>
        /// <param name="VersionNo">版本号：例如V1.02 </param>
        /// <param name="ProgLangID">编程语言标识 见视图 svw_ProgLanguages或调用GetProgLanguages方法</param>
        /// <param name="MenuShowCtrl">菜单样式：1-横向菜单2-树菜单3-竖向手风琴菜单5-带tab页的风格7-竖向黑色主题</param>
        /// <param name="MenuStyle"></param>
        /// <param name="Author">作者</param>
        /// <param name="Coauthor">合作者</param>
        /// <param name="LogoPicPath">网页顶部banner路径</param>
        /// <param name="LogoPic">网页顶部banner二进制流</param>
        /// <param name="BGPicPath">登录背景图片路径</param>
        /// <param name="BackgroundPic">登录背景图片二进制流</param>
        /// <param name="DefaultAppServer">默认服务器</param>
        /// <returns></returns>
        public IRAPError AddASystem(int SystemID, string ProductNo, string VersionNo, int ProgLangID, int MenuShowCtrl, int MenuStyle,
            string Author = "softland", string Coauthor = "", string LogoPicPath = "", byte[] LogoPic = null, string BGPicPath = "", byte[] BackgroundPic = null,
            string DefaultAppServer = "")
        {
            var systemEntity = _db.Set<ETreeSysDir>().FirstOrDefault(c => c.NodeID == SystemID && c.TreeID == 3);
            if (systemEntity == null)
            {
                throw new Exception($"系统标识：{SystemID} 不存在！");
            }
            var a = new SystemEntity()
            {
                Author = Author,
                BackgroundPic = BackgroundPic,
                BGPicPath = BGPicPath,
                Coauthor = Coauthor,
                DefaultAppServer = DefaultAppServer,
                DefaultDataSrcLinkID = 0,
                GAYearMonth = System.DateTime.Now.ToString("yyyy-MM"),
                LogoPic = LogoPic,
                LogoPicPath = LogoPicPath,
                ProductNo = ProductNo,
                SystemID = SystemID,
                TelephoneNo = "",
                VersionNo = VersionNo,
                VoiceChannelMax = 0,
                VoiceChannelMin = 0,
                VoiceFilePath = ""
            };
            _systems.Insert(a);
            var b = new SystemExEntity()
            {
                SystemID = SystemID,
                MenuShowCtrl = (byte)MenuShowCtrl,
                MenuStyle1 = (byte)MenuStyle,
                ProgLanguageID = (short)ProgLangID,

            };
            _systemEx1.Add(b);
            _db.SaveChanges();
            return new IRAPError(0, "新增成功！");
        }

        /// <summary>
        /// 获取一个系统
        /// </summary>
        /// <param name="systemID">系统标识</param>
        /// <returns></returns>
        private SystemEntity GetASystem(int systemID)
        {
            var systemEntity = _systems.Entities.FirstOrDefault(c => c.SystemID == systemID);
            if (systemEntity == null)
            {
                throw new Exception($"系统标识：{systemID} 不存在！");
            }
            return systemEntity;
        }
        /// <summary>
        /// 修改系统
        /// </summary>
        /// <param name="system">系统的实体</param>
        /// <returns></returns>
        public IRAPError SaveSystem(SystemEntity system)
        {
            _systems.Update(system);
            _db.SaveChanges();
            return new IRAPError(0, "修改成功！");
        }
        //菜单管理

        /// <summary>
        /// 获取编程语言清单
        /// </summary>
        /// <returns></returns>
        public List<ProgLanguageDTO> GetProgLanguages()
        {
            var list = new List<ProgLanguageDTO>();
            list.Add(new ProgLanguageDTO() { ProgLangID = 1, ProgLanuage = "VC++" });
            list.Add(new ProgLanguageDTO() { ProgLangID = 2, ProgLanuage = "C++ Builder" });
            list.Add(new ProgLanguageDTO() { ProgLangID = 3, ProgLanuage = "Delphi" });
            list.Add(new ProgLanguageDTO() { ProgLangID = 4, ProgLanuage = "PowerBuilder" });
            list.Add(new ProgLanguageDTO() { ProgLangID = 5, ProgLanuage = "Visual Basic" });
            list.Add(new ProgLanguageDTO() { ProgLangID = 6, ProgLanuage = "Visual Foxpro" });
            list.Add(new ProgLanguageDTO() { ProgLangID = 7, ProgLanuage = "ASP" });
            list.Add(new ProgLanguageDTO() { ProgLangID = 8, ProgLanuage = "Java" });
            list.Add(new ProgLanguageDTO() { ProgLangID = 9, ProgLanuage = "C#" });
            list.Add(new ProgLanguageDTO() { ProgLangID = 10, ProgLanuage = "ASP.Net" });
            list.Add(new ProgLanguageDTO() { ProgLangID = 11, ProgLanuage = "Python" });
            list.Add(new ProgLanguageDTO() { ProgLangID = 12, ProgLanuage = "Golang" });
            return list;
        }

        public List<LangDTO> GetLanguages()
        {
            var list = new List<LangDTO>();
            list.Add(new LangDTO { LanguageID = 30, LanuageName = "简体中文" });
            list.Add(new LangDTO { LanguageID = 0, LanuageName = "English" });
            list.Add(new LangDTO { LanguageID = 28, LanuageName = "繁體中文" });
            return list;
        }

        public List<ZoneDTO> GetZones()
        {
            var list = new List<ZoneDTO>();
            list.Add(new ZoneDTO { Zone = 8, ZoneName = "(UTC+08:00) 北京，重庆，香港特别行政区，乌鲁木齐" });
            list.Add(new ZoneDTO { Zone = 9, ZoneName = "(UTC+09:00) 大阪、札幌、東京" });
            list.Add(new ZoneDTO { Zone = -5, ZoneName = "(UTC-05:00) 东部时间(美国和加拿大)" });
            list.Add(new ZoneDTO { Zone = -6, ZoneName = "(UTC-06:00) 中部时间(美国和加拿大)" });
            list.Add(new ZoneDTO { Zone = -7, ZoneName = "(UTC-07:00) 山地时间(美国和加拿大)" });
            list.Add(new ZoneDTO { Zone = -8, ZoneName = "(UTC-08:00) 太平洋时间(美国和加拿大)" });
            list.Add(new ZoneDTO { Zone = -10, ZoneName = "(UTC-10:00) 夏威夷" });
            list.Add(new ZoneDTO { Zone = -12, ZoneName = "(UTC-12:00) 国际日期变更线西" });
            list.Add(new ZoneDTO { Zone = 0, ZoneName = "(UTC+00:00) 都柏林，爱丁堡，里斯本，伦敦" });
            list.Add(new ZoneDTO { Zone = 0, ZoneName = "(UTC) 协调世界时" });
            return list;
        }
    }
}
