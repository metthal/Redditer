using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redditer.Models
{
    public class Comment
    {
        public int Depth { get; set; }
        public string Text { get; set; }
        public string Author { get; set; }
        public int Score { get; set; }
    }
}
