using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedditerCore.Reddit;

namespace Redditer.Providers
{
    public class Reddit
    {
        public static RedditClient Instance => _instance ?? (_instance = new RedditClient());

        private static RedditClient _instance;
    }
}
