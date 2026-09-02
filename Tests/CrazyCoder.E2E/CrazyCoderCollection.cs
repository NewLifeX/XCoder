namespace CrazyCoder.E2E;

/// <summary>共享集合：全测试类共用同一个码神工具 app 实例（串行执行）</summary>
[CollectionDefinition("CrazyCoder")]
public class CrazyCoderCollection : ICollectionFixture<CrazyCoderSession>
{
}
