﻿using System;
using System.Linq;
using InstaSharper.Classes;
using InstaSharper.Tests.Utils;
using Xunit;
using Xunit.Abstractions;
using InstaSharper.Classes.Models;

namespace InstaSharper.Tests.Endpoints
{
    [Collection("Endpoints")]
    public class MediaTest
    {
        private readonly ITestOutputHelper _output;

        public MediaTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [RunnableInDebugOnlyTheory]
        [InlineData("1484832969772514291")]
        public async void GetMediaByIdTest(string mediaId)
        {
            //arrange
            var username = "alex_codegarage";
            var password = Environment.GetEnvironmentVariable("instaapiuserpassword");
            var apiInstance = TestHelpers.GetDefaultInstaApiInstance(new UserSessionData
            {
                UserName = username,
                Password = password
            });
            //act
            _output.WriteLine($"Trying to login as user: {username}");
            if (!TestHelpers.Login(apiInstance, _output)) return;
            _output.WriteLine($"Getting media by ID: {mediaId}");
            var media = await apiInstance.GetMediaByIdAsync(mediaId);
            //assert
            Assert.NotNull(media);
        }

        [RunnableInDebugOnlyTheory]
        [InlineData("1379932752706850783")]
        public async void GetMediaLikersTest(string mediaId)
        {
            //arrange
            var username = "alex_codegarage";
            var password = Environment.GetEnvironmentVariable("instaapiuserpassword");
            var apiInstance = TestHelpers.GetDefaultInstaApiInstance(new UserSessionData
            {
                UserName = username,
                Password = password
            });
            //act
            _output.WriteLine($"Trying to login as user: {username}");
            if (!TestHelpers.Login(apiInstance, _output)) return;
            _output.WriteLine($"Getting media [{mediaId}] likers");
            var likers = await apiInstance.GetMediaLikersAsync(mediaId);
            var anyDuplicate = likers.Value.GroupBy(x => x.Pk).Any(g => g.Count() > 1);
            //assert
            Assert.NotNull(likers);
            Assert.False(anyDuplicate);
        }

        [RunnableInDebugOnlyTheory]
        [InlineData("1379932752706850783")]
        public async void GetMediaCommentsTest(string mediaId)
        {
            //arrange
            var username = "alex_codegarage";
            var password = Environment.GetEnvironmentVariable("instaapiuserpassword");
            var apiInstance = TestHelpers.GetDefaultInstaApiInstance(new UserSessionData
            {
                UserName = username,
                Password = password
            });
            //act
            _output.WriteLine($"Trying to login as user: {username}");
            if (!TestHelpers.Login(apiInstance, _output)) return;
            _output.WriteLine($"Getting media [{mediaId}] comments");
            var comments = await apiInstance.GetMediaCommentsAsync(mediaId, 0);
            var anyDuplicate = comments.Value.Comments.GroupBy(x => x.Pk).Any(g => g.Count() > 1);
            //assert
            Assert.NotNull(comments);
            Assert.False(anyDuplicate);
        }

        [RunnableInDebugOnlyTheory]
        [InlineData("alex_codegarage")]
        public async void GetUserMediaListTest(string userToFetch)
        {
            //arrange
            var username = "alex_codegarage";
            var password = Environment.GetEnvironmentVariable("instaapiuserpassword");
            var apiInstance = TestHelpers.GetDefaultInstaApiInstance(new UserSessionData
            {
                UserName = username,
                Password = password
            });
            var random = new Random(DateTime.Today.Millisecond);
            var pages = 5;
            //act
            _output.WriteLine($"Trying to login as user: {username}");
            if (!TestHelpers.Login(apiInstance, _output)) return;
            _output.WriteLine($"Getting posts of user: {userToFetch}");

            var posts = await apiInstance.GetUserMediaAsync(userToFetch, pages);
            var anyDuplicate = posts.Value.GroupBy(x => x.Code).Any(g => g.Count() > 1);

            //assert
            Assert.NotNull(posts);
            Assert.Equal(userToFetch, posts.Value[random.Next(0, posts.Value.Count)].User.UserName);
            Assert.False(anyDuplicate);
        }

        [RunnableInDebugOnlyTheory]
        [InlineData("1484832969772514291_196754384")]
        public async void PostDeleteCommentTest(string mediaId)
        {
            //arrange
            var username = "alex_codegarage";
            var password = Environment.GetEnvironmentVariable("instaapiuserpassword");
            var apiInstance = TestHelpers.GetDefaultInstaApiInstance(new UserSessionData
            {
                UserName = username,
                Password = password
            });
            var text = "test comment";
            //act
            if (!TestHelpers.Login(apiInstance, _output)) return;

            var postResult = await apiInstance.CommentMediaAsync(mediaId, text);
            var delResult = await apiInstance.DeleteCommentAsync(mediaId, postResult.Value.Pk);
            //assert
            Assert.True(postResult.Succeeded);
            Assert.Equal(text, postResult.Value.Text);
            Assert.True(delResult.Succeeded);
        }

        [Theory]
        [InlineData("1510405963000000025_1414585238", InstaMediaType.Image)]
        public async void DeleteMediaPhotoTest(string mediaId, InstaMediaType mediaType)
        {
            //arrange
            var username = "thisidlin";
            var password = System.IO.File.ReadAllText(@"C:\privKey\instasharp.txt");
            var apiInstance = TestHelpers.GetDefaultInstaApiInstance(new UserSessionData
            {
                UserName = username,
                Password = password
            });
            //act
            if (!TestHelpers.Login(apiInstance, _output)) return;
            var deleteMediaPhoto = await apiInstance.DeleteMediaAsync(mediaId, mediaType);
            //assert
            Assert.False(deleteMediaPhoto.Value); //As the media doesn't exists
        }

        [Theory]
        [InlineData("1510414591769980888_1414585238", InstaMediaType.Video)]
        public async void DeleteMediaVideoTest(string mediaId, InstaMediaType mediaType)
        {
            //arrange
            var username = "thisidlin";
            var password = System.IO.File.ReadAllText(@"C:\privKey\instasharp.txt");
            var apiInstance = TestHelpers.GetDefaultInstaApiInstance(new UserSessionData
            {
                UserName = username,
                Password = password
            });
            //act
            if (!TestHelpers.Login(apiInstance, _output)) return;
            var deleteMediaVideo = await apiInstance.DeleteMediaAsync(mediaId, mediaType);
            //assert
            Assert.True(deleteMediaVideo.Value);
        }

        [Theory]
        [InlineData("1513736003209429255_1414585238", "Hello!")]
        public async void EditMediaTest(string mediaId, string caption)
        {
            //arrange
            var username = "thisidlin";
            var password = System.IO.File.ReadAllText(@"C:\privKey\instasharp.txt");
            var apiInstance = TestHelpers.GetDefaultInstaApiInstance(new UserSessionData
            {
                UserName = username,
                Password = password
            });
            //act
            if (!TestHelpers.Login(apiInstance, _output)) return;
            var editMedia = await apiInstance.EditMediaAsync(mediaId, caption);
            //assert
            Assert.True(editMedia.Value);
        }
    }
}