﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Redditer.Models;
using Redditer.Providers;
using Redditer.Utilities;
using RedditerCore.Reddit;

namespace Redditer.ViewModels
{
    public class SubredditThreadViewModel : BaseViewModel
    {
        public SubredditThreadViewModel()
        {
            Thread = null;
            SelectedComment = null;
            IsLoadingComments = true;
            ParentViewModel = null;

            _nextListing = null;
        }

        public SubredditThread Thread
        {
            get { return _thread; }
            set
            {
                _thread = value;
                OnPropertyChanged();
            }
        }

        public Comment SelectedComment
        {
            get { return _selectedComment; }
            set
            {
                _selectedComment = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoadingComments
        {
            get { return _isLoadingComments; }
            set
            {
                _isLoadingComments = value;
                OnPropertyChanged();
                OnPropertyChanged("AreCommentsLoaded");
            }
        }

        public bool AreCommentsLoaded
        {
            get { return !_isLoadingComments; }
            set
            {
                _isLoadingComments = !value;
                OnPropertyChanged();
                OnPropertyChanged("IsLoadingComments");
            }
        }

        public SubredditViewModel ParentViewModel
        {
            get { return _parentViewModel; }
            set
            {
                _parentViewModel = value;
                OnPropertyChanged();
            }
        }

        public async void LoadComments()
        {
            IsLoadingComments = true;
            var response = await Reddit.Instance.ListComments(Thread.Link);
            var commentArray = response.AsArray();
            if (commentArray == null)
                return;

            string dummy1, dummy2;
            var opCommentListings = new RedditResponse(commentArray[0].ToString()).ParseListings(out dummy1, out dummy2);
            var userCommentsListings = new RedditResponse(commentArray[1].ToString()).ParseListings(out _nextListing, out dummy2);

            Thread.Selftext = WebUtility.HtmlDecode(opCommentListings[0].Value<JObject>("data").Value<string>("selftext"));
            OnPropertyChanged("Thread");

            var comments = new ObservableCollection<Comment>();
            LinearizeComments(comments, userCommentsListings, 0);
            Thread.Comments = comments;
            OnPropertyChanged("Thread");

            IsLoadingComments = false;
        }

        public async Task<bool> Login()
        {
            var auth = new Authenticator.Authenticator();
            await Reddit.Instance.LogIn(auth);

            OnPropertyChanged("IsLoggedIn");
            OnPropertyChanged("IsNotLoggedIn");
            OnPropertyChanged("User");
            return IsLoggedIn;
        }

        public void Logout()
        {
            var auth = new Authenticator.Authenticator();
            Reddit.Instance.LogOut(auth);

            OnPropertyChanged("IsLoggedIn");
            OnPropertyChanged("IsNotLoggedIn");
            OnPropertyChanged("User");
        }

        public async void LoadMoreComments(Comment placeholderComment)
        {
            if (!placeholderComment.LoadMoreComments)
                return;

            var response = await Reddit.Instance.ListComments(Thread.Link, placeholderComment.LoadMoreCommentsLink.Value);
            var commentsArray = response.AsArray();
            if (commentsArray == null)
                return;

            var index = Thread.Comments.IndexOf(placeholderComment);
            Thread.Comments.RemoveAt(index);

            // This listings now contains info about the parent comment, so we need to skip this one level and only parse its replies
            string dummy1, dummy2;
            var moreCommentsListings = new RedditResponse(commentsArray[1].ToString()).ParseListings(out dummy1, out dummy2);

            JToken replies;
            if (moreCommentsListings[0].Value<JObject>("data").TryGetValue("replies", out replies) && replies.Type == JTokenType.Object)
            {
                moreCommentsListings =
                    new RedditResponse(moreCommentsListings[0].Value<JObject>("data").Value<JObject>("replies").ToString()).ParseListings(out dummy1, out dummy2);
            }
            else
                return;

            var moreComments = new ObservableCollection<Comment>();
            LinearizeComments(moreComments, moreCommentsListings, placeholderComment.Depth, placeholderComment.LoadMoreCommentsChildren);

            for (var i = 0; i < moreComments.Count; ++i)
            {
                Thread.Comments.Insert(index + i, moreComments[i]);
            }
        }

        public void LinearizeComments(ObservableCollection<Comment> comments, JArray listing, int depth, List<string> validLinks = null)
        {
            foreach (var jcomment in listing)
            {
                var data = jcomment.Value<JObject>("data");
                if (validLinks != null && !validLinks.Contains(data.Value<string>("id")))
                    continue;

                if (jcomment.Value<string>("kind") == "t1")
                {
                    var comment = Comment.Parse(data);
                    comment.Depth = depth;
                    comments.Add(comment);

                    JToken replies;
                    if (data.TryGetValue("replies", out replies) && replies.ToString() != "")
                    {
                        string dummy1, dummy2;
                        var repliesResponse = new RedditResponse(replies.ToString());
                        var repliesListings = repliesResponse.ParseListings(out dummy1, out dummy2);
                        LinearizeComments(comments, repliesListings, depth + 1);
                    }
                }
                else if (jcomment.Value<string>("kind") == "more")
                {
                    var comment = new Comment
                    {
                        Depth = depth,
                        LoadMoreCommentsLink = Maybe<string>.Just(data.Value<string>("parent_id").Remove(0, 3)),
                        LoadMoreCommentsCount = data.Value<int>("count"),
                        LoadMoreCommentsChildren = data.Value<JArray>("children").ToObject<List<string>>()
                    };
                    comments.Add(comment);
                }
            }
        }

        public bool IsLoggedIn => Reddit.Instance.User.Authenticated;
        public bool IsNotLoggedIn => !IsLoggedIn;
        public string User => Reddit.Instance.User.Username;

        private SubredditThread _thread;
        private string _nextListing;
        private bool _isLoadingComments;
        private SubredditViewModel _parentViewModel;
        private Comment _selectedComment;
    }
}
