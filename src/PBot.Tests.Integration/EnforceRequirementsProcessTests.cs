using NUnit.Framework;
using PBot.Requirements;

class EnforceRequirementsProcessTests
{
    [Test, Explicit("Careful this will execute the rules against the real repo!")]
    public async void Run()
    {
        var command = new EnforceRequirementsProcess();

        await command.Execute(null, null);
    }

}