using Specify;
using Specify.Stories;
using TestStack.BDDfy.Configuration;
using TestStack.BDDfy.Xunit;

namespace Magneto.Tests
{
	public abstract class ScenarioFor<TSut, TStory> : Specify.ScenarioFor<TSut, TStory>
		where TSut : class
		where TStory : Story, new()
	{
		[BddfyFact]
		public override void Specify()
		{
			base.Specify();
		}

		public override string Title => this.GetTitle();
	}

	public abstract class ScenarioFor<TSut> : Specify.ScenarioFor<TSut>
		where TSut : class
	{
		[BddfyFact]
		public override void Specify()
		{
			base.Specify();
		}

		public override string Title => this.GetTitle();
	}

    public static class ScenarioExtensions
    {
        public static string GetTitle<TSut, TStory>(this Specify.ScenarioFor<TSut, TStory> scenario) where TSut : class where TStory : Story, new()
        {
            var type = scenario.GetType();
			var title = Configurator.Scanners
					.Humanize(type.FullName.Replace(type.Namespace + ".", string.Empty))
					.ToTitleCase();
			if (scenario.Number != 0)
			{
				title = $"Scenario {scenario.Number:00}: {title}";
			}
			return title;
		}
    }
}
