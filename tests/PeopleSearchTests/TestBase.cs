using System;
using Autofac;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PeopleSearch;
using PeopleSearch.Data.Contexts;
using PeopleSearch.Models;

namespace PeopleSearchTests
{

    [TestClass]
    public abstract class TestBase
    {
        protected static IContainer Container { get; set; }
        protected ILifetimeScope Scope { get; private set; }
        
        protected static void Initialize(TestContext context, Action<ContainerBuilder> containerInitializer = null)
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new PeopleSearchModule());
            Container = builder.Build();
        }

        protected static void Cleanup()
        {
            Container?.Dispose();
            Container = null;
        }

        [TestInitialize]
        public virtual void TestInitialize()
        {
            this.Scope = Container.BeginLifetimeScope();
        }

        [TestCleanup]
        public virtual void TestCleanUp()
        {
            this.Scope.Dispose();
            this.Scope = null;
        }
    }
}
