namespace CleanArchitecture.Application.Common.Abstracts.ClinetInfo
{
    public interface ICurrentRequestInfoService
    {
        public string? Host { get; init; }
        public string? Method { get; init; }
        public string? Path { get; init; }
        public string? PathBase { get; init; }
        public string? Protocol { get; init; }

    }
}
