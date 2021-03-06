﻿using GameStats.Server.Controllers;
using GameStats.Server.Models;
using GameStats.Utility.Tools;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace GameStats.Tests
{
    [Collection("ReportsController tests")]
    public class ReportsControllerTests : IDisposable
    {
        public string DbPath = @".\GameStats.Storage.db";
        public ReportsController controller;
        private DatabaseContext context;
        private ServersController serversController;

        public ReportsControllerTests()
        {
            context = new DatabaseContext("Filename=" + DbPath);
            controller = new ReportsController(context);
            serversController = new ServersController(context);
        }

        public void Dispose()
        {
            if (File.Exists(DbPath))
                File.Delete(DbPath);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(25)]
        [InlineData(50)]
        [InlineData(55)]
        public void GET_RecentMatches_OK(int count)
        {
            Server.Models.Server server = RandomModels.CreateRandomServer();
            Assert.True(serversController.PutServerInfo(server.Endpoint, server.Info) is OkResult);

            for (int i = 0; i <= count + 2; i++)
            {
                Match match = RandomModels.CreateRandomMatch(server);
                Assert.True(serversController.PutMatch(server.Endpoint, match.Timestamp, match) is OkResult);
            }

            IEnumerable<Match> matches = (IEnumerable<Match>)((ObjectResult)controller.GetRecentMatches(count)).Value;
            if (count < 0) Assert.Equal(0, matches.Count());
            if (count > 50) Assert.Equal(50, matches.Count());
            if (count <= 50 && count >= 0) Assert.Equal(count, matches.Count());
        }
    }
}
