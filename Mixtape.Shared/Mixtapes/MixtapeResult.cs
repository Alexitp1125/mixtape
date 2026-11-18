namespace Mixtape.Shared.Mixtapes
{
    // Result of a change to a mixtape.
    public sealed class MixtapeResult
    {
        public MixtapeStatus Status { get; init; }

        // Slug of the mixtape or the song that was created, when Status is Ok; null otherwise.
        public string? Slug { get; init; }
    }
}
