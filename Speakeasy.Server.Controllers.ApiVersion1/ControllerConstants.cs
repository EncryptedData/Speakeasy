namespace Speakeasy.Server.Controllers.ApiVersion1;

public static class ControllerConstants
{
    public static class FileSizes
    {
        public const int KiloByte = 1024;

        public const int MegaByte = KiloByte * KiloByte;

        public const int TwoMegaBytes = MegaByte * 2;

        public const int SixTeenMegabytes = MegaByte * 16;
    }

    public static class Limits
    {
        public static class ProfilePicture
        {
            public const int Width = 400;

            public const int Height = 400;
        }

        public static class Emoji
        {
            public const int Width = 128;

            public const int Height = 128;
        }

        public static class Gif
        {
            public const int Width = 1024;

            public const int Height = 1024;

            public const int MaximumTake = 25;
        }
    }
}