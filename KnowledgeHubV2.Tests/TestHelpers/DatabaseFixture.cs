using KnowledgeHubV2.Data;
using KnowledgeHubV2.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Threading;

namespace KnowledgeHubV2.Tests.TestHelpers
{
    public class DatabaseFixture : IDisposable
    {
        public ApplicationDbContext Context { get; }
        public NoteRepository NoteRepository { get; }

        public DatabaseFixture()
        {
            Context = TestDbContext.CreateInMemoryContext();
            var dbContextFactoryMock = new Mock<IDbContextFactory<ApplicationDbContext>>();
            dbContextFactoryMock.Setup(f => f.CreateDbContextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Context);
            NoteRepository = new NoteRepository(dbContextFactoryMock.Object);
        }

        public void Dispose()
        {
            Context.Dispose();
        }
    }
} 