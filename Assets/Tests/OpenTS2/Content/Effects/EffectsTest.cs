using NUnit.Framework;
using OpenTS2.Content;

public class EffectsTest
{
    [Test]
    public void testLoadsEffects()
    {
        TestMain.Initialize();
        var contentProvider = ContentProvider.Get();
        contentProvider.AddPackage("C:\\Program Files (x86)\\EA Games\\The Sims 2 Ultimate Collection\\The Sims 2\\TSData\\Res\\Effects\\effects.package");

        foreach (var abstractAsset in contentProvider.GetAssetsOfType(0xEA5118B0))
        {
            TestContext.Out.WriteLine("got an asset, " + abstractAsset.GlobalTGI);
        }
    }
    
}