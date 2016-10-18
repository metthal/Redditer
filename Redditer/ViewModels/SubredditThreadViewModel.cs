using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redditer.Models;

namespace Redditer.ViewModels
{
    public class SubredditThreadViewModel : BaseViewModel
    {
        public SubredditThread Thread
        {
            get { return _thread; }
            set
            {
                _thread = value;
                OnPropertyChanged();
            }
        }

        private SubredditThread _thread;
    }
}
