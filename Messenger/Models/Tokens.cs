using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Models
{
    public class Tokens
    {
        [Required]
        public string access_token { get; set; }
        [Required]
        public string refresh_token { get; set; }
    }
}
