using DataAccess.Model.SharedModels;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

namespace NotificationService
{
    public interface INotificationService
    {
        bool Notify(NotificationContent content);
    }
}
