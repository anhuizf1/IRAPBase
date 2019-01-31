using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPBase
{
    public interface IUnitOfWork
    {
        int Commit();
        void RollBack();
    }
}
