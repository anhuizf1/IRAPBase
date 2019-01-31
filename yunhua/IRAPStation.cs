using IRAPBase.DTO;
using IRAPBase.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase
{
    public class IRAPStation
    {
        private UnitOfWork _unitOfWork = null;
        private   Repository<StationEntity> _stations;
        private int _communityID = 0;
        public int CommunityID { get { return _communityID; } set { _communityID = value; } }
        public IRAPStation()
        {
            _unitOfWork = new UnitOfWork(new IRAPSqlDBContext("name=IRAPContext"));
            _stations = _unitOfWork.Repository<StationEntity>();
        }
        //获取所有站点
        public List<StationEntity> GetAllStations()
        {
           List<StationEntity> list =   _stations.Table.ToList<StationEntity>();
            return list;
        }
        //判断某个mac地址是否存在！
        public IRAPError HasStation(string stationID )
        {
            IRAPError error = new IRAPError();
           StationEntity e=   _stations.Table.FirstOrDefault(r => r.StationID == stationID);
            if (e == null)
            {
                error.ErrCode = 91;
                error.ErrText = $"此站点：{stationID} 不存在！";
            }
            else
            {
                error.ErrCode = 0;
                error.ErrText = $"站点存在！站点名：{e.HostName}";
            }
            return error;
        }

        public StationEntity GetStation(string stationID)
        {
            IRAPError error = new IRAPError();
            StationEntity e = _stations.Table.FirstOrDefault(r => r.StationID == stationID);
           
            return e;
        }
    }
}
