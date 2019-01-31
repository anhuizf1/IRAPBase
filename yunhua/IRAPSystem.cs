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
        private UnitOfWork _unitOfWork  ;
 
        private Repository<SystemEntity> _systemRepo;

        public Repository<SystemEntity> Repository { get { return _systemRepo;  } }

        public void SaveChange()
        {
            _unitOfWork.Commit();
        }
        public IRAPSystem()
        {
            _unitOfWork = new UnitOfWork(new IRAPSqlDBContext("name=IRAPContext"));
            _systemRepo = _unitOfWork.Repository<SystemEntity>();
        }

    }
}
