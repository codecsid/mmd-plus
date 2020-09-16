using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataAccess.Model.SharedModels
{
    public class TeamMember
    {
        [Required(ErrorMessage = "Field Required"), StringLength(60, ErrorMessage = "Character Limit Exceeded")]
        [EmailAddress(ErrorMessage = "Invalid Email Address"), RegularExpression(@"^[a-zA-Z0-9_.+-]+@(?:(?:[a-zA-Z0-9-]+\.)?[a-zA-Z]+\.)?(db|gmail|google)\.com$", ErrorMessage = "Invalid Email Address")]
        public string EmailId { get; set; }
    }
}
