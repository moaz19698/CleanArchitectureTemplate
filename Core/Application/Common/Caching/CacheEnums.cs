namespace CleanArchitecture.Application.Common.Caching;
public enum CacheKeysPrefixes
{
    Cart = 0,
    Product = 1,
}
public enum CacheStore
{
    InMemory = 1,
    Redis = 2,
    All = 3,
}
