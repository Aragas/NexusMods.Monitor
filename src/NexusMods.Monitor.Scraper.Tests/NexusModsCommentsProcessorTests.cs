using MediatR;

using Microsoft.Extensions.DependencyInjection;

using NexusMods.Monitor.Scraper.Application;
using NexusMods.Monitor.Scraper.Application.Commands.Comments;
using NexusMods.Monitor.Scraper.Application.Queries.Comments;
using NexusMods.Monitor.Scraper.Application.Queries.NexusModsComments;
using NexusMods.Monitor.Scraper.Application.Queries.Subscriptions;

using NodaTime;

using NSubstitute;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using Xunit;
using Xunit.Abstractions;

namespace NexusMods.Monitor.Scraper.Tests
{
    public class NexusModsCommentsProcessorTests : BaseTests
    {
        public record Case(SubscriptionViewModel[] Subscriptions, CommentViewModel[] Comments, NexusModsCommentRootViewModel[] NexusModsComments, IBaseRequest[] ExpectedCommands, [CallerMemberName] string CallerMemberName = "")
        {
            public static readonly Case[] TestCases = new []
            {
                AddsComment(),
                RemovesComment(),
                DoesNothingWithExistingEqualComment(),
            };

            private static Case AddsComment()
            {
                var comment = new NexusModsCommentRootViewModel("", 1, 1, "", "", new(1, "", "", "", "", false, false, Instant.MinValue, new List<NexusModsCommentReplyViewModel>()));
                return new(
                    new SubscriptionViewModel[] {new(1, 1)},
                    new CommentViewModel[] { },
                    new NexusModsCommentRootViewModel[] {comment},
                    new IBaseRequest[] {CommentAddCommand.FromViewModel(comment)}
                );
            }
            private static Case RemovesComment()
            {
                return new(
                    new SubscriptionViewModel[] {new(1, 1)},
                    new CommentViewModel[] {new(1, 1, 1, false, false, new List<CommentReplyViewModel>())},
                    new NexusModsCommentRootViewModel[] { },
                    new IBaseRequest[] {new CommentRemoveCommand(1)}
                );
            }
            private static Case DoesNothingWithExistingEqualComment()
            {
                var comment = new NexusModsCommentRootViewModel("", 1, 1, "", "", new(1, "", "", "", "", false, false, Instant.MinValue, new List<NexusModsCommentReplyViewModel>()));
                return new(
                    new SubscriptionViewModel[] { new(1, 1) },
                    new CommentViewModel[] { new(1, 1, 1, false, false, new List<CommentReplyViewModel>()) },
                    new NexusModsCommentRootViewModel[] { comment },
                    new IBaseRequest[] { }
                );
            }

            public override string ToString() => CallerMemberName;
        }

        public static IEnumerable<object[]> TestCasesData => Case.TestCases.Select(testCase => new object[] { testCase });

        public NexusModsCommentsProcessorTests(ITestOutputHelper outputHelper) : base(outputHelper) { }

        private ServiceProvider GetServiceProvider(Action<IBaseRequest> onMessageReceived, SubscriptionViewModel[] subscriptions, CommentViewModel[] comments, NexusModsCommentRootViewModel[] nexusModsComments)
        {
            var services = GetServiceCollection();

            services.AddSingleton<ISubscriptionQueries>(_ =>
            {
                var subscriptionQueries = Substitute.For<ISubscriptionQueries>();
                subscriptionQueries.GetAllAsync(Arg.Any<CancellationToken>()).Returns(subscriptions.ToAsyncEnumerable());
                return subscriptionQueries;
            });

            services.AddSingleton<ICommentQueries>(_ =>
            {
                var commentQueries = Substitute.For<ICommentQueries>();
                commentQueries.GetAllAsync(Arg.Any<uint>(), Arg.Any<uint>(), Arg.Any<CancellationToken>()).Returns(comments.ToAsyncEnumerable());
                return commentQueries;
            });

            services.AddSingleton<INexusModsCommentQueries>(_ =>
            {
                var nexusModsCommentQueries = Substitute.For<INexusModsCommentQueries>();
                nexusModsCommentQueries.GetAllAsync(Arg.Any<uint>(), Arg.Any<uint>(), Arg.Any<CancellationToken>()).Returns(nexusModsComments.ToAsyncEnumerable());
                return nexusModsCommentQueries;
            });

            services.AddSingleton<IMediator>(new MediatorSendInterceptor(onMessageReceived));

            services.AddSingleton<NexusModsCommentsProcessor>();

            return services.BuildServiceProvider();
        }

        [Theory]
        [MemberData(nameof(TestCasesData))]
        public async Task Test(Case @case)
        {
            var commands = new List<IBaseRequest>();

            await using var sp = GetServiceProvider(x => commands.Add(x), @case.Subscriptions, @case.Comments, @case.NexusModsComments);
            var processor = sp.GetRequiredService<NexusModsCommentsProcessor>();
            await processor.Process(CancellationToken.None);

            Assert.Equal(@case.ExpectedCommands, commands);
        }
    }
}