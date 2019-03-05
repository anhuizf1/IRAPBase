using IRAPBase.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase
{
    public class IRAPLog
    {
        private IQueryable<LoginEntity> _loginLog = null;

        public IQueryable<LoginEntity> LoginLog { get { return _loginLog; } }
        public IRAPLog()
        {
            _loginLog=  new Repository<LoginEntity>("IRAPContext").Table;
            
        }
    }
}
