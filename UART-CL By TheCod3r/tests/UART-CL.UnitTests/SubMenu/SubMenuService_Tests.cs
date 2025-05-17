using FluentAssertions;
using NUnit.Framework;
using UART_CL_By_TheCod3r.SubMenu;

namespace UART_CL.UnitTests.SubMenu;

public class SubMenuService_Tests
{
    private Dictionary<string, string> _regionMap = null!;
    private List<string> _output = null!;
    private Action<string> _writeLine = null!;
    private Func<string> _readLine = null!;

    [SetUp]
    public void SetUp()
    {
        _regionMap = new Dictionary<string, string> { { "US1", "United States" } };
        _output = new List<string>();
        _writeLine = s => _output.Add(s);
        _readLine = () => ""; // simulate Enter key
    }

    [Test]
    public void RunSubMenu_ShouldShowNoPathErrorMessage_WhenViewBIOSWithoutFile()
    {
        var inputs = new Queue<string>(new[] { "2", "", "X" });
        _readLine = () => inputs.Dequeue();

        SubMenuService.RunSubMenu(
            "AppTitle",
            _regionMap,
            _readLine,
            _writeLine,
            loadDumpFile: (r, w, f1, f2, a) => null!,
            setConsoleTitle: _ => { },
            pathToDump: ""
        );

        _output.Should().Contain("You must select a .bin file to read before proceeding. " +
            "Please select a valid .bin file and try again.");
        _output.Should().Contain("Press Enter to continue...");
    }

    [Test]
    public void RunSubMenu_ShouldExit_WhenUserChooses_X()
    {
        var inputs = new Queue<string>(new[] { "X" });
        _readLine = () => inputs.Dequeue();

        var titleSet = "";
        void SetTitle(string title) => titleSet = title;

        SubMenuService.RunSubMenu(
            "AppTitle",
            _regionMap,
            _readLine,
            _writeLine,
            loadDumpFile: (r, w, f1, f2, a) => null!,
            setConsoleTitle: SetTitle,
            pathToDump: ""
        );

        titleSet.Should().Be("AppTitle");
    }
}
