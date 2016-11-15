using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redditer.Utilities;

namespace Redditer.Models
{
    public class Comment
    {
        public Comment()
        {
            LoadMoreCommentsLink = Maybe<string>.Nothing();
        }

        public int Depth { get; set; }

        public string Name { get; set; }
        public string Text { get; set; }
        public string Author { get; set; }
        public int Score { get; set; }
        public string Flair { get; set; }
        public DateTime Created { get; set; }
        public Maybe<DateTime> Edited { get; set; }
        public int Gilded { get; set; }
        public bool IsGilded => Gilded > 0;
        public bool IsMultigilded => Gilded > 1;
        public Maybe<bool> Likes { get; set; }
        public bool Upvoted => Likes!= null && Likes.Defined && Likes.Value;
        public bool Downvoted => Likes != null && Likes.Defined && !Likes.Value;
        public bool Voted => Upvoted || Downvoted;
        public bool RegularComment => !LoadMoreComments;

        public Maybe<string> LoadMoreCommentsLink { get; set; }
        public bool LoadMoreComments => LoadMoreCommentsLink.Defined;
        public int LoadMoreCommentsCount { get; set; }
        public List<string> LoadMoreCommentsChildren { get; set; }
    }
}
