using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BITTreeHole.Data;
using BITTreeHole.Data.Entities;
using MongoDB.Bson;

namespace BITTreeHole.Test.UnitTest.Mocks
{
    public sealed class MockDataFacade : IDataFacade
    {
        /// <inheritdoc />
        public IQueryable<UserEntity> Users { get; set; }
        
        /// <inheritdoc />
        public IQueryable<PostRegionEntity> PostRegions { get; set; }
        
        /// <inheritdoc />
        public IQueryable<PostEntity> Posts { get; set; }
        
        /// <inheritdoc />
        public IQueryable<CommentEntity> Comments { get; set; }
        
        /// <inheritdoc />
        public IQueryable<UserVotePostEntity> UserVotePosts { get; set; }
        
        /// <inheritdoc />
        public IQueryable<UserWatchPostEntity> UserWatchPosts { get; set; }
        
        /// <inheritdoc />
        public void AddUser(UserEntity user)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void RemoveUser(UserEntity user)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public void AddPostRegion(PostRegionEntity postRegion)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public void RemovePostRegion(PostRegionEntity postRegion)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public void AddPostIndexEntity(PostEntity postIndexEntity)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public async Task AddPostContentEntity(PostContentEntity postContentEntity)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public async Task<PostContentEntity> FindPostContentEntity(ObjectId contentId)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public async Task<List<PostContentEntity>> FindPostContentEntities(IEnumerable<ObjectId> contentIds)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public async Task UpdatePostContentText(ObjectId postContentId, string text)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public async Task UpdatePostContentImageIds(ObjectId postContentId, IReadOnlyDictionary<int, ObjectId?> positionValue)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public async Task<ObjectId> UploadImage(Stream imageDataStream)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public async Task RemoveImage(ObjectId imageId)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public void AddVoteEntity(UserVotePostEntity entity)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public void RemoveVoteEntity(UserVotePostEntity entity)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public void AddCommentIndexEntity(CommentEntity indexEntity)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public async Task AddCommentContentEntity(CommentContentEntity contentEntity)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public async Task<List<CommentContentEntity>> FindCommentContentEntities(IEnumerable<ObjectId> contentEntityIds)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public async Task CommitChanges()
        {
            throw new System.NotImplementedException();
        }
    }
}
