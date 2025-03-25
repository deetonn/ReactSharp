using Reactivity;

var config = new Config();

config.TitleEnabled.Value = false;

public class Config
{
    public Property<bool> TitleEnabled { get; set; } = new(false);

    public Config()
    {
        React.UseEffect(() =>
        {
            if (TitleEnabled.Value)
            {
                Console.WriteLine("Title has been enabled.");
            } 
            else
            {
                Console.WriteLine("Title has been disabled.");
            }
        }, new DependencyList([TitleEnabled.State()]));
    }
}