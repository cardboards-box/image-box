namespace ImageBox.Tests;

[TestClass]
public class GenericTests : TestSetup
{
    [TestMethod]
    public void UriTest()
    {
        (string, IOPathType)[] uris =
        [
            ("F://this/file/doesn't/exist.zip", IOPathType.LOCAL_ABSOLUTE),
            ("F:\\this\\file\\doesn't\\exist.zip", IOPathType.LOCAL_ABSOLUTE),
            ("https://static.index-0.com/image/dance.gif", IOPathType.REMOTE_HTTP),
            ("http://static.index-0.com/image/dance.gif", IOPathType.REMOTE_HTTP),
            ("ftp://static.index-0.com/image/dance.gif", IOPathType.REMOTE_FTP),
            ("ftps://static.index-0.com/image/dance.gif", IOPathType.REMOTE_FTP),
            ("./this/file/doesnt/exist/either.zip", IOPathType.LOCAL_RELATIVE),
            ("../this/file/doesnt/exist/either.zip", IOPathType.LOCAL_RELATIVE),
            (".\\this\\file\\doesnt\\exist\\either.zip", IOPathType.LOCAL_RELATIVE),
            ("..\\this\\file\\doesnt\\exist\\either.zip", IOPathType.LOCAL_RELATIVE),
            ("~/this/file/doesnt/exist/either.zip", IOPathType.LOCAL_RELATIVE_TILDE),
            ("~\\this\\file\\doesnt\\exist\\either.zip", IOPathType.LOCAL_RELATIVE_TILDE),
        ];

        foreach (var (test, expected) in uris)
        {
            var path = new IOPath(test);
            Assert.AreEqual(expected, path.Type, $"Path Type Test: {test}");
        }
    }
}
