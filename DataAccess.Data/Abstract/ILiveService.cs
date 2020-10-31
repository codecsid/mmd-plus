using DataAccess.Model;
using DataAccess.Model.SharedModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Data.Abstract
{
    public interface ILiveService
    {
        public Task<LiveResponse> GetLiveStatus();

    }
}
