namespace Mixtape.Shared.Mixtapes
{
    // How a change to a mixtape ended.
    public enum MixtapeStatus
    {
        Ok,

        // The mixtape or the song doesn't exist or belongs to someone else.
        NotFound,

        // The mixtape to move a song to isn't another mixtape of the same person.
        TargetInvalid,

        // The name of the mixtape doesn't have 1 to 30 characters.
        NameInvalid,

        // The title of the song doesn't have 1 to 120 characters.
        TitleInvalid,

        // The artist of the song doesn't have 1 to 120 characters.
        PerformerInvalid,

        // The cover isn't one of the generic ones.
        CoverInvalid,

        // The ID of the video doesn't have the shape of a YouTube ID.
        VideoInvalid,

        TooManyMixtapes,

        TooManyTracks,
    }
}
