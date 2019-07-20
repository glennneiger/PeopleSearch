using Autofac;
using PeopleSearch.Data.Services;

namespace PeopleSearch
{
    public class PeopleSearchModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PeopleService>()
                .As<IPeopleService>()
                .InstancePerLifetimeScope();
        }
    }
}
