using System;
using System.Collections.Generic;
using System.Linq;
using BITTreeHole.Data.Entities;

namespace BITTreeHole.Test.UnitTest.Mocks
{
    public sealed class MockDataFacadeBuilder
    {
        private IEnumerable<UserEntity> _users;
        private IEnumerable<PostRegionEntity> _postRegions;
        private IEnumerable<PostEntity> _posts;
        private IEnumerable<CommentEntity> _comments;
        private IEnumerable<UserVotePostEntity> _votes;

        public MockDataFacadeBuilder()
        {
            _users = new UserEntity[0];
            _postRegions = new PostRegionEntity[0];
            _posts = new PostEntity[0];
            _comments = new CommentEntity[0];
            _votes = new UserVotePostEntity[0];
        }

        public MockDataFacadeBuilder SetUsers(IEnumerable<UserEntity> users)
        {
            _users = users ?? throw new ArgumentNullException(nameof(users));
            return this;
        }

        public MockDataFacadeBuilder SetPostRegions(IEnumerable<PostRegionEntity> postRegions)
        {
            _postRegions = postRegions ?? throw new ArgumentNullException(nameof(postRegions));
            return this;
        }

        public MockDataFacadeBuilder SetPosts(IEnumerable<PostEntity> posts)
        {
            _posts = posts ?? throw new ArgumentNullException(nameof(posts));
            return this;
        }

        public MockDataFacadeBuilder SetComments(IEnumerable<CommentEntity> comments)
        {
            _comments = comments ?? throw new ArgumentNullException(nameof(comments));
            return this;
        }

        public MockDataFacadeBuilder SetVotes(IEnumerable<UserVotePostEntity> votes)
        {
            _votes = votes ?? throw new ArgumentNullException(nameof(votes));
            return this;
        }

        public MockDataFacade Build()
        {
            return new MockDataFacade
            {
                Users = _users.AsQueryable(),
                PostRegions = _postRegions.AsQueryable(),
                Posts = _posts.AsQueryable(),
                Comments = _comments.AsQueryable(),
                UserVotePosts = _votes.AsQueryable()
            };
        }
    }
}
