using IRAPBase.DTO;
using IRAPBase.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase
{
    public class IRAPSystem
    {

        private IDbContext _db = null;
        private Repository<SystemEntity> _systemRepo;

        public Repository<SystemEntity> Repository { get { return _systemRepo; } }


        public IRAPSystem()
        {
            _db = new IRAPSqlDBContext("IRAPContext");
            _systemRepo = new Repository<SystemEntity>(_db);
        }

       
        /// <summary>
        /// 获取所有系统清单
        /// </summary>
        /// <returns></returns>
        public List<SystemDTO> GetSystemList()
        {
            IQueryable<ETreeSysDir> dirs = new Repository<ETreeSysDir>(_db).Table.Where(r=>r.TreeID==3);
            IQueryable<SystemExEntity> sysEx=   new Repository<SystemExEntity>(_db).Table;

            var query = _systemRepo.Table.Join(dirs, sy => sy.SystemID, np => np.NodeID, (sy, np) => new { sy, np }).
                Join(sysEx, i => i.sy.SystemID, d => d.SystemID, (s, p) => new SystemDTO {
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

            //方法1
            /*
            var query = _systemRepo.Table.Join(dirs,  sy => sy.SystemID, np => np.NodeID, (sy, np) => new SystemDTO
            {
                SystemID = sy.SystemID,
                Accessible = "1",
                AddToolBar = false,
                Application = sy.DefaultAppServer,
                Author = sy.Author,
                BackgroundPic = sy.BackgroundPic,
                BGPicPath = sy.BGPicPath,
                Coauthor = sy.Coauthor,
                DataSrcLinkID = (short)sy.DefaultDataSrcLinkID,
                GAYearMonth = sy.GAYearMonth,
                IconFile = sy.LogoPicPath,
                IconImage = null,
                KeyStrokeStream = "",
                LogoPic = sy.LogoPic,
                LogoPicPath = sy.LogoPicPath,
                MenuShowCtrl = 1,
                MenuStyle = 1,
                ProductNo = sy.ProductNo,
                ScreenResolution = 1,
                SystemName = np.NodeName,
                VersionNo = sy.VersionNo
            });
            return query.ToList();*/
            /* 方法2
            IQueryable<SystemDTO> result = from c in _systemRepo.Table //.Select(r=>new { r.Ordinal,r.SystemID })
                                           join t in dirs.Where(r=>r.TreeID==3)
                                                  on    c.SystemID  
                                                  equals   t.NodeID
                                                  select new SystemDTO
                                                  {
                                                      SystemID = c.SystemID,
                                                      Accessible = "1",
                                                      AddToolBar = false,
                                                      Application = c.DefaultAppServer,
                                                      Author = c.Author,
                                                      BackgroundPic = c.BackgroundPic,
                                                      BGPicPath = c.BGPicPath,
                                                      Coauthor = c.Coauthor,
                                                      DataSrcLinkID = (short)c.DefaultDataSrcLinkID,
                                                      GAYearMonth = c.GAYearMonth,
                                                      IconFile = c.LogoPicPath,
                                                      IconImage =null,
                                                      KeyStrokeStream = "",
                                                      LogoPic = c.LogoPic,
                                                      LogoPicPath = c.LogoPicPath,
                                                      MenuShowCtrl = 1,
                                                      MenuStyle = 1,
                                                      ProductNo = c.ProductNo,
                                                      ScreenResolution = 1,
                                                      SystemName = t.NodeName,
                                                      VersionNo = c.VersionNo
                                                  }; 
           return result.ToList(); */
             
        }

        

    }
}
